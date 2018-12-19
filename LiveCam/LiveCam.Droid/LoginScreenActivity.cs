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
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using LiveCam.Droid.Models;
using LiveCam.Droid.Presenters;
using LiveCam.Droid.Views;

namespace LiveCam.Droid
{
    [Activity(Label = "LoginScreenActivity", MainLauncher = true)]
    public class LoginScreenActivity : Activity, ILoginView
    {
        private static readonly string TAG = "FaceTracker";

        private Button _btnLogin;
        private TextView _txtUser;
        private TextView _txtPass;
        private TextView _txtPasswordWarning;
        private bool loggedOn = false;
        private GraphicOverlay _mGraphicOverlay;

        private const int RC_HANDLE_LOCATION_PERM = 3;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LoginScreen);

            _btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            _txtPass = FindViewById<TextView>(Resource.Id.txtPass);
            _txtUser = FindViewById<TextView>(Resource.Id.txtUser);
            _txtPasswordWarning = FindViewById<TextView>(Resource.Id.txtPasswordWarning);
            _btnLogin.Click += _btnLogin_Click;


            
        }

        private void RequestGpsPermission()
        {
            Log.Warn(TAG, "GPS permission is not granted. Requesting permission");

            var permissions = new string[] { Manifest.Permission.AccessCoarseLocation };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this,
                Manifest.Permission.AccessCoarseLocation))
            {
                ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_LOCATION_PERM);
                return;
            }

             ActivityCompat.RequestPermissions(this, permissions, RC_HANDLE_LOCATION_PERM); 

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Log.Debug(TAG, $"OnRequestPermissionsResult invoked, requestCode: {requestCode}");
            Android.Support.V7.App.AlertDialog.Builder builder;
            switch (requestCode)
            {
                case RC_HANDLE_LOCATION_PERM:
                    if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
                    {
                        Log.Debug(TAG, "Location permission granted");
                        return;
                    }

                    Log.Error(TAG, "Location permission not granted: results len = " + grantResults.Length +
                                   " Result code = " +
                                   (grantResults.Length > 0 ? grantResults[0].ToString() : "(empty)"));


                    builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    builder.SetTitle("LiveCam")
                        .SetMessage("You have not granted location permission. ")
                        .SetPositiveButton(Resource.String.ok, (o, e) => Finish())
                        .Show();
                    break;


                default:
                    Log.Debug(TAG, "Got unexpected permission result: " + requestCode);
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                    return;
                    break;
            }



        }

        private void _btnLogin_Click(object sender, EventArgs e)
        {
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) ==
                Permission.Granted)
            {

                Log.Debug(TAG, "Location permission is granted");
                Toast.MakeText(this, "Location permission granted", ToastLength.Long);
                if (String.IsNullOrEmpty(_txtUser.Text) || String.IsNullOrEmpty(_txtPass.Text))
                {
                    Toast.MakeText(this, "The fields can not be empty", ToastLength.Long).Show();
                }
                else
                {
                    //UZKOMENTUOTI, KAI BUS PADARYTA, KAD BACKENDAS ATSIUNCIA ATGAL ATSAKYMA
                    //if (_txtUser.Text == "admin" && _txtPass.Text == "admin")
                    //LoginSuccesful("test");

                    LoginPresenter presenter = new LoginPresenter(this, new LoginModel());
                    presenter.Login();
                }
            }
            else
            {
                Log.Debug(TAG, "Requesting location perm");
                RequestGpsPermission();
            }

        }

        public void LoginSuccesful(string result)
        {
            

            Intent nextActivity = new Intent(this, typeof(MainActivity));
            nextActivity.PutExtra("Person", result);
            StartActivity(nextActivity);
        }

        public void LoginUnsuccessful()
        {
            Toast.MakeText(this, "Password or username is incorrect", ToastLength.Long).Show();
        }

        public string LoginText
        {
            get => _txtUser.Text;
            set => _txtUser.Text = value;
        }

        public string PasswordText
        {
            get => _txtPass.Text;
            set => _txtPass.Text = value;

        }
    }
}