using System.Collections.Generic;
using Notes.Models;
using References.Models;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using Npgsql;
using Xamarin.Forms;

namespace DBMethods.Data
{
    public class DBContext
    {
        protected const string Connection = "Host=192.168.215.45;Port=5432;Database=food_diary;Username=postgres;Password=21122002";
        protected static NpgsqlConnection connection;

        //Creating the class object and the connection to the BD
        public DBContext() 
        {
            try
            {
                connection = new NpgsqlConnection(Connection);
                connection.Open();
                Debug.WriteLine("************ Connected to the database ***************");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*********** Error: {ex.Message}");
            }
        }

        // Getting and saving the User ID
        public int GetID()
        {
            Debug.WriteLine("********************* In the GetID ***********************");
            Debug.WriteLine("************** Will return " + Application.Current.Properties["User_ID"]);
            if (Application.Current.Properties["User_ID"] != null)
                return Convert.ToInt32(Application.Current.Properties["User_ID"]);
            else return -1;
        }
        public async void SetID(int User_ID)
        {
            Debug.WriteLine("********************* In the SetID ***********************");
            Application.Current.Properties["User_ID"] = User_ID;
            Debug.WriteLine("************** Will set " + Application.Current.Properties["User_ID"]);
            await Application.Current.SavePropertiesAsync();
        }

        // Adding login and password
        public async Task<int> AddUser(string Login, string password)
        {
            bool check = await CheckUser(Login, password);
            if (check) return 0; // If user already exist
            else
            {
                Debug.WriteLine("************ In the AddUser *************");
                string sql = "INSERT INTO users (login, password) VALUES (@log, @pass);";
                try
                {
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("log", Login);
                        command.Parameters.AddWithValue("pass", password);
                        await command.ExecuteNonQueryAsync();
                        return 1; // If user was added
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"********************** Error: {ex.Message}");
                    return -1; // If there was another error
                }
            }
        }

        // Checking if user exist
        public async Task<bool> CheckUser(string Login, string hashedPassword)
        {
            string sql = "SELECT ID FROM Users WHERE Login = @log AND Password = @hashpass";
            try
            {
                Debug.WriteLine("************ In the CheckUser *************");
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("log", Login);
                    command.Parameters.AddWithValue("hashpass", hashedPassword);

                    object result = await command.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out int userId))
                    {
                        SetID(userId);
                        Debug.WriteLine("************* User ID = " + userId);
                        return true;
                    }
                    else return false;
                }            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"******************** Error: {ex.Message}");
                return false;
            }
        }

        // Getting notes
        public async Task<List<Note>> GetNotes(int User_ID, string date)
        {
            Debug.WriteLine("************ In the GetNotes *************");
            List<Note> notes = new List<Note>();
            string sql = "SELECT * FROM Data WHERE User_ID = @UID AND Date = @D";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("UID", User_ID);
                command.Parameters.AddWithValue("D", date);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Note note = new Note
                        {
                            ID = reader.GetInt32(0),
                            Record = reader.GetString(1),
                            Calories = reader.GetInt32(2),
                            User_ID = reader.GetInt32(3),
                            Date = reader.GetString(4)
                        };
                        notes.Add(note);
                    }
                }
            }
            return notes;
        }

        // Updating note
        public async Task<bool> UpdateNote(Note note)
        {
            Debug.WriteLine("************ In the UpdateNote *************");
            Debug.WriteLine("************ NoteID = " +note.ID + " *************");
            //string sql = "UPDATE Note SET Date = @date, Record = @record, Calories = @calories WHERE ID = @id";
            string sql = "UPDATE Data SET Record = @record, Calories = @calories WHERE ID = @id";
            try
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("id", note.ID);
                    //command.Parameters.AddWithValue("date", note.Date);
                    command.Parameters.AddWithValue("record", note.Record);
                    command.Parameters.AddWithValue("calories", note.Calories);

                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"************************ Error: {ex.Message}");
                return false;
            }
        }

        // Adding note
        public async Task<bool> AddNote(Note note)
        {
            try
            {
                Debug.WriteLine("************ In the AddNote *************");
                string sql = "INSERT INTO Data (Date, Record, Calories, User_ID) VALUES (@date, @record, @calories, @userId)";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("date", note.Date);
                    command.Parameters.AddWithValue("record", note.Record);
                    command.Parameters.AddWithValue("calories", note.Calories);
                    command.Parameters.AddWithValue("userId", GetID());

                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"************** Error: {ex.Message}");
                return false;
            }
        }

        // Getting sum of calories
        public async Task<int> SumCalories(string date, int User_ID)
        {
            Debug.WriteLine("************ In the SumCalories *************");
            int Cal;
            string sql = "SELECT SUM(Calories) AS TotalCalories FROM Data WHERE Date = @D AND User_ID = @UID;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("D", date);
                command.Parameters.AddWithValue("UID", User_ID);

                object result = await command.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out Cal))
                { }
                else Cal = 0;
            }
            return Cal;
        }

        // Deleting note
        public async Task DeleteNote(Note note)
        {
            Debug.WriteLine("************ In the DeleteNote *************");
            string sql = "DELETE FROM data WHERE id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("id", note.ID);
                await command.ExecuteNonQueryAsync();
            }
        }

        // Geting reference records
        public async Task<List<Reference>> GetReferences()
        {
            List<Reference> references = new List<Reference>();

            string sql = "SELECT Name, Calories FROM Reference ORDER BY Name";

            using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Reference R = new Reference
                        {
                            Name = reader.GetString(0),
                            Calories = reader.GetInt32(1)
                        };
                        references.Add(R);
                    }
                }
            }
            return references;
        }
    }
}
