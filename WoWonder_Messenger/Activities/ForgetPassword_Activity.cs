using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using WoWonder.Functions;
using WoWonder_API.Classes.Global;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Activities
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleInstance)]
    public class ForgetPassword_Activity : AppCompatActivity
    {
        #region Variables Basic

        private Button Btn_Send;
        private EditText EmailEditext;
        private LinearLayout Main_LinearLayout;

        Android.Support.V7.Widget.Toolbar TopChatToolBar;
        private ProgressBar progressBar;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "ForgetPassword_Layout" layout resource
                SetContentView(Resource.Layout.ForgetPassword_Layout);

                EmailEditext = FindViewById<EditText>(Resource.Id.emailfield);
                Btn_Send = FindViewById<Button>(Resource.Id.send);
                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                progressBar.Visibility = ViewStates.Invisible;
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

                //Add Event
                Btn_Send.Click += BtnSendOnClick;
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

                //Close Event
                Btn_Send.Click -= BtnSendOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void BtnSendOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmailEditext.Text))
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var check = IMethods.Fun_String.IsEmailValid(EmailEditext.Text);
                        if (!check)
                        {
                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                GetText(Resource.String.Lbl_VerificationFailed),
                                GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            progressBar.Visibility = ViewStates.Visible;
                            var (Api_status, Respond) =
                                await Global.Get_Reset_Password_Email(EmailEditext.Text);
                            if (Api_status == 200)
                            {
                                if (Respond is ApiStatusObject result)
                                {
                                    progressBar.Visibility = ViewStates.Invisible;
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_CheckYourEmail),
                                        GetText(Resource.String.Lbl_WeSentEmailTo).Replace("@", EmailEditext.Text),
                                        GetText(Resource.String.Lbl_Ok));
                                }
                            }
                            else if (Api_status == 400)
                            {
                                var error = Respond as ErrorObject;
                                if (error != null)
                                {
                                    var errortext = error._errors.Error_text;
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security), errortext,
                                        GetText(Resource.String.Lbl_Ok));

                                    if (errortext.Contains("Invalid or expired access_token"))
                                        API_Request.Logout(this);
                                }

                                progressBar.Visibility = ViewStates.Invisible;
                            }
                            else if (Api_status == 404)
                            {
                            
                                progressBar.Visibility = ViewStates.Invisible;
                                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                    GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                    else
                    {
                        progressBar.Visibility = ViewStates.Invisible;
                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed),
                            GetText(Resource.String.Lbl_Something_went_wrong), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                progressBar.Visibility = ViewStates.Invisible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed),
                    exception.ToString(), GetText(Resource.String.Lbl_Ok));
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