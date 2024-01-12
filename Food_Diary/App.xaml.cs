using Xamarin.Forms;
using DBMethods.Data;
using System.Diagnostics;

namespace Food_Diary
{
    public partial class App : Application
    {
        public static DBContext dbContext;
        
        public App()
        {
            InitializeComponent();

            Debug.WriteLine("************ In the OnAppearing ***********");
            if (!Application.Current.Properties.ContainsKey("FirstRun"))
            {
                Application.Current.Properties["User_ID"] = null;
                Application.Current.SavePropertiesAsync();
                Application.Current.Properties["FirstRun"] = true;
                Application.Current.SavePropertiesAsync();
            }

            dbContext = new DBContext();

            MainPage = new NavigationPage(new AuthPage())
            {
                BarBackgroundColor = Color.FromHex("#B9D0CD"),
                BarTextColor = Color.FromHex("#FF9376"),
                HeightRequest = 80
            };
        }

        protected override void OnStart() { }
        protected override void OnSleep() { }
        protected override void OnResume() { }
    }
}
