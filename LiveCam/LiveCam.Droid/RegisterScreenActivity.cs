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
using LiveCam.Droid.Presenters;
using LiveCam.Droid.Views;

namespace LiveCam.Droid
{
    [Activity(Label = "RegisterScreenActivity")]
    public class RegisterScreenActivity : Activity, IRegisterView
    {
        private Button _btnRegister;
        private TextView _txtUser;
        private TextView _txtPass;
        private TextView _txtName;
        private TextView _txtLastName;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.RegisterScreen);

            _btnRegister = FindViewById<Button>(Resource.Id.register);
            _txtUser = FindViewById<TextView>(Resource.Id.txtUserNameRegister);
            _txtPass = FindViewById<TextView>(Resource.Id.txtPasswordRegister);
            _txtName = FindViewById<TextView>(Resource.Id.txtNameRegister);
            _txtLastName = FindViewById<TextView>(Resource.Id.txtLastNameRegister);

            _btnRegister.Click += _btnRegister_Click;
        }

        private void _btnRegister_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(_txtLastName.Text) || String.IsNullOrWhiteSpace(_txtName.Text) || String.IsNullOrWhiteSpace(_txtPass.Text) || String.IsNullOrWhiteSpace(_txtUser.Text))
            {
                Toast.MakeText(this, "The fields can not be empty", ToastLength.Long).Show();
            }

            else
            {
                RegisterPresenter presenter = new RegisterPresenter(this, new RegisterModel());
                presenter.Register();
            }
        }


        public string UserNameText
        {
            get => _txtUser.Text;
            set => _txtUser.Text = value;
        }

        public string PasswordText
        {
            get => _txtPass.Text;
            set => _txtPass.Text = value;
        }
        public string NameText
        {
            get => _txtName.Text;
            set => _txtName.Text = value;
        }

        public string LastNameText
        {
            get => _txtLastName.Text;
            set => _txtLastName.Text = value;
        }

        public void RegisterSuccesful(string result)
        {
            Toast.MakeText(this, "Registration successful", ToastLength.Long);
            Intent nextActivity = new Intent(this, typeof(LoginScreenActivity));
            StartActivity(nextActivity);
        }

        public void RegisterUnsuccessful()
        {
            Toast.MakeText(this, "Inputs are wrong or the username is taken", ToastLength.Long).Show();
        }
    }
}