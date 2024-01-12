using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Food_Diary
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReferencePage : ContentPage
    {
        public ReferencePage()
        {
            Title = "Справочник";
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            collectionView_ref.ItemsSource = await App.dbContext.GetReferences();
            base.OnAppearing();
        }
    }
}