using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using FFImageLoading;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.SettingsPreferences
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class My_Account_Activity : AppCompatActivity
    {

        #region Variables Basic

        private static AppCompatTextView Txt_Username_icon;
        private static EditText Txt_Username_text;

        private static AppCompatTextView Txt_Email_icon;
        private static EditText Txt_Email_text;

        private static AppCompatTextView Txt_Gender_icon;
        private static RadioGroup RadioGender;
        private RadioButton RB_Male;
        private RadioButton RB_Female;

        public string GenderStatus = "";
        public string Phone_number = "";

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "Settings_MyAccount_Layout" layout resource
                SetContentView(Resource.Layout.Settings_MyAccount_Layout);

                //Set ToolBar
                var ToolBar = FindViewById<Toolbar>(Resource.Id.MyAccounttoolbar);
                ToolBar.Title = this.GetText(Resource.String.Lbl_My_Account);

                SetSupportActionBar(ToolBar);
                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                //Get values
                Txt_Username_icon = FindViewById<AppCompatTextView>(Resource.Id.Username_icon);
                Txt_Username_text = FindViewById<EditText>(Resource.Id.Username_text);

                Txt_Email_icon = FindViewById<AppCompatTextView>(Resource.Id.Email_icon);
                Txt_Email_text = FindViewById<EditText>(Resource.Id.Email_text);

                Txt_Gender_icon = FindViewById<AppCompatTextView>(Resource.Id.Gendericon);
                RadioGender = FindViewById<RadioGroup>(Resource.Id.radioGender);
                RB_Male = (RadioButton)FindViewById(Resource.Id.radioMale);
                RB_Female = (RadioButton)FindViewById(Resource.Id.radioFemale);

                Get_Data_User();
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

                //Event
                RB_Male.CheckedChange += RbMaleOnCheckedChange;
                RB_Female.CheckedChange += RbFemaleOnCheckedChange;
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
                //Event
                RB_Male.CheckedChange -= RbMaleOnCheckedChange;
                RB_Female.CheckedChange -= RbFemaleOnCheckedChange;

                base.OnPause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private void RbFemaleOnCheckedChange(object sender,
            CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                bool isChecked = RB_Female.Checked;
                if (isChecked)
                {
                    RB_Male.Checked = false;
                    GenderStatus = "female";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RbMaleOnCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs checkedChangeEventArgs)
        {
            try
            {
                bool isChecked = RB_Male.Checked;
                if (isChecked)
                {
                    RB_Female.Checked = false;
                    GenderStatus = "male";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Get_Data_User()
        {
            try
            {
                IMethods.Set_TextViewIcon("1", Txt_Username_icon, IonIcons_Fonts.Person);
                IMethods.Set_TextViewIcon("1", Txt_Email_icon, IonIcons_Fonts.Email);
                IMethods.Set_TextViewIcon("1", Txt_Gender_icon, IonIcons_Fonts.Male);

                var local = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (local != null)
                {
                    Txt_Username_text.Text = local.username;
                    Txt_Email_text.Text = local.email;
                    Phone_number = local.phone_number;

                    if (local.gender == "male" || local.gender == "Male")
                    {
                        RB_Male.Checked = true;
                        RB_Female.Checked = false;
                        GenderStatus = "male";
                    }
                    else
                    {
                        RB_Male.Checked = false;
                        RB_Female.Checked = true;
                        GenderStatus = "female";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void SaveDataButtonOnClick()
        {
            try
            {

                if (IMethods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, this.GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                        {
                            {"username", Txt_Username_text.Text},
                            {"email", Txt_Email_text.Text},
                            {"gender", GenderStatus},
                            {"phone_number", Phone_number},
                        };

                    var (Api_status, Respond) = await Global.Update_User_Data(dictionary);
                    if (Api_status == 200)
                    {
                        if (Respond is MessageObject result)
                        {
                            if (result.Message.Contains("updated"))
                            {
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                                AndHUD.Shared.Dismiss(this);
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

                            //Show a Error image with a message
                            AndHUD.Shared.ShowError(this, errortext, MaskType.Clear, TimeSpan.FromSeconds(2));
                        }
                    }
                    else if (Api_status == 404)
                    {
                        var error = Respond.ToString();
                       

                        //Show a Error image with a message
                        AndHUD.Shared.ShowError(this, error, MaskType.Clear, TimeSpan.FromSeconds(2));
                    }

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, this.GetText(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Profile_Menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                case Resource.Id.menue_SaveData:
                    SaveDataButtonOnClick();
                    break;
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