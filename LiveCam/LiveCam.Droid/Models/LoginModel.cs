using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LiveCam.Droid.Helpers;
using Newtonsoft.Json;
using Encoding = LiveCam.Droid.Helpers.Encoding;

namespace LiveCam.Droid.Models
{
    public static class Extensions
    {
        public static StringContent AsJson(this object o)
            => new StringContent(JsonConvert.SerializeObject(o), System.Text.Encoding.UTF8, "application/json");
    }

    public class LoginModel : ILoginModel
    {
        /// <summary>
        /// Try to login to the backend with provided username and pwd
        /// </summary>
        /// <param name="address">Server adress and port (http://ip-adress:port)</param>
        /// <param name="name">Username to connect with</param>
        /// <param name="pwd">Password to connect with</param>
        public async Task<string> TryToLogin(string address, string name, string pwd)
        {
            string pass = Encoding.Sha256(pwd);
            var client = new HttpClient();
            client.BaseAddress = new Uri(address);
            var data = new {identifier = name, password = pass};
            var result = await client.PostAsync("api/login", data.AsJson()).Result.Content.ReadAsStringAsync();
            Console.WriteLine(result);

            return result;
        }
    }
}