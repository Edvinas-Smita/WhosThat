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
    [Activity(Label = "RegisterScreenActivity")]
    public class RegisterScreenActivity : Activity
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

            }
        }
    }
}