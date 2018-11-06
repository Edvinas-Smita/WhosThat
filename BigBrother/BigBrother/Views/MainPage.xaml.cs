using Xamarin.Forms;

namespace BigBrother.Views
{
	public partial class MainPage : CarouselPage
	{
		private string credentials = null;
		public MainPage()
		{
			InitializeComponent();
			if (credentials == null && false)	//TODO: remove && false to get login prompt
			{
				RequestLogin();
			}
		}

		private async void RequestLogin()
		{
			await Navigation.PushModalAsync(new LoginPage());
		}
	}
}