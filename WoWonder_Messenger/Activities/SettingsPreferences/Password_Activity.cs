using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using FFImageLoading;
using WoWonder.Functions;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Activities.SettingsPreferences
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Password_Activity : AppCompatActivity
    {

        #region Variables Basic

        private static EditText Txt_CurrentPassword;
        private static EditText Txt_NewPassword;
        private static EditText Txt_RepeatPassword;

        private TextView Txt_linkForget;
        private TextView Txt_Save;

        private AdView mAdView;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "Settings_Password_Layout" layout resource
                SetContentView(Resource.Layout.Settings_Password_Layout);

                //Set ToolBar
                var ToolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                ToolBar.Title = this.GetText(Resource.String.Lbl_Change_Password);

                SetSupportActionBar(ToolBar);
                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                //Get values
                Txt_CurrentPassword = FindViewById<EditText>(Resource.Id.CurrentPassword_Edit);
                Txt_NewPassword = FindViewById<EditText>(Resource.Id.NewPassword_Edit);
                Txt_RepeatPassword = FindViewById<EditText>(Resource.Id.RepeatPassword_Edit);

                Txt_linkForget = FindViewById<TextView>(Resource.Id.linkText);
                Txt_Save = FindViewById<TextView>(Resource.Id.toolbar_title);

                //Show Ads
                mAdView = FindViewById<AdView>(Resource.Id.adView);
                if (AppSettings.Show_ADMOB_Banner)
                {
                    mAdView.Visibility = ViewStates.Visible;
                    string android_id = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                    var adRequest = new AdRequest.Builder();
                    adRequest.AddTestDevice(android_id);
                    mAdView.LoadAd(adRequest.Build());
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Invisible;
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                mAdView?.Resume();
                //Event
                Txt_linkForget.Click += TxtLinkForget_OnClick;
                Txt_Save.Click += SaveDataButtonOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Event
                Txt_linkForget.Click -= TxtLinkForget_OnClick;
                Txt_Save.Click -= SaveDataButtonOnClick;
                mAdView?.Pause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void SaveDataButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Txt_CurrentPassword.Text == "" || Txt_NewPassword.Text == "" || Txt_RepeatPassword.Text == "")
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Please_check_your_details), ToastLength.Long)
                        .Show();
                    return;
                }

                if (Txt_NewPassword.Text != Txt_RepeatPassword.Text)
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Your_password_dont_match), ToastLength.Long)
                        .Show();
                }
                else
                {
                    if (IMethods.CheckConnectivity())
                    {
                        //Show a progress
                        AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                        if (Txt_CurrentPassword.Text != null && Txt_NewPassword.Text != null &&
                            Txt_RepeatPassword.Text != null)
                        {
                            var dataPrivacy = new Dictionary<string, string>
                            {
                                {"new_password", Txt_NewPassword.Text},
                                {"current_password", Txt_CurrentPassword.Text}
                            };

                            var (Api_status, Respond) =
                                await Global.Update_User_Data(dataPrivacy);
                            if (Api_status == 200)
                            {
                                if (Respond is MessageObject result)
                                {
                                    if (result.Message.Contains("updated"))
                                    {
                                        Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                                        AndHUD.Shared.Dismiss(this);
                                    }
                                    else
                                    {
                                        //Show a Error image with a message
                                        AndHUD.Shared.ShowError(this, result.Message, MaskType.Clear,
                                            TimeSpan.FromSeconds(2));
                                    }
                                }
                            }
                            else if (Api_status == 400)
                            {
                                if (Respond is ErrorObject error)
                                {
                                    var errortext = error._errors.Error_text;
                                   

                                    if (errortext.Contains("Invalid or expired access_token"))
                                        API_Request.Logout(this);
                                }
                            }
                            else if (Api_status == 404)
                            {
                                var error = Respond.ToString();
                              
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details),
                                ToastLength.Long).Show();
                        }

                        AndHUD.Shared.Dismiss(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        private void TxtLinkForget_OnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(ForgetPassword_Activity));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
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
                mAdView?.Destroy();
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