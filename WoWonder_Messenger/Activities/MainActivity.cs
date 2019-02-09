using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FFImageLoading;
using Newtonsoft.Json;
using Org.Json;
using WoWonder.Activities.Tab;
using WoWonder.Frameworks.onesignal;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.Helpers.SocialLogins;
using WoWonder.SQLite;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Object = Java.Lang.Object;
using Console = System.Console;
using IMethods = WoWonder.Functions.IMethods;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Activities
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : AppCompatActivity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, IResultCallback
    {
        #region Variables Basic

        private Button Btn_Login;
        private Button RegisterButton;
        private EditText UsernameEditext;
        private EditText PasswordEditext;
        private LinearLayout Main_LinearLayout;
        private TextView topTitile;
        private TextView subTitile;
        private TextView Txt_forgetpass;
        private TextView Txt_Terms_of_service;
        private TextView Txt_Privacy;

        private ProgressBar progressBar;

        private LoginButton BtnFBLogin;
        private ICallbackManager mFBCallManager;
        private FB_MyProfileTracker mprofileTracker;

        public static GoogleApiClient mGoogleApiClient;
        //private SignInButton mGoogleSignIn;
        private Button mGoogleSignIn;
        private AppSettings st = new AppSettings();
        private Typeface regularTxt, semiboldTxt;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.Main);

                regularTxt = Typeface.CreateFromAsset(Assets, "fonts/SF-UI-Display-Regular.ttf");
                semiboldTxt = Typeface.CreateFromAsset(Assets, "fonts/SF-UI-Display-Semibold.ttf");
                 
                //Get values
                UsernameEditext = FindViewById<EditText>(Resource.Id.usernamefield);
                PasswordEditext = FindViewById<EditText>(Resource.Id.passwordfield);
                progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                Btn_Login = FindViewById<Button>(Resource.Id.loginButton);

                RegisterButton = FindViewById<Button>(Resource.Id.signUpButton);

                Main_LinearLayout = FindViewById<LinearLayout>(Resource.Id.mainLinearLayout);

                topTitile = FindViewById<TextView>(Resource.Id.titile);
                topTitile.Text = AppSettings.Application_Name;

                subTitile = FindViewById<TextView>(Resource.Id.subtitile);
                subTitile.Text = this.GetText(Resource.String.Lbl_Subtitile_Login);

                Txt_forgetpass = FindViewById<TextView>(Resource.Id.forgetpassButton);

                Txt_Terms_of_service = FindViewById<TextView>(Resource.Id.secTermTextView);
                Txt_Privacy = FindViewById<TextView>(Resource.Id.secPrivacyTextView);

                progressBar.Visibility = ViewStates.Invisible;

                UsernameEditext.SetTypeface(regularTxt, TypefaceStyle.Normal);
                PasswordEditext.SetTypeface(regularTxt, TypefaceStyle.Normal);
                topTitile.SetTypeface(regularTxt, TypefaceStyle.Normal);
                subTitile.SetTypeface(regularTxt, TypefaceStyle.Normal);
                Txt_forgetpass.SetTypeface(regularTxt, TypefaceStyle.Normal);
                Txt_Terms_of_service.SetTypeface(regularTxt, TypefaceStyle.Normal);
                Txt_Privacy.SetTypeface(regularTxt, TypefaceStyle.Normal);
                FontController.SetFont(Btn_Login, 1);
                FontController.SetFont(RegisterButton, 1);

                //Social Logins >>
                //==============================
                FacebookSdk.SdkInitialize(this);

                mprofileTracker = new FB_MyProfileTracker();
                mprofileTracker.StartTracking();

                BtnFBLogin = FindViewById<LoginButton>(Resource.Id.fblogin_button);
                BtnFBLogin.SetReadPermissions(new List<string>
                {
                    "email",
                    "public_profile"
                });
                mFBCallManager = CallbackManagerFactory.Create();
                BtnFBLogin.RegisterCallback(mFBCallManager, this);
               
                //FB accessToken
                var accessToken = AccessToken.CurrentAccessToken;
                var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                if (isLoggedIn && Profile.CurrentProfile != null)
                {
                    LoginManager.Instance.LogOut();
                }

                // Configure sign-in to request the user's ID, email address, and basic profile. ID and basic profile are included in DEFAULT_SIGN_IN.
                var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(GoogleServices.ClientId)
                    .RequestScopes(new Scope(Scopes.Profile))
                    .RequestScopes(new Scope(Scopes.PlusLogin))
                    .RequestServerAuthCode(GoogleServices.ClientId)
                    .RequestProfile().RequestEmail().Build();

                // Build a GoogleApiClient with access to the Google Sign-In API and the options specified by gso.
                mGoogleApiClient = new GoogleApiClient.Builder(this, this, this)
                    .EnableAutoManage(this, this)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();

                mGoogleSignIn = FindViewById<Button>(Resource.Id.Googlelogin_button);

               

                if (!AppSettings.Show_Facebook_Login)
                    BtnFBLogin.Visibility = ViewStates.Gone;

                if (!AppSettings.Show_Google_Login)
                    mGoogleSignIn.Visibility = ViewStates.Gone;

                //==============================

                if (!string.IsNullOrEmpty(OneSignalNotification.OneSignalAPP_ID))
                {
                    if (AppSettings.ShowNotification)
                        OneSignalNotification.RegisterNotificationDevice();
                }

                if ((int) Build.VERSION.SdkInt < 23)
                {

                }
                else
                {
                    RequestPermissions(new string[]
                    {
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage,
                    }, 1);
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

                //Add Event
                Txt_forgetpass.Click += TxtForgetpassOnClick;
                mprofileTracker.mOnProfileChanged += MprofileTrackerOnM_OnProfileChanged;
                mGoogleSignIn.Click += MGsignBtnOnClick;
                Txt_Terms_of_service.Click += TxtTermsOfServiceOnClick;
                Txt_Privacy.Click += TxtPrivacyOnClick;
                Main_LinearLayout.Click += Main_LoginPage_Click;
                RegisterButton.Click += RegisterButton_Click;
                Btn_Login.Click += ButtonClickOnClick;
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
                Txt_forgetpass.Click -= TxtForgetpassOnClick;
                mprofileTracker.mOnProfileChanged -= MprofileTrackerOnM_OnProfileChanged;
                mGoogleSignIn.Click -= MGsignBtnOnClick;
                Txt_Terms_of_service.Click -= TxtTermsOfServiceOnClick;
                Txt_Privacy.Click -= TxtPrivacyOnClick;
                Main_LinearLayout.Click -= Main_LoginPage_Click;
                RegisterButton.Click -= RegisterButton_Click;
                Btn_Login.Click -= ButtonClickOnClick;
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
                string url = WoWonder_API.Client.WebsiteUrl + "/terms/privacy-policy";
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
                string url = WoWonder_API.Client.WebsiteUrl + "/terms/terms";
                IMethods.IApp.OpenbrowserUrl(this, url);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TxtForgetpassOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                StartActivity(typeof(ForgetPassword_Activity));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(typeof(Register_Activity));
                this.OverridePendingTransition(Resource.Animation.abc_grow_fade_in_from_bottom,
                    Resource.Animation.abc_shrink_fade_out_from_bottom);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Main_LoginPage_Click(object sender, EventArgs e)
        {
            try
            {
                InputMethodManager inputManager =(InputMethodManager) this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async void ButtonClickOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                        GetText(Resource.String.Lbl_Error_check_internet_connection), GetText(Resource.String.Lbl_Ok));
                }
                else
                {
                    if (!string.IsNullOrEmpty(UsernameEditext.Text) || !string.IsNullOrEmpty(PasswordEditext.Text))
                    {
 
  
                        progressBar.Visibility = ViewStates.Visible;
                        Btn_Login.Visibility = ViewStates.Gone;

                        var (apiStatus,response) = await WoWonder_API.Current.GetSettings();
                        if (apiStatus == 200)
                        {
                            if (response is GetSiteSettingsObject.Config result)
                            {
                                var PushID = result.pushId.ToString();
                                if (OneSignalNotification.OneSignalAPP_ID == "")
                                {
                                    OneSignalNotification.OneSignalAPP_ID = PushID;
                                    if (AppSettings.ShowNotification) OneSignalNotification.RegisterNotificationDevice();
                                }

                                string timeZone = IMethods.ITime.GetTimeZone();
                                var (Api_status, Respond) = await Global.Get_Auth(UsernameEditext.Text, PasswordEditext.Text, "UTC");
                                if (Api_status == 200)
                                {
                                    if (Respond is AuthObject auth)
                                    {
                                        Current.AccessToken = auth.access_token;

                                        UserDetails.Username = UsernameEditext.Text;
                                        UserDetails.Full_name = UsernameEditext.Text;
                                        UserDetails.Password = PasswordEditext.Text;
                                        UserDetails.access_token = auth.access_token;
                                        UserDetails.User_id = auth.user_id;
                                        UserDetails.Status = "Active";
                                        UserDetails.Cookie = auth.access_token;
                                        UserDetails.Email = UsernameEditext.Text;

                                        //Insert user data to database
                                        var user = new DataTables.LoginTB()
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

                                        if (AppSettings.Show_WalkTroutPage)
                                        {
                                            StartActivity(new Intent(this, typeof(AppIntroWalkTroutPage)));
                                        }
                                        else
                                        {
                                            StartActivity(new Intent(this, typeof(Tabbed_Main_Page)));

                                            //Get data profile
                                            var data = API_Request.Get_MyProfileData_Api(this).ConfigureAwait(false);
                                        }

                                        progressBar.Visibility = ViewStates.Gone;
                                        Finish();
                                    }
                                }
                                else  if(Api_status == 400)
                                {
                                    var error = Respond as ErrorObject;
                                    if (error != null)
                                    {
                                        var errortext = error._errors.Error_text;
                                        var errorid = error._errors.Error_id;

                                        if (errorid == "3")
                                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                GetText(Resource.String.Lbl_Security),
                                                GetText(Resource.String.Lbl_ErrorLogin_3), GetText(Resource.String.Lbl_Ok));
                                        else if (errorid == "4")
                                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                GetText(Resource.String.Lbl_Security),
                                                GetText(Resource.String.Lbl_ErrorLogin_4), GetText(Resource.String.Lbl_Ok));
                                        else if (errorid == "5")
                                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                GetText(Resource.String.Lbl_Security),
                                                GetText(Resource.String.Lbl_ErrorLogin_5), GetText(Resource.String.Lbl_Ok));
                                        else
                                            IMethods.DialogPopup.InvokeAndShowDialog(this,
                                                GetText(Resource.String.Lbl_Security), errortext,
                                                GetText(Resource.String.Lbl_Ok));
                                    }
                                }
                                

                                progressBar.Visibility = ViewStates.Gone;
                                Btn_Login.Visibility = ViewStates.Visible;
                            }
                            
                        }
                        else if (apiStatus == 400)
                        {
                            var error = response as ErrorObject;
                            if (error != null)
                            {
                                var errortext = error._errors.Error_text;
                                var errorid = error._errors.Error_id;

                                if (errorid == "3")
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security),
                                        GetText(Resource.String.Lbl_ErrorLogin_3), GetText(Resource.String.Lbl_Ok));
                                else if (errorid == "4")
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security),
                                        GetText(Resource.String.Lbl_ErrorLogin_4), GetText(Resource.String.Lbl_Ok));
                                else if (errorid == "5")
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security),
                                        GetText(Resource.String.Lbl_ErrorLogin_5), GetText(Resource.String.Lbl_Ok));
                                else
                                    IMethods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_Security), errortext,
                                        GetText(Resource.String.Lbl_Ok));
                            }
                        }
                        else if (apiStatus == 404)
                        {
                            //var Error = Respond.ToString();
                            IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                        }
                    }
                    else
                    {
                        progressBar.Visibility = ViewStates.Gone;
                        Btn_Login.Visibility = ViewStates.Visible;
                        IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                            GetText(Resource.String.Lbl_Please_enter_your_data), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                progressBar.Visibility = ViewStates.Gone;
                Btn_Login.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message,
                    GetText(Resource.String.Lbl_Ok));
                Console.WriteLine(exception);
            }
        }

        #region Social Logins

        private string FB_firstName, FB_lastName, FB_name, FB_email, FB_accessToken, FB_profileId;
        private string G_firstName, G_lastName, G_profileId;

        private string G_AccountName,
            G_AccountType,
            G_displayName,
            G_email,
            G_Img,
            G_Idtoken,
            G_accessToken,
            G_ServerCode;

        #region Facebook

        public void OnCancel()
        {
            try
            {
                SetResult(Result.Canceled);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnError(FacebookException error)
        {
            try
            {
                // Handle exception
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnSuccess(Object result)
        {
            try
            {
                var loginResult = result as LoginResult;
                var id = AccessToken.CurrentAccessToken.UserId;

                SetResult(Result.Ok);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void OnCompleted(JSONObject json, GraphResponse response)
        {
            try
            {
                var data = json.ToString();
                var result = JsonConvert.DeserializeObject<FacebookResult>(data);
                FB_email = result.email;

                var accessToken = AccessToken.CurrentAccessToken;
                if (accessToken != null)
                {
                    FB_accessToken = accessToken.Token;


                    progressBar.Visibility = ViewStates.Visible;
                    Btn_Login.Visibility = ViewStates.Gone;

                    var (apiStatus, respond) = await WoWonder_API.Current.GetSettings();
                    if (apiStatus == 200)
                    {
                        if (respond is GetSiteSettingsObject.Config res)
                        {
                            var PushID = res.pushId;
                            if (OneSignalNotification.OneSignalAPP_ID == "")
                            {
                                OneSignalNotification.OneSignalAPP_ID = PushID;
                                if (AppSettings.ShowNotification) OneSignalNotification.RegisterNotificationDevice();
                            }

                            var (Api_status, respond2) =
                                await WoWonder_API.Requests.RequestsAsync.Global.Get_SocialLogin(FB_accessToken,
                                    "facebook");
                            if (Api_status == 200)
                            {
                                if (respond2 is AuthObject auth)
                                {
                                    Current.AccessToken = auth.access_token;

                                    UserDetails.Username = UsernameEditext.Text;
                                    UserDetails.Full_name = UsernameEditext.Text;
                                    UserDetails.Password = PasswordEditext.Text;
                                    UserDetails.access_token = auth.access_token;
                                    UserDetails.User_id = auth.user_id;
                                    UserDetails.Status = "Active";
                                    UserDetails.Cookie = auth.access_token;
                                    UserDetails.Email = UsernameEditext.Text;

                                    //Insert user data to database
                                    var user = new DataTables.LoginTB()
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

                                    if (AppSettings.Show_WalkTroutPage)
                                    {
                                        StartActivity(new Intent(this, typeof(AppIntroWalkTroutPage)));
                                    }
                                    else
                                    {
                                        StartActivity(new Intent(this, typeof(Tabbed_Main_Page)));

                                        //Get data profile
                                        var dataUser = API_Request.Get_MyProfileData_Api(this).ConfigureAwait(false);
                                    }

                                    progressBar.Visibility = ViewStates.Gone;
                                    Btn_Login.Visibility = ViewStates.Visible;
                                    Finish();
                                }
                            }
                            else if (Api_status == 400)
                            {
                                if (respond is ErrorObject error)
                                {
                                    var errortext = error._errors.Error_text;
                                    

                                    if (errortext.Contains("Invalid or expired access_token"))
                                        API_Request.Logout(this);
                                }
                            }
                            else if (Api_status == 404)
                            {
                                var error = respond.ToString();
                               
                            }

                            progressBar.Visibility = ViewStates.Gone;
                            Btn_Login.Visibility = ViewStates.Visible;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                progressBar.Visibility = ViewStates.Gone;
                Btn_Login.Visibility = ViewStates.Visible;

                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message,
                    GetText(Resource.String.Lbl_Ok));
                Console.WriteLine(exception);
            }
        }

        private void MprofileTrackerOnM_OnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            try
            {
                if (e.mProfile != null)
                    try
                    {
                        FB_firstName = e.mProfile.FirstName;
                        FB_lastName = e.mProfile.LastName;
                        FB_name = e.mProfile.Name;
                        FB_profileId = e.mProfile.Id;

                        var request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                        var parameters = new Bundle();
                        parameters.PutString("fields", "id,name,age_range,email");
                        request.Parameters = parameters;
                        request.ExecuteAsync();
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                else
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Null_Data_User), ToastLength.Short).Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //======================================================

        #region Google

        //Event Click login using google
        private void MGsignBtnOnClick(object sender, EventArgs e)
        {
            try
            {

                mGoogleApiClient.Connect();

                var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);

                    //Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(this);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.
                    opr.SetResultCallback(new SignInResultCallback {Activity = this});
                }

                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    if (!mGoogleApiClient.IsConnecting)
                        ResolveSignInError();
                    else if (mGoogleApiClient.IsConnected) mGoogleApiClient.Disconnect();
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.GetAccounts) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.UseCredentials) == Permission.Granted)
                    {
                        if (!mGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (mGoogleApiClient.IsConnected) mGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.GetAccounts,
                            Manifest.Permission.UseCredentials
                        }, 110);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            try
            {
                Log.Debug("Login_Activity", "handleSignInResult:" + result.IsSuccess);
                if (result.IsSuccess)
                {
                    // Signed in successfully, show authenticated UI.
                    var acct = result.SignInAccount;
                    SetContentGoogle(acct);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ResolveSignInError()
        {
            try
            {
                if (mGoogleApiClient.IsConnecting) return;

                var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
                StartActivityForResult(signInIntent, 0);
            }
            catch (IntentSender.SendIntentException io)
            {
                //The intent was cancelled before it was sent. Return to the default
                //state and attempt to connect to get an updated ConnectionResult
                Console.WriteLine(io);
                mGoogleApiClient.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            try
            {
                var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
                if (opr.IsDone)
                {
                    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                    // and the GoogleSignInResult will be available instantly.
                    Log.Debug("Login_Activity", "Got cached sign-in");
                    var result = opr.Get() as GoogleSignInResult;
                    HandleSignInResult(result);
                }
                else
                {
                    // If the user has not previously signed in on this device or the sign-in has expired,
                    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                    // single sign-on will occur in this branch.

                    opr.SetResultCallback(new SignInResultCallback {Activity = this});
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void SetContentGoogle(GoogleSignInAccount acct)
        {
            try
            {
                //Successful log in hooray!!
                if (acct != null)
                {
                    progressBar.Visibility = ViewStates.Visible;
                    Btn_Login.Visibility = ViewStates.Gone;

                    G_AccountName = acct.Account.Name;
                    G_AccountType = acct.Account.Type;
                    G_displayName = acct.DisplayName;
                    G_firstName = acct.GivenName;
                    G_lastName = acct.FamilyName;
                    G_profileId = acct.Id;
                    G_email = acct.Email;
                    G_Img = acct.PhotoUrl.Path;
                    G_Idtoken = acct.IdToken;
                    G_ServerCode = acct.ServerAuthCode;

                    var api = new GoogleAPI();
                    G_accessToken = await api.GetAccessTokenAsync(G_ServerCode);

                    if (!string.IsNullOrEmpty(G_accessToken))
                    {


                        var (apiStatus,settingsResult) = await WoWonder_API.Current.GetSettings();
                        if (apiStatus == 200)
                        {
                            if (settingsResult is GetSiteSettingsObject.Config res)
                            {
                                var PushID = res.pushId;

                                if (OneSignalNotification.OneSignalAPP_ID == "")
                                {
                                    OneSignalNotification.OneSignalAPP_ID = PushID;
                                    if (AppSettings.ShowNotification) OneSignalNotification.RegisterNotificationDevice();
                                }

                                string key = IMethods.IApp.GetValueFromManifest(this, "com.google.android.geo.API_KEY");
                                var (Api_status, respond) =
                                    await Global.Get_SocialLogin(G_accessToken, "google", key);
                                if (Api_status == 200)
                                {
                                    if (respond is AuthObject auth)
                                    {
                                        Current.AccessToken = auth.access_token;

                                        UserDetails.Username = UsernameEditext.Text;
                                        UserDetails.Full_name = UsernameEditext.Text;
                                        UserDetails.Password = PasswordEditext.Text;
                                        UserDetails.access_token = auth.access_token;
                                        UserDetails.User_id = auth.user_id;
                                        UserDetails.Status = "Active";
                                        UserDetails.Cookie = auth.access_token;
                                        UserDetails.Email = UsernameEditext.Text;

                                        //Insert user data to database
                                        var user = new DataTables.LoginTB()
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

                                        if (AppSettings.Show_WalkTroutPage)
                                        {
                                            StartActivity(new Intent(this, typeof(AppIntroWalkTroutPage)));
                                        }
                                        else
                                        {
                                            StartActivity(new Intent(this, typeof(Tabbed_Main_Page)));

                                            //Get data profile
                                            var data = API_Request.Get_MyProfileData_Api(this).ConfigureAwait(false);
                                        }

                                        Finish();
                                    }
                                }
                                else if (Api_status == 400)
                                {
                                    if (respond is ErrorObject error)
                                    {
                                        var errortext = error._errors.Error_text;


                                        if (errortext.Contains("Invalid or expired access_token"))
                                            API_Request.Logout(this);
                                    }
                                }
                                else if (Api_status == 404)
                                {
                                    var error = respond.ToString();
                                
                                }

                                progressBar.Visibility = ViewStates.Gone;
                                Btn_Login.Visibility = ViewStates.Visible;
                            }
                            
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                progressBar.Visibility = ViewStates.Gone;
                Btn_Login.Visibility = ViewStates.Visible;
                IMethods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), exception.Message,
                    GetText(Resource.String.Lbl_Ok));
                Console.WriteLine(exception);
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            try
            {
                // An unresolvable error has occurred and Google APIs (including Sign-In) will not
                // be available.
                Log.Debug("Login_Activity", "onConnectionFailed:" + result);

                //The user has already clicked 'sign-in' so we attempt to resolve all
                //errors until the user is signed in, or the cancel
                ResolveSignInError();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnResult(Object result)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        //======================================================

        #endregion

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                Log.Debug("Login_Activity", "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);
                if (requestCode == 0)
                {
                    var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                    HandleSignInResult(result);
                }
                else
                {
                    // Logins Facebook
                    mFBCallManager.OnActivityResult(requestCode, (int) resultCode, data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 110)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        if (!mGoogleApiClient.IsConnecting)
                            ResolveSignInError();
                        else if (mGoogleApiClient.IsConnected) mGoogleApiClient.Disconnect();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

                mprofileTracker.StopTracking();
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            FinishAffinity();
        }
    }
}