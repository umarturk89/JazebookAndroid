using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using FFImageLoading;
using WoWonder.Functions;

namespace WoWonder.Activities.SettingsPreferences {
    [Activity (Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Help_Activity : AppCompatActivity {
        public static SwipeRefreshLayout swipeRefreshLayout;
        public static WebView localWebView;
        private string Url = "";

        protected override void OnCreate (Bundle savedInstanceState) {
            try {
                base.OnCreate (savedInstanceState);

                IMethods.IApp.FullScreenApp (this);

                // Create your application here

                // Set our view from the "Settings_Help_Layout" layout resource
                SetContentView (Resource.Layout.Settings_Help_Layout);

                var data = Intent.GetStringExtra ("URL") ?? "Data not available";
                if (data != "Data not available" && !String.IsNullOrEmpty (data)) {
                    Url = data;
                }

                var ToolBar = FindViewById<Toolbar> (Resource.Id.Searchtoolbar);
                ToolBar.Title = this.GetText(Resource.String.Lbl_Help);

                SetSupportActionBar (ToolBar);

                SupportActionBar.SetDisplayShowCustomEnabled (true);
                SupportActionBar.SetDisplayHomeAsUpEnabled (true);
                SupportActionBar.SetHomeButtonEnabled (true);
                SupportActionBar.SetDisplayShowHomeEnabled (true);

                localWebView = FindViewById<WebView> (Resource.Id.LocalWebView);
                swipeRefreshLayout = (SwipeRefreshLayout) FindViewById (Resource.Id.swipeRefreshLayout);

                swipeRefreshLayout.SetColorSchemeResources (Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);

                //Set WebView
                localWebView.SetWebViewClient (new MyWebViewClient (this));

                //Load url to be randered on WebView
                localWebView.LoadUrl (Url);
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


        public class MyWebViewClient : WebViewClient {
            public Activity mActivity;
            public MyWebViewClient (Activity mActivity) {
                this.mActivity = mActivity;
            }

            public override bool ShouldOverrideUrlLoading (WebView view, string url) {
                view.LoadUrl ("javascript:$('.header-container').hide();");
                view.LoadUrl ("javascript:$('.footer-wrapper').hide();");
                view.LoadUrl ("javascript:$('.content-container').css('margin-top', '0');");
                view.LoadUrl ("javascript:$('.wo_about_wrapper_parent').css('top', '0');");

                view.LoadUrl (url);
                return true;
            }

            public override void OnPageStarted (WebView view, string url, Android.Graphics.Bitmap favicon) {
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                view.Settings.JavaScriptEnabled = true;
                view.LoadUrl ("javascript:$('.header-container').hide();");
                view.LoadUrl ("javascript:$('.footer-wrapper').hide();");
                view.LoadUrl ("javascript:$('.content-container').css('margin-top', '0');");
                view.LoadUrl ("javascript:$('.wo_about_wrapper_parent').css('top', '0');");

                base.OnPageStarted (view, url, favicon);
            }

            public override void OnPageFinished (WebView view, string url) {
                view.LoadUrl ("javascript:$('.header-container').hide();");
                view.LoadUrl ("javascript:$('.footer-wrapper').hide();");
                view.LoadUrl ("javascript:$('.content-container').css('margin-top', '0');");
                view.LoadUrl ("javascript:$('.wo_about_wrapper_parent').css('top', '0');");

                Help_Activity.swipeRefreshLayout.Refreshing = false;
                Help_Activity.swipeRefreshLayout.Enabled = false;

                base.OnPageFinished (view, url);
            }

            public override void OnReceivedError (WebView view, ClientError errorCode, string description, string failingUrl) {
                Help_Activity.swipeRefreshLayout.Refreshing = false;
                Help_Activity.swipeRefreshLayout.Enabled = false;

                base.OnReceivedError (view, errorCode, description, failingUrl);
            }
        }
    }
}