﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LiveCam.Droid.Models
{
    public interface IRegisterModel
    {
        Task<String> TryToRegister(string address, string username, string passwd, string name, string lastname);
    }
}