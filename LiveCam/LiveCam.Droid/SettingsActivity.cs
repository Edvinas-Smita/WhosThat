using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LiveCam.Droid
{
    /// <summary>
	/// Parent setting activity, all ti does is load up the headers
	/// </summary>
	[Activity(Label = "@string/menu_settings", Icon = "@drawable/ic_launcher", Theme = "@style/ThemeActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : PreferenceActivity, ISharedPreferencesOnSharedPreferenceChangeListener
    {

        private int leftToClick = 5;
        private Toast toast;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //this.AddPreferencesFromResource(Resource.preferences_general);
            //var reset = this.FindPreference("reset_high_score");
            //reset.PreferenceClick += (sender, e) => {
            //    if (toast != null && leftToClick > 0)
            //        toast.Cancel();

            //    leftToClick--;
                if (leftToClick == 0)
                {
                    //SyncStateContract.Helpers.Settings.HighScore = SyncStateContract.Helpers.Settings.HighScoreDefault;
                    //SyncStateContract.Helpers.Settings.HighScoreDay = DateTime.Today;
                    //toast = Toast.MakeText(this, Resource.String.has_been_reset, ToastLength.Long);
                    //toast.Show();
                }
                else if (leftToClick > 0)
                {
                    //var text = Resources.GetString(Resource.String.reset_count);
                    //toast = Toast.MakeText(this, string.Format(text, leftToClick), ToastLength.Short);
                    //toast.Show();
                }
           // };
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            //switch (key)
            //{
            //    case SyncStateContract.Helpers.Settings.WeightKey:
            //        this.SetWeight();
            //        break;
            //    case SyncStateContract.Helpers.Settings.CadenceKey:
            //        this.SetCadence();
            //        break;
            //}
        }

        private void SetCadence()
        {
            //var pref = (ListPreference)this.FindPreference(SyncStateContract.Helpers.Settings.CadenceKey);
            ////pref.SetEntries (Helpers.Settings.UseKilometeres ? Resource.Array.cadence_names_km : Resource.Array.cadence_names);
            //switch (SyncStateContract.Helpers.Settings.Cadence)
            //{
            //    case "0":
            //        pref.SetSummary(SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.cadence0_km : Resource.String.cadence0);
            //        break;
            //    case "1":
            //        pref.SetSummary(SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.cadence1_km : Resource.String.cadence1);
            //        break;
            //    case "2":
            //        pref.SetSummary(SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.cadence2_km : Resource.String.cadence2);
            //        break;
            //    case "3":
            //        pref.SetSummary(SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.cadence3_km : Resource.String.cadence3);
            //        break;
            //    case "4":
            //        pref.SetSummary(SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.cadence4_km : Resource.String.cadence4);
            //        break;
            //    case "5":
            //        pref.SetSummary(SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.cadence5_km : Resource.String.cadence5);
            //        break;
            //    case "6":
            //        pref.SetSummary(SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.cadence6_km : Resource.String.cadence6);
            //        break;
            //}
        }

        private void SetWeight()
        {
            //var pref = (EditTextPreference)this.FindPreference(SyncStateContract.Helpers.Settings.WeightKey);
            //var format = SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.weight_kg : Resource.String.weight_lbs;
            //var title = SyncStateContract.Helpers.Settings.UseKilometeres ? Resource.String.weight_title_kg : Resource.String.weight_title_lbs;
            //var weight = SyncStateContract.Helpers.Settings.Weight;
            ////check for accuracy
            //if (weight > 1000)
            //    weight = 1000;
            //else if (weight < 0)
            //    weight = 0;

            //if (weight != SyncStateContract.Helpers.Settings.Weight)
            //    SyncStateContract.Helpers.Settings.Weight = weight;
            //pref.Summary = string.Format(Resources.GetString(format), weight);
            //pref.SetTitle(title);
            //pref.SetDialogTitle(title);
        }

        protected override void OnStart()
        {
            base.OnStart();
            this.SetWeight();
            this.SetCadence();

        }

        protected override void OnPause()
        {
            base.OnPause();
            this.PreferenceManager.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }
    }
}