using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using BigBrother.Views;
using Xamarin.Forms;

namespace BigBrother.ViewModels
{
    class LoginViewModel : INotifyPropertyChanged
    {
	    public event PropertyChangedEventHandler PropertyChanged = delegate { };
	    public Action InvalidCredentialsPopup;

	    public ICommand SubmitCommand { get; set; }

	    private INavigation navigation;
	    private string email;
	    public string Email
	    {
		    get { return email; }
		    set
		    {
			    email = value;
				PropertyChanged(this, new PropertyChangedEventArgs("Email"));
		    }
		}

	    private string pass;
	    public string Password
	    {
		    get { return pass; }
		    set
		    {
			    pass = value;
			    PropertyChanged(this, new PropertyChangedEventArgs("Password"));
		    }
	    }

	    public LoginViewModel(INavigation navigation)
	    {
		    this.navigation = navigation;
			SubmitCommand = new Command(OnSubmit);
	    }

	    private async void OnSubmit()
	    {
		    if ("wrong".Equals(email) && "wrong".Equals(pass))	//TODO: actual login
		    {
			    InvalidCredentialsPopup();
		    }
		    else
		    {
			    await navigation.PopModalAsync(false);
		    }
	    }
	}
}
