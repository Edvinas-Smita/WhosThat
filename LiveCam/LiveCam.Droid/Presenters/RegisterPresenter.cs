using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class RegisterPresenter
    {
        private IRegisterView _registerView;
        private IRegisterModel _registerModel;

        public RegisterPresenter(IRegisterView registerView, IRegisterModel registerModel)
        {
            _registerView = registerView;
            _registerModel = registerModel;
        }

        public async void Register()
        {
            var result = await _registerModel.TryToRegister(
                "http://88.119.27.98:55555",
                _registerView.UserNameText,
                _registerView.PasswordText,
                _registerView.NameText,
                _registerView.LastNameText);
            if (result == null)
            {
                _registerView.RegisterUnsuccessful();
            }
            else
            {
                _registerView.RegisterSuccesful(result);
            }
        }
    }
}