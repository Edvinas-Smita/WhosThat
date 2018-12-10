using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LiveCam.Droid.Models;
using LiveCam.Droid.Views;

namespace LiveCam.Droid.Presenters
{
    public class LoginPresenter
    {
        private ILoginView _loginView;
        private ILoginModel _loginModel;

        /// <summary>
        /// Binds UI with data and performs main actions
        /// </summary>
        /// <param name="loginView">View to be associated with</param>
        /// <param name="loginModel">Model to be associated with</param>
        public LoginPresenter(ILoginView loginView, ILoginModel loginModel)
        {
            _loginView = loginView;
            _loginModel = loginModel;
        }

        /// <summary>
        /// Interacts with model to process login
        /// </summary>
        public async void Login()
        {
            var result = await _loginModel.TryToLogin(
                "http://88.119.27.98:55555", 
                _loginView.LoginText,
                _loginView.PasswordText);
            if (result == null)
            {
                _loginView.LoginUnsuccessful();
            }
            else
            {
                _loginView.LoginSuccesful(result);
            }
        }
    }
}