using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Food_Diary
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutPage : ContentPage
	{
		public AboutPage ()
		{
			Title = "О программе";
			InitializeComponent ();
		}
	}
}