using SQLite;

namespace Notes.Models
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Date { get; set; }
        public string Record { get; set; }
        public int Calories { get; set; }
        public int User_ID { get; set; }
    }
}
