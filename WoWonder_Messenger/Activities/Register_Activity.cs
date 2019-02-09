using System;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FFImageLoading;
using WoWonder.Frameworks.onesignal;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using IMethods = WoWonder.Functions.IMethods;

namespace WoWonder.Activities
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleInstance)]
    public class Register_Activity : AppCompatActivity, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private Button RegisterButton;
        private RelativeLayout Rootview;
        private EditText EmailEditext;
        private EditText UsernameEditext;
        private EditText PasswordEditext;
        private EditText PasswordRepeatEditext;
        private LinearLayout Main_LinearLayout;

        private TextView Txt_Terms_of_service;
        private TextView Txt_Privacy;
        private CheckBox Chk_agree;

        Android.Support.V7.Widget.Toolbar TopChatToolBar;
        private ProgressBar progressBar;
        private Typeface regularTxt, semiboldTxt;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "Register_Layout" layout resource
                SetContentView(Resource.Layout.Register_Layout);

                regularTxt = Typeface.CreateFromAsset(Assets, "fonts/SF-UI-Display-Regular.ttf");
                semiboldTxt = Typeface.CreateFromAsset(Assets, "fonts/SF-UI-Display-Semibold.ttf");
                 
                EmailEditext = FindViewById<EditText>(Resource.Id.emailfield);
                UsernameEditext = FindViewById<EditText>(Resource.Id.usernamefield);
                PasswordEditext = FindViewById<EditText>(Resource.Id.passwordfield);
                PasswordRepeatEditext = FindViewById<EditText>(Resource.Id.passwordrepeatfield);
                RegisterButton = FindViewById<Button>(Resource.Id.registerButton);
                Main_LinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLinearLayout);
                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
                 
                progressBar.Visibility = ViewStates.Gone;
                RegisterButton.Visibility = ViewStates.Visible;

                Chk_agree = FindViewById<CheckBox>(Resource.Id.termCheckBox);
                Txt_Terms_of_service = FindViewById<TextView>(Resource.Id.secTermTextView);
                Txt_Privacy = FindViewById<TextView>(Resource.Id.secPrivacyTextView);

                EmailEditext.SetTypeface(regularTxt, TypefaceStyle.Normal);
                UsernameEditext.SetTypeface(regularTxt, TypefaceStyle.Normal);
                PasswordEditext.SetTypeface(regularTxt, TypefaceStyle.Normal);
                PasswordRepeatEditext.SetTypeface(regularTxt, TypefaceStyle.Normal);
                Txt_Terms_of_service.SetTypeface(regularTxt, TypefaceStyle.Normal);
                Txt_Privacy.SetTypeface(regularTxt, TypefaceStyle.Normal);
                FontController.SetFont(RegisterButton, 1);
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
                Main_LinearLayout.Click += Main_LoginPage_Click;
                RegisterButton.Click += RegisterButton_Click;
                Txt_Terms_of_service.Click += TxtTermsOfServiceOnClick;
                Txt_Privacy.Click += TxtPrivacyOnClick;

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
                Main_LinearLayout.Click -= Main_LoginPage_Click;
                RegisterButton.Click -= RegisterButton_Click;
                Txt_Terms_of_service.Click -= TxtTermsOfServiceOnClick;
                Txt_Privacy.Click -= TxtPrivacyOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TxtPrivacyOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string url = Client.WebsiteUrl + "/terms/privacy-policy";
                IMethods.IApp.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TxtTermsOfServiceOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                string url = Client.WebsiteUrl + "/terms/terms";
                IMethods.IApp.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Main_LoginPage_Click(object sender, EventArgs e)
        {
            if (Chk_agree.Checked)
            {
                InputMethodManager inputManager =
                    (InputMethodManager) this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
            }
            else
            {
                IMethods.DialogPopup.InvokeAndShowDialog(this, this.GetText(Resource.String.Lbl_Warning),
                    this.GetText(Resource.String.Lbl_Error_Terms), this.GetText(Resource.String.Lbl_Ok));
            }
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Chk_agree.Checked)
                {
                    if (IMethods.CheckConnectivity())
                    {
                        if (!String.IsNullOrEmpty(UsernameEditext.Text) ||
                            !String.IsNullOrEmpty(PasswordEditext.Text) ||
                            !String.IsNullOrEmpty(PasswordRepeatEditext.Text) ||
                            !String.IsNullOrEmpty(EmailEditext.Text))
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
                                if (PasswordRepeatEditext.Text == PasswordEditext.Text)
                                {
                                    progressBar.Visibility = ViewStates.Visible;
                                    RegisterButton.Visibility = ViewStates.Gone;

                                    var settingsResult =await Current.GetSettings();
                                    if (settingsResult.Item1 == 200)
                                    {
                                        var PushID = settingsResult.Item2.PushId.ToString();
                                        if (OneSignalNotification.OneSignalAPP_ID == "")
                                        {
                                            OneSignalNotification.OneSignalAPP_ID = PushID;
                                            if (AppSettings.ShowNotification)
                                                OneSignalNotification.RegisterNotificationDevice();
                                        }

                                        var (Api_status, Respond) =
                                            await WoWonder_API.Requests.RequestsAsync.Global.Get_Create_Account(
                                                UsernameEditext.Text,
                                                PasswordEditext.Text, PasswordRepeatEditext.Text, EmailEditext.Text);
                                        if (Api_status == 200)
                                        {
                                            if (Respond is CreatAccountObject result)
                                            {
                    
                                                Current.AccessToken = result.access_token;

                                                UserDetails.Username = UsernameEditext.Text;
                                                UserDetails.Full_name = UsernameEditext.Text;
                                                UserDetails.Password = PasswordEditext.Text;
                                                UserDetails.access_token = result.access_token;
                                                UserDetails.User_id = result.user_id;
                                                UserDetails.Status = "Active";
                                                UserDetails.Cookie = result.access_token;
                                                UserDetails.Email = EmailEditext.Text;

                                                //Insert user data to database
                                                var user = new DataTables.LoginTB
                                                {
                                                    UserID = UserDetails.User_id,
                                                    access_token = UserDetails.access_token,
                                                    Cookie = UserDetails.Cookie,
                                                    Username = UsernameEditext.Text,
                                                    Password = PasswordEditext.Text,
                                                    Status = "Active",
                                                    Lang = ""
                                                };

                                                Classes.DataUserLoginList.Add(user);

                                                var dbDatabase = new SqLiteDatabase();
                                                dbDatabase.InsertRow(user);
                                                dbDatabase.Dispose();

                                                StartActivity(new Intent(this, typeof(AppIntroWalkTroutPage)));
                                            }

                                            progressBar.Visibility = ViewStates.Gone;
                                            RegisterButton.Visibility = ViewStates.Visible;
                                            Finish();
                                        }
                                        else if (Api_status == 220)
                                        {
                                            //var dialog = new MaterialDialog.Builder(this);

                                            //dialog.Title(GetText(Resource.String.Lbl_ActivationSent));
                                            //dialog.Content(GetText(Resource.String.Lbl_ActivationDetails)
                                            //    .Replace("@", EmailEditext.Text));
                                            //dialog.PositiveText(GetText(Resource.String.Lbl_Ok)).OnPositive(this);

                                            //dialog.AlwaysCallSingleChoiceCallback();
                                            // dialog.Build().Show();
                                            var obj = new IMethods.DialogPopup(this);

                                            var x = await obj.ShowDialog("Success",
                                                    "Registration successful! We have sent you an email, Please check your inbox/spam to verify your email.");
                                            
                                            StartActivity(new Intent(this, typeof(MainActivity)));


                                        }
                                        else if (Api_status == 400)
                                        {
                                            var error = Respond as ErrorObject;
                                            if (error != null)
                                            {
                                                var errortext = error._errors.Error_text;

                                                var errorid = error._errors.Error_id;

                                                if (errorid == "3")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_3),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "4")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_4),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "5")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_Something_went_wrong),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "6")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_6),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "7")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_7),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "8")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_8),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "9")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_9),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "10")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_10),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else if (errorid == "11")
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security),
                                                        GetText(Resource.String.Lbl_ErrorRegister_11),
                                                        GetText(Resource.String.Lbl_Ok));
                                                else
                                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                        GetText(Resource.String.Lbl_Security), errortext,
                                                        GetText(Resource.String.Lbl_Ok));
                                            }

                                            progressBar.Visibility = ViewStates.Gone;
                                            RegisterButton.Visibility = ViewStates.Visible;
                                        }
                                        else if (Api_status == 404)
                                        {
                                          
                                            progressBar.Visibility = ViewStates.Gone;
                                            RegisterButton.Visibility = ViewStates.Visible;
                                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                GetText(Resource.String.Lbl_Security),
                                                GetText(Resource.String.Lbl_Error_Login),
                                                GetText(Resource.String.Lbl_Ok));
                                        }
                                    }
                                }
                                else
                                {
                                    progressBar.Visibility = ViewStates.Gone;
                                    RegisterButton.Visibility = ViewStates.Visible;

                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security),
                                        GetText(Resource.String.Lbl_Error_Register_password),
                                        GetText(Resource.String.Lbl_Ok));
                                }
                            }
                        }
                        else
                        {
                            progressBar.Visibility = ViewStates.Gone;
                            RegisterButton.Visibility = ViewStates.Visible;
                            IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        RegisterButton.Visibility = ViewStates.Visible;
                       
                        IMethods.DialogPopup.InvokeAndShowDialog(this, this.GetText(Resource.String.Lbl_Security),
                            this.GetText(Resource.String.Lbl_Error_check_internet_connection),
                            this.GetText(Resource.String.Lbl_Ok));
                        return;
                    }
                }
                else
                {
                    IMethods.DialogPopup.InvokeAndShowDialog(this, this.GetText(Resource.String.Lbl_Warning),
                        this.GetText(Resource.String.Lbl_Error_Terms), this.GetText(Resource.String.Lbl_Ok));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                progressBar.Visibility = ViewStates.Gone;
                RegisterButton.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, this.GetText(Resource.String.Lbl_Authentication_failed),
                    ex.Message, this.GetText(Resource.String.Lbl_Ok));
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

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            if (p1 == DialogAction.Positive)
            {
               Finish();
            }
            else if (p1 == DialogAction.Negative)
            {
                p0.Dismiss();
            }
        }
    }
}