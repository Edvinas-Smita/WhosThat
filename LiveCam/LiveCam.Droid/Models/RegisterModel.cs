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
using Encoding = LiveCam.Droid.Helpers.Encoding;

namespace LiveCam.Droid.Models
{
    class RegisterModel : IRegisterModel
    {
        public async Task<string> TryToRegister(string address, string username, string passwd, string name, string lastname)
        {
            string pass = Encoding.Sha256(passwd);
            var client = new HttpClient();
            client.BaseAddress = new Uri(address);
            var data = new { identifier = username, password = pass, firstName = name, lastName = lastname };
            var result = await client.PostAsync("api/register", data.AsJson()).Result.Content.ReadAsStringAsync();
            Console.WriteLine(result);

            return result;
        }
    }
}