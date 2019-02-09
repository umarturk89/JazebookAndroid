using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using FFImageLoading;
using WoWonder.Functions;
using WoWonder.Helpers;

namespace WoWonder.Activities.SettingsPreferences {

    [Activity (Theme = "@style/SettingsTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Settings_Activity : AppCompatActivity {
        protected override void OnCreate (Bundle savedInstanceState) {
            try {
                base.OnCreate (savedInstanceState);

                IMethods.IApp.FullScreenApp (this);

                SetContentView (Resource.Layout.Settings_Layout);

                var ToolBar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.settingstoolbar);
                ToolBar.Title = this.GetText(Resource.String.Lbl_Settings);

                SetSupportActionBar (ToolBar);
                SupportActionBar.SetDisplayHomeAsUpEnabled (true);
                SupportActionBar.SetDisplayShowHomeEnabled (true);

                var ff = FragmentManager.BeginTransaction ().Replace (Resource.Id.content_frame, new SettingsPrefsFragment (this)).Commit ();
               


                AdsGoogle.Ad_Interstitial(this);

            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }

        public override bool OnOptionsItemSelected (IMenuItem item) {
            switch (item.ItemId) {
                case Android.Resource.Id.Home:
                    Finish ();
                    return true;
            }
            return base.OnOptionsItemSelected (item);
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}