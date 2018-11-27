﻿using System;
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
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
namespace LiveCam.Droid
{
    public static class Extensions
    {
        public static StringContent AsJson(this object o)
         => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
    }

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
            _btnLogin.Click += _btnLogin_Click;
        }

        private void _btnLogin_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_txtUser.Text) || String.IsNullOrEmpty(_txtPass.Text)) Toast.MakeText(this, "The fields can not be empty", ToastLength.Long).Show();
            else
            {


                //UZKOMENTUOTI, KAI BUS PADARYTA, KAD BACKENDAS ATSIUNCIA ATGAL ATSAKYMA
                //if (_txtUser.Text == "admin" && _txtPass.Text == "admin")
                    //LoginSuccesful();






                Task.Run(async () =>
                {
                    string pass = Sha256(_txtPass.Text);
                    var client = new HttpClient();
                    client.BaseAddress = new Uri("http://88.119.27.98:55555");
                    var data = new { identifier = _txtUser.Text, password = pass };
                    var result = await client.PostAsync("api/login", data.AsJson()).Result.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
                    if(result==null) Toast.MakeText(this, "Password or username is incorrect", ToastLength.Long).Show();
                    else
                    {
                        LoginSuccesful(result);
                    }
                });
            }
        }

        private void LoginSuccesful(string result)
        {
            Intent nextActivity = new Intent(this, typeof(MainActivity));
            nextActivity.PutExtra("Person", result);
            StartActivity(nextActivity);
        }

        static string Sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}