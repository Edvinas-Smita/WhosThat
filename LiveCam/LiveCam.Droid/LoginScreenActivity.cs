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

namespace LiveCam.Droid
{
    [Activity(Label = "LoginScreenActivity", MainLauncher = true)]
    public class LoginScreenActivity : Activity
    {
        private Button _btnLogin;
        private TextView _txtUser;
        private TextView _txtPass;
        private bool loggedOn = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LoginScreen);

            _btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            _txtPass = FindViewById<TextView>(Resource.Id.txtPass);
            _txtUser = FindViewById<TextView>(Resource.Id.txtUser);
        }
    }
}