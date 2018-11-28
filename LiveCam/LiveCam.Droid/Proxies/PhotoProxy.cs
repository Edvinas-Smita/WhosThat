using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LiveCam.Droid.Proxies
{
    public static class PhotoProxy
    {
        public static Bitmap LastPhoto { get; set; }
    }
}