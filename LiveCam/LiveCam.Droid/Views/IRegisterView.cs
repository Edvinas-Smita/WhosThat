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

namespace LiveCam.Droid.Views
{
    interface IRegisterView
    {
        string UserNameText { get; set; }
        string PasswordText { get; set; }
        string NameText { get; set; }
        string LastNameText { get; set; }
        void RegisterSuccesful(string result);
        void RegisterUnsuccessful();
    }
}