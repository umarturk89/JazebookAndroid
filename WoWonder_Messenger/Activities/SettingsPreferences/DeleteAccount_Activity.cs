using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;

using WoWonder.Functions;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Page;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.SettingsPreferences
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class DeleteAccount_Activity : AppCompatActivity
    {
        private AdView mAdView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                SetContentView(Resource.Layout.Settings_DeleteAccount_layout);

                //Set ToolBar
                var ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_DeleteAccount);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                //Get values
                Txt_Password = FindViewById<EditText>(Resource.Id.Password_Edit);
                Chk_Delete = FindViewById<CheckBox>(Resource.Id.DeleteCheckBox);
                Btn_Delete = FindViewById<Button>(Resource.Id.DeleteButton);

                Chk_Delete.Text = GetText(Resource.String.Lbl_IWantToDelete1) + " " + UserDetails.Username + " " +
                                  GetText(Resource.String.Lbl_IWantToDelete2) + " " + AppSettings.Application_Name + " " +
                                  GetText(Resource.String.Lbl_IWantToDelete3);

                mAdView = FindViewById<AdView>(Resource.Id.adView);
                if (AppSettings.Show_ADMOB_Banner)
                { 
                    mAdView.AdSize = AdSize.LargeBanner;
                    mAdView.Visibility = ViewStates.Visible;
                    string android_id = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                    var adRequest = new AdRequest.Builder();
                    adRequest.AddTestDevice(android_id); 
                    mAdView.LoadAd(adRequest.Build());
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Gone;
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                Btn_Delete.Click += BtnDeleteOnClick;

                mAdView?.Resume();
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
                mAdView?.Pause();

                //Close Event
                Btn_Delete.Click -= BtnDeleteOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Delete
        private async void BtnDeleteOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Chk_Delete.Checked)
                {
                    if (!IMethods.CheckConnectivity())
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Short).Show();
                    }
                    else
                    {
                        var localdata = Classes.DataUserLoginList.FirstOrDefault(a => a.UserID == UserDetails.User_id);
                        if (localdata != null)
                        {
                            if (Txt_Password.Text == localdata.Password)
                            {
                                var (apiStatus, respond) = await Global.Delete_User(Txt_Password.Text);
                                if (apiStatus == 200)
                                {
                                    if (respond is MessageObject result)
                                    {
                                        API_Request.Delete(this);

                                        var dbDatabase = new SqLiteDatabase();
                                        var ss = await dbDatabase.CheckTablesStatus();
                                        dbDatabase.Dispose();

                                        IMethods.DialogPopup.InvokeAndShowDialog(this,
                                            GetText(Resource.String.Lbl_Deleted),
                                            GetText(Resource.String.Lbl_Your_account_was_successfully_deleted),
                                            GetText(Resource.String.Lbl_Ok));
                                    }
                                }
                                else if (apiStatus == 400)
                                {
                                    if (respond is ErrorObject error)
                                    {
                                        var errortext = error._errors.Error_text;
                                       

                                        if (errortext.Contains("Invalid or expired access_token"))
                                            API_Request.Logout(this);
                                    }
                                }
                                else if (apiStatus == 404)
                                {
                                    var error = respond.ToString();
                                 
                                }
                            }
                            else
                            {
                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning),
                                    GetText(Resource.String.Lbl_Please_confirm_your_password),
                                    GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                }
                else
                {
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Warning),
                        GetText(Resource.String.Lbl_Error_Terms),
                        GetText(Resource.String.Lbl_Ok));
                }
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
                mAdView?.Destroy();
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Variables Basic

        private static EditText Txt_Password;
        private CheckBox Chk_Delete;
        private Button Btn_Delete;

        #endregion
    }
}