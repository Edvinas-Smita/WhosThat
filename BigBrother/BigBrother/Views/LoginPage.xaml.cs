using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigBrother.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BigBrother.Views
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			var viewModel = new LoginViewModel(Navigation);
			BindingContext = viewModel;
			viewModel.InvalidCredentialsPopup += () => DisplayAlert("Error", "Email and/or password are wrong!", "OK");
			InitializeComponent ();

			Email.Completed += (object sender, EventArgs e) => { Password.Focus(); };
			Password.Completed += (object sender, EventArgs e) =>
			{
				viewModel.SubmitCommand.Execute(null);
			};
		}
	}
}