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
    public interface ILoginView
    {
        string LoginText { get; set; }
        string PasswordText { get; set; }
        void LoginSuccesful(string result);
        void LoginUnsuccessful();

    }
}