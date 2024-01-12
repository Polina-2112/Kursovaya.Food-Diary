using System;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;

namespace Food_Diary
{
	public partial class AuthPage : ContentPage
	{
		public AuthPage()
		{
            Title = "Вход и регистрация";
			InitializeComponent ();
            if (App.dbContext.GetID() != -1) 
            {
                Navigation.PushModalAsync(new MainPage());
            }
		}


        // Password hashing
        protected string Hashing(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashedPassword;
            }
        }


        // Sign in
        private async void InButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("********* In the Sign In; id = " + App.dbContext.GetID());
            string log = LoginField.Text;
            string pass = PassField.Text; pass = Hashing(pass);
            if (await App.dbContext.CheckUser(log, pass))
            {
                await Navigation.PushModalAsync(new NavigationPage(new MainPage())
                { 
                    Padding = 0,
                    BarBackgroundColor = Color.FromHex("#B9D0CD"),
                    BarTextColor = Color.FromHex("#FF9376"),
                    HeightRequest = 50
                }); ;
            }
            else await DisplayAlert("Ошибка!", "Неверное имя или пароль или пользователь не существует!", "ОК");
        }

        // Sign up
        private async void RegButton_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("********* In the Sign Up; id = " + App.dbContext.GetID());
            string log = LoginField.Text;
            string pass = PassField.Text; pass = Hashing(pass);
            int res = await App.dbContext.AddUser(log, pass);
            if (res == 0) await DisplayAlert("Внимание!", "Такой пользователь уже существует!", "ОК");
            else
            {
                if (res == -1) await DisplayAlert("Внимание!", "Возникла ошибка, попробуйте позже!", "ОК");
                else
                {
                    if (res == 1)
                    { 
                        await Navigation.PushModalAsync(new NavigationPage(new MainPage())
                        {
                            Padding = 0,
                            BarBackgroundColor = Color.FromHex("#B9D0CD"),
                            BarTextColor = Color.FromHex("#FF9376"),
                            HeightRequest = 50
                        });
                    }
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            System.Environment.Exit(0);
            return true;
        }
    }
}