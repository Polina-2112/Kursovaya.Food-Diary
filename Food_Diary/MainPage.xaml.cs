using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Notes.Models;
using System.Diagnostics;

namespace Food_Diary
{
    public partial class MainPage : ContentPage
    {
        string date;
        DateTime NowDate = DateTime.Today;
        public MainPage()
        {
            Title = "Дневник питания";
            InitializeComponent();
            Device.SetFlags(new string[] { "Shapes_Experimental" });
        }

      
        // Updating sources in collection view
        protected async Task Update(string date)
        {
            collectionView.ItemsSource = await App.dbContext.GetNotes(App.dbContext.GetID(), date);
            allCaloriesField.Text = Convert.ToString(await App.dbContext.SumCalories(date, App.dbContext.GetID()));
        }


        // Setting the date for the field
        protected void DateSet()
        {
            date = Convert.ToString(NowDate.Day) + '.' + Convert.ToString(NowDate.Month);
            datefield.Text = date;
        }       


        // When the page appearing
        protected override async void OnAppearing()
        {
            DateSet();
            await Update(date);
            base.OnAppearing();
        }


        // When the note was selected
        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                Note sel_note = (Note)e.CurrentSelection.FirstOrDefault();

                string str = sel_note.Record; str += ", "; str += Convert.ToString(sel_note.Calories);

                string result = await DisplayPromptAsync(title: "Редактирование записи",
                    message: "Название, каллорийность", accept: "Сохранить", cancel: "Удалить",
                    initialValue: str);

                if (result != null)
                {
                    string[] rec = result.Split(new char[] { ',' });
                    if (rec.Length != 2)
                    {
                        await DisplayAlert("Ошибка ввода", "Введите полную запись, например, \"Обед, 800\"", "OK");
                    }
                    else
                    {
                        sel_note.Record = rec[0].Trim();
                        sel_note.Calories = Convert.ToInt32(rec[1].Trim());
                        await App.dbContext.UpdateNote(sel_note);
                        await Update(date);
                    }
                }
                else
                {
                    await App.dbContext.DeleteNote(sel_note);
                    await Update(date);
                }
                collectionView.SelectedItems.Clear();
            }
        }


        // When navigating throw date lists
        private async void LeftArrow_Clicked(object sender, EventArgs e)
        {
            NowDate = NowDate.AddDays(-1);
            DateSet(); await Update(date);
        }
        private async void RightArrow_Clicked(object sender, EventArgs e)
        {
            NowDate = NowDate.AddDays(1);
            if (NowDate <= DateTime.Today)
            {
                DateSet(); await Update(date);
            }
            else NowDate = NowDate.AddDays(-1);
        }


        // When adding note
        private async void Plus_Clicked(object sender, EventArgs e)
        {
            if (NowDate == DateTime.Today)
            {
                Note AddingNote = new Note();

                string result = await DisplayPromptAsync(title: "Новая запись",
                    message: "Название, каллорийность", accept: "Добавить", cancel: "Отмена",
                    placeholder: "Греческий салат, 170");

                Debug.WriteLine("**************** " + result);

                if (result != null)
                {
                    string[] rec = result.Split(new char[] { ',' });
                    if (rec.Length == 2)
                    {
                        Debug.WriteLine("******** In the checking note *********");
                        AddingNote.Record = rec[0].Trim();
                        AddingNote.Calories = Convert.ToInt32(rec[1].Trim());
                        AddingNote.Date = date;
                        AddingNote.User_ID = App.dbContext.GetID();
                        await App.dbContext.AddNote(AddingNote);
                        await Update(date);
                    }
                    else
                        await DisplayAlert("Ошибка ввода", "Введите полную запись, например, \"Обед, 800\"", "OK");
                }
            }            
        }


        // When want to go to reference or about page
        private async void PoleznostiButton_Clicked(object sender, EventArgs e)
        { 
            string res = await DisplayActionSheet("Полезности", "Выйти", null, "Таблица калорий", "О программе");
            if (res == "Таблица калорий") await Navigation.PushAsync(new ReferencePage());
            if (res == "О программе") await Navigation.PushAsync(new AboutPage());
        }

        private async void SignOut_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Внимание!", "Вы действительно хотите выйти из аккаунта?", "Да", "Нет");
            if (result)
            {
                App.dbContext.SetID(-1);
                await Navigation.PushModalAsync(new AuthPage());
            }
        }

        protected override bool OnBackButtonPressed()
        {
            System.Environment.Exit(0);
            return true;
        }
    }
}
