using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BigBrother.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BigBrother
{
	public partial class App : Application
	{

		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			Quit();	//TODO: temp so I wouldnt have to kill the app everytime to stop the debugger
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
