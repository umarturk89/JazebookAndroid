using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using FFImageLoading;
using WoWonder.Activities.DefaultUser;
using WoWonder.Activities.Tab;
using WoWonder.Frameworks.onesignal;
using WoWonder.Functions;
using WoWonder.SQLite;
using WoWonder_API;
using static WoWonder_API.Requests.RequestsAsync;
using Boolean = System.Boolean;
using Exception = System.Exception;
using IMethods = WoWonder.Functions.IMethods;


namespace WoWonder.Activities.SettingsPreferences
{
    public class SettingsPrefsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener,MaterialDialog.ISingleButtonCallback
    {
        private Preference EditProfile;
        private Preference BlockedUsers;
        private Preference Account_Pref;
        private Preference Password_Pref;
        private Preference General_Invite_Pref;
        private Preference General_Call_Pref;
        private Preference Support_Help_Pref;
        private Preference Support_Logout_Pref;
        private Preference Support_deleteaccount_Pref;
        private Preference Support_Report_Pref;
        private EditTextPreference About_Me_Pref;
        private ListPreference Lang_Pref;
        private ListPreference Privacy_Follow_Pref;
        private ListPreference Privacy_Birthday_Pref;
        private ListPreference Privacy_Message_Pref;
        private SwitchPreference Notifcation_Popup_Pref;
        private CheckBoxPreference Notifcation_PlaySound_Pref;

        private readonly Activity _activityContext;
        private string S_About = "";

        public string S_WhoCanFollowMe = "0";
        public string S_WhoCanMessageMe = "0";
        public string S_WhoCanSeeMyBirthday = "0";

        public static bool S_SoundControl = true;

        public string TypeDialog = "";

        public SettingsPrefsFragment(Activity activity)
        {
            try
            {
                _activityContext = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                AddPreferencesFromResource(Resource.Xml.SettingsPrefs);

                Wo_Main_Settings.Shared_Data = PreferenceManager.SharedPreferences;

                PreferenceCategory mCategory = (PreferenceCategory)FindPreference("category_General");

                EditProfile = FindPreference("editprofile_key");
                BlockedUsers = FindPreference("blocked_key");
                Account_Pref = FindPreference("editAccount_key");
                Password_Pref = FindPreference("editpassword_key");
                General_Invite_Pref = FindPreference("invite_key");
                General_Call_Pref = FindPreference("Call_key");

                if (!AppSettings.Enable_Audio_Video_Call)
                        mCategory.RemovePreference(General_Call_Pref);
                

                if (!AppSettings.Invitation_System)     
                        mCategory.RemovePreference(General_Invite_Pref);
                

                Support_Report_Pref = FindPreference("Report_key");
                Support_Help_Pref = FindPreference("help_key");
                Support_Logout_Pref = FindPreference("logout_key");
                Support_deleteaccount_Pref = FindPreference("deleteaccount_key");

                Lang_Pref = (ListPreference) FindPreference("Lang_key");
                About_Me_Pref = (EditTextPreference) FindPreference("about_me_key");

                //Privacy
                Privacy_Follow_Pref = (ListPreference) FindPreference("whocanfollow_key");
                Privacy_Message_Pref = (ListPreference) FindPreference("whocanMessage_key");
                Privacy_Birthday_Pref = (ListPreference) FindPreference("whocanseemybirthday_key");

                Notifcation_Popup_Pref = (SwitchPreference) FindPreference("notifications_key");
                Notifcation_PlaySound_Pref = (CheckBoxPreference) FindPreference("checkBox_PlaySound_key");

                //Add Click event to Preferences
                EditProfile.Intent = new Intent(Application.Context, typeof(MyProfile_Activity));
                BlockedUsers.Intent = new Intent(Application.Context, typeof(BlockedUsers_Activity));
                Account_Pref.Intent = new Intent(Application.Context, typeof(My_Account_Activity));
                Password_Pref.Intent = new Intent(Application.Context, typeof(Password_Activity));
                General_Invite_Pref.Intent = new Intent(Application.Context, typeof(Invite_Friends_Activity));

                //Update Preferences data on Load
                OnSharedPreferenceChanged(Wo_Main_Settings.Shared_Data, "about_me_key");
                OnSharedPreferenceChanged(Wo_Main_Settings.Shared_Data, "whocanfollow_key");
                OnSharedPreferenceChanged(Wo_Main_Settings.Shared_Data, "whocanMessage_key");
                OnSharedPreferenceChanged(Wo_Main_Settings.Shared_Data, "whocanseemybirthday_key");
                OnSharedPreferenceChanged(Wo_Main_Settings.Shared_Data, "notifications_key");
                OnSharedPreferenceChanged(Wo_Main_Settings.Shared_Data, "checkBox_PlaySound_key");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        //Delete Acount
        private void SupportDeleteaccountPrefOnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                TypeDialog = "Delete";

                var dialog = new MaterialDialog.Builder(_activityContext);

                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(_activityContext.GetText(Resource.String.Lbl_Are_you_DeleteAccount) + " " +
                               AppSettings.Application_Name);
                dialog.PositiveText(_activityContext.GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(_activityContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.Build().Show();
                dialog.AlwaysCallSingleChoiceCallback();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void GeneralCallPrefOnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                dbDatabase.Clear_CallUser_List();
                Last_Calls_Fragment.mAdapter?.Clear();

                Toast.MakeText(_activityContext, this.GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                if (AppSettings.Enable_Audio_Video_Call)
                {
                    General_Call_Pref.PreferenceClick += GeneralCallPrefOnPreferenceClick;
                }

                //Support
                Support_Report_Pref.PreferenceClick += SupportReportPrefOnPreferenceClick;
                Support_Help_Pref.PreferenceClick += SupportHelpPrefOnPreferenceClick;
                //Add OnChange event to Preferences
                About_Me_Pref.PreferenceChange += About_Me_Pref_PreferenceChange;
                Lang_Pref.PreferenceChange += Lang_Pref_PreferenceChange;
                Privacy_Follow_Pref.PreferenceChange += Privacy_Follow_Pref_PreferenceChange;
                Privacy_Message_Pref.PreferenceChange += Privacy_Message_Pref_PreferenceChange;
                Privacy_Birthday_Pref.PreferenceChange += Privacy_Birthday_Pref_PreferenceChange;
                Notifcation_Popup_Pref.PreferenceChange += Notifcation_Popup_Pref_PreferenceChange;
                Notifcation_PlaySound_Pref.PreferenceChange += Notifcation_PlaySound_Pref_PreferenceChange;
                //Event Click Items
                Support_Logout_Pref.PreferenceClick += SupportLogout_OnPreferenceClick;
                Support_deleteaccount_Pref.PreferenceClick += SupportDeleteaccountPrefOnPreferenceClick;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();
                PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);

                if (AppSettings.Enable_Audio_Video_Call)
                {
                    General_Call_Pref.PreferenceClick -= GeneralCallPrefOnPreferenceClick;
                }

                //Support
                Support_Report_Pref.PreferenceClick -= SupportReportPrefOnPreferenceClick;
                Support_Help_Pref.PreferenceClick -= SupportHelpPrefOnPreferenceClick;
                //Add OnChange event to Preferences
                About_Me_Pref.PreferenceChange -= About_Me_Pref_PreferenceChange;
                Lang_Pref.PreferenceChange -= Lang_Pref_PreferenceChange;
                Privacy_Follow_Pref.PreferenceChange -= Privacy_Follow_Pref_PreferenceChange;
                Privacy_Message_Pref.PreferenceChange -= Privacy_Message_Pref_PreferenceChange;
                Privacy_Birthday_Pref.PreferenceChange -= Privacy_Birthday_Pref_PreferenceChange;
                Notifcation_Popup_Pref.PreferenceChange -= Notifcation_Popup_Pref_PreferenceChange;
                Notifcation_PlaySound_Pref.PreferenceChange -= Notifcation_PlaySound_Pref_PreferenceChange;
                //Event Click Items
                Support_Logout_Pref.PreferenceClick -= SupportLogout_OnPreferenceClick;
                Support_deleteaccount_Pref.PreferenceClick -= SupportDeleteaccountPrefOnPreferenceClick;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (key.Equals("about_me_key"))
                {
                    // Set summary to be the user-description for the selected value
                    EditTextPreference etp = (EditTextPreference) FindPreference("about_me_key");

                    if (datauser != null)
                    {
                        Wo_Main_Settings.Shared_Data.Edit().PutString("about_me_key", datauser.about).Commit();

                        S_About = datauser.about;

                        if (S_About != "Empty")
                        {
                            etp.EditText.Text = S_About;
                            etp.Text = S_About;
                        }
                        else
                        {
                            etp.EditText.Text = this.GetText(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.Application_Name;
                            etp.Text = this.GetText(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.Application_Name;
                        }
                    }

                    string Getvalue = Wo_Main_Settings.Shared_Data.GetString("about_me_key", S_About);
                    etp.EditText.Text = Getvalue;
                    etp.Summary = Getvalue;

                }
                else if (key.Equals("whocanfollow_key"))
                {
                    var valueAsText = Privacy_Follow_Pref.Entry;
                    if (string.IsNullOrEmpty(valueAsText))
                    {
                        if (datauser != null)
                        {
                            Wo_Main_Settings.Shared_Data.Edit().PutString("whocanfollow_key", datauser.follow_privacy)
                                .Commit();
                            Privacy_Follow_Pref.SetValueIndex(int.Parse(datauser.follow_privacy));

                            S_WhoCanFollowMe = datauser.follow_privacy;
                            if (S_WhoCanFollowMe == "0")
                            {
                                Privacy_Follow_Pref.Summary = this.GetText(Resource.String.Lbl_Everyone);
                            }
                            else if (S_WhoCanFollowMe == "1")
                            {
                                Privacy_Follow_Pref.Summary = this.GetText(Resource.String.Lbl_People_i_Follow);
                            }
                            else
                            {
                                Privacy_Follow_Pref.Summary = this.GetText(Resource.String.Lbl_No_body);
                            }
                        }
                    }
                    else
                    {
                        Privacy_Follow_Pref.Summary = valueAsText;
                        S_WhoCanFollowMe = Privacy_Follow_Pref.Value;
                        //Privacy_Follow_Pref.SetValueIndex(0);
                    }
                }
                else if (key.Equals("whocanMessage_key"))
                {
                    var valueAsText = Privacy_Message_Pref.Entry;
                    if (string.IsNullOrEmpty(valueAsText))
                    {
                        if (datauser != null)
                        {
                            Privacy_Message_Pref.SetValueIndex(int.Parse(datauser.message_privacy));
                            Wo_Main_Settings.Shared_Data.Edit().PutString("whocanMessage_key", datauser.message_privacy)
                                .Commit();
                            S_WhoCanMessageMe = datauser.message_privacy;
                            if (S_WhoCanMessageMe == "0")
                            {
                                Privacy_Message_Pref.Summary = this.GetText(Resource.String.Lbl_Everyone);
                            }
                            else if (S_WhoCanMessageMe == "1")
                            {
                                Privacy_Message_Pref.Summary = this.GetText(Resource.String.Lbl_People_i_Follow);
                            }
                            else
                            {
                                Privacy_Message_Pref.Summary = this.GetText(Resource.String.Lbl_No_body);
                            }
                        }
                    }
                    else
                    {
                        Privacy_Message_Pref.Summary = valueAsText;
                        S_WhoCanMessageMe = Privacy_Message_Pref.Value;
                    }
                }
                else if (key.Equals("whocanseemybirthday_key"))
                {
                    var valueAsText = Privacy_Birthday_Pref.Entry;
                    if (string.IsNullOrEmpty(valueAsText))
                    {
                        if (datauser != null)
                        {
                            Privacy_Birthday_Pref.SetValueIndex(int.Parse(datauser.birth_privacy));
                            Wo_Main_Settings.Shared_Data.Edit()
                                .PutString("whocanseemybirthday_key", datauser.birth_privacy).Commit();

                            S_WhoCanSeeMyBirthday = datauser.birth_privacy;
                            if (S_WhoCanSeeMyBirthday == "0")
                            {
                                Privacy_Birthday_Pref.Summary = this.GetText(Resource.String.Lbl_Everyone);
                            }
                            else if (S_WhoCanSeeMyBirthday == "1")
                            {
                                Privacy_Birthday_Pref.Summary = this.GetText(Resource.String.Lbl_People_i_Follow);
                            }
                            else
                            {
                                Privacy_Birthday_Pref.Summary = this.GetText(Resource.String.Lbl_No_body);
                            }
                        }
                    }
                    else
                    {
                        Privacy_Birthday_Pref.Summary = valueAsText;
                        S_WhoCanSeeMyBirthday = Privacy_Birthday_Pref.Value;
                    }
                }
                else if (key.Equals("notifications_key"))
                {
                    bool Getvalue = Wo_Main_Settings.Shared_Data.GetBoolean("notifications_key", true);
                    Notifcation_Popup_Pref.Checked = Getvalue;
                }
                else if (key.Equals("checkBox_PlaySound_key"))
                {
                    bool Getvalue = Wo_Main_Settings.Shared_Data.GetBoolean("checkBox_PlaySound_key", true);
                    Notifcation_PlaySound_Pref.Checked = Getvalue;

                    if (Getvalue)
                    {
                        S_SoundControl = true;
                    }
                    else
                    {
                        S_SoundControl = false;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Help
        private void SupportHelpPrefOnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                Support_Help_Pref.Intent = new Intent(Application.Context, typeof(Help_Activity));
                Support_Help_Pref.Intent.PutExtra("URL", Client.WebsiteUrl + "/terms/about-us");
                StartActivity(Support_Help_Pref.Intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Report >> Contact Us
        private void SupportReportPrefOnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                Support_Report_Pref.Intent = new Intent(Application.Context, typeof(Report_Activity));
                Support_Report_Pref.Intent.PutExtra("URL", Client.WebsiteUrl + "/contact-us");
                StartActivity(Support_Report_Pref.Intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Logout
        private void SupportLogout_OnPreferenceClick(object sender,
            Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                TypeDialog = "Logout";

                var dialog = new MaterialDialog.Builder(Context);

                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(Context.GetText(Resource.String.Lbl_Are_you_logout));
                dialog.PositiveText(Context.GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(Context.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Notification >> Play Sound 
        private void Notifcation_PlaySound_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            if (e.Handled)
            {
                CheckBoxPreference etp = (CheckBoxPreference) sender;
                var value = e.NewValue.ToString();
                etp.Checked = Boolean.Parse(value);
                if (etp.Checked)
                {
                    S_SoundControl = true;
                }
                else
                {
                    S_SoundControl = false;
                }
            }
        }

        //Notifcation >> Popup 
        private void Notifcation_Popup_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            if (e.Handled)
            {
                SwitchPreference etp = (SwitchPreference) sender;
                var value = e.NewValue.ToString();
                etp.Checked = Boolean.Parse(value);
                if (etp.Checked)
                {
                    OneSignalNotification.RegisterNotificationDevice();
                }
                else
                {
                    OneSignalNotification.Un_RegisterNotificationDevice();
                }
            }
        }

        #region Privacy

        private void Privacy_Birthday_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    ListPreference etp = (ListPreference) sender;
                    var value = e.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[Int32.Parse(value)];
                    etp.Summary = valueAsText;

                    S_WhoCanSeeMyBirthday = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"birth_privacy", S_WhoCanSeeMyBirthday}
                        };

                        var data = Global.Update_User_Data(dataPrivacy)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Privacy_Message_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    ListPreference etp = (ListPreference) sender;
                    var value = e.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[Int32.Parse(value)];
                    etp.Summary = valueAsText;

                    S_WhoCanMessageMe = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"message_privacy", S_WhoCanMessageMe},
                        };

                        var data = Global.Update_User_Data(dataPrivacy)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Privacy_Follow_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    ListPreference etp = (ListPreference) sender;
                    var value = e.NewValue.ToString();
                    var valueAsText = etp.GetEntries()[Int32.Parse(value)];
                    etp.Summary = valueAsText;

                    S_WhoCanFollowMe = value;

                    if (IMethods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"follow_privacy", S_WhoCanFollowMe},
                        };

                        var data = Global.Update_User_Data(dataPrivacy)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //Lang
        private void Lang_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    Android.Preferences.ListPreference etp = (Android.Preferences.ListPreference) sender;
                    var value = e.NewValue;

                    Wo_Main_Settings.SetApplicationLang(value.ToString());

                    Toast.MakeText(_activityContext, this.GetText(Resource.String.Lbl_Closed_App), ToastLength.Long).Show();

                    Intent intent = new Intent(_activityContext, typeof(SpalshScreen_Activity));
                    intent.AddCategory(Intent.CategoryHome);
                    intent.SetAction(Intent.ActionMain);
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    _activityContext.StartActivity(intent);
                    _activityContext.FinishAffinity();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //About
        private void About_Me_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                EditTextPreference etp = (EditTextPreference) sender;
                var value = etp.EditText.Text;
                etp.Summary = value;

                var datauser = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (datauser != null)
                {
                    datauser.about = etp.EditText.Text;
                    S_About = etp.EditText.Text;
                }

                if (IMethods.CheckConnectivity())
                {
                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"about", value},
                    };

                    var data = Global.Update_User_Data(dataPrivacy)
                        .ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_Error_check_internet_connection),
                        ToastLength.Long).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (TypeDialog == "Delete")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        var Intent = new Intent(Context, typeof(DeleteAccount_Activity));
                        StartActivity(Intent);
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else if (TypeDialog == "Logout")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            Toast.MakeText(_activityContext, _activityContext.GetText(Resource.String.Lbl_You_will_be_logged),ToastLength.Long).Show();
                            API_Request.Logout(_activityContext);
                        }
                        else
                        {
                            if (_activityContext.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                                _activityContext.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                            {
                                Toast.MakeText(_activityContext, _activityContext.GetText(Resource.String.Lbl_You_will_be_logged),
                                    ToastLength.Long).Show();
                                API_Request.Logout(_activityContext);
                            }
                            else
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.ReadExternalStorage,
                                    Manifest.Permission.WriteExternalStorage
                                }, 101);
                        }
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
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

        public override void OnDestroy()
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