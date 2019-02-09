using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Github.Florent37.Diagonallayout;
using Com.Theartofdev.Edmodo.Cropper;
using FFImageLoading;
using Java.Lang;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Console = System.Console;
using Exception = System.Exception;
using File = Java.IO.File;

using Uri = Android.Net.Uri;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Activities.DefaultUser
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class MyProfile_Activity : AppCompatActivity, View.IOnClickListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {

        #region Variables Basic

        private static TextView Txt_UserName;
        private static TextView Txt_Fullname;
        private static TextView Txt_Following;
        private static TextView Txt_FollowingCount;
        private static TextView Txt_Followers;
        private static TextView Txt_FollowersCount;

        private static AppCompatTextView Txt_name_icon;
        private static EditText Txt_FirstName;
        private static EditText Txt_LastName;

        private static AppCompatTextView Txt_gender_icon;
        private static EditText Txt_gender_text;
        private static AppCompatTextView Txt_location_icon;
        private static EditText Txt_location_text;
        private static AppCompatTextView Txt_mobile_icon;
        private static EditText Txt_mobile_text;
        private static AppCompatTextView Txt_website_icon;
        private static EditText Txt_website_text;
        private static AppCompatTextView Txt_work_icon;
        private static EditText Txt_work_text;

        private static ImageView Image_Userprofile;
        private static ImageView Image_UserCover;

        private static CircleButton EditProfile_button;

        private static AppCompatTextView Txt_facebook_icon;
        private static EditText Txt_facebook_text;
        private static AppCompatTextView Txt_Google_icon;
        private static EditText Txt_Google_text;
        private static AppCompatTextView Txt_Twitter_icon;
        private static EditText Txt_Twitter_text;
        private static AppCompatTextView Txt_VK_icon;
        private static EditText Txt_VK_text;
        private static AppCompatTextView Txt_Instagram_icon;
        private static EditText Txt_Instagram_text;
        private static AppCompatTextView Txt_Youtube_icon;
        private static EditText Txt_Youtube_text;

        private static string S_UserId = "";
        private static string S_About = "";
        private static string S_School = "";
        private static string S_Facebook = "";
        private static string S_Google = "";
        private static string S_Twitter = "";
        private static string S_Youtube = "";
        private static string S_VK = "";
        private static string S_Instagram = "";

        public string TypeIMG = "";

        private DiagonalLayoutSettings settings;

        private AdView mAdView;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "Last_Messages_Fragment" layout resource
                SetContentView(Resource.Layout.MyProfile_Layout);

                S_UserId = UserDetails.User_id;

                //Set ToolBar
                var ToolBar = FindViewById<Toolbar>(Resource.Id.myprofiletoolbar);
                ToolBar.Title = "";

                SetSupportActionBar(ToolBar);
                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                //Get values
                Txt_name_icon = FindViewById<AppCompatTextView>(Resource.Id.name_icon);

                Txt_Fullname = FindViewById<TextView>(Resource.Id.Txt_fullname);
                Txt_UserName = FindViewById<TextView>(Resource.Id.username);

                Txt_Followers = FindViewById<TextView>(Resource.Id.Txt_flowersView);
                Txt_FollowersCount = FindViewById<TextView>(Resource.Id.Txt_flowers_count);

                Txt_Following = FindViewById<TextView>(Resource.Id.flowinglabelView);
                Txt_FollowingCount = FindViewById<TextView>(Resource.Id.Txt_flowing_countView);

                Txt_FirstName = FindViewById<EditText>(Resource.Id.FirstName_text);
                Txt_LastName = FindViewById<EditText>(Resource.Id.LastName_text);

                Image_Userprofile = FindViewById<ImageView>(Resource.Id.profile_image);
                Image_UserCover = FindViewById<ImageView>(Resource.Id.coverImageView);

                Txt_gender_icon = FindViewById<AppCompatTextView>(Resource.Id.gender_icon);
                Txt_gender_text = FindViewById<EditText>(Resource.Id.gender_text);

                Txt_location_icon = FindViewById<AppCompatTextView>(Resource.Id.location_icon);
                Txt_location_text = FindViewById<EditText>(Resource.Id.location_text);

                Txt_mobile_icon = FindViewById<AppCompatTextView>(Resource.Id.mobile_icon);
                Txt_mobile_text = FindViewById<EditText>(Resource.Id.mobile_text);

                Txt_website_icon = FindViewById<AppCompatTextView>(Resource.Id.website_icon);
                Txt_website_text = FindViewById<EditText>(Resource.Id.website_text);

                Txt_work_icon = FindViewById<AppCompatTextView>(Resource.Id.work_icon);
                Txt_work_text = FindViewById<EditText>(Resource.Id.work_text);

                Txt_facebook_icon = FindViewById<AppCompatTextView>(Resource.Id.facebook_icon);
                Txt_facebook_text = FindViewById<EditText>(Resource.Id.facebook_text);
                Txt_Google_icon = FindViewById<AppCompatTextView>(Resource.Id.Google_icon);
                Txt_Google_text = FindViewById<EditText>(Resource.Id.Google_text);
                Txt_Twitter_icon = FindViewById<AppCompatTextView>(Resource.Id.Twitter_icon);
                Txt_Twitter_text = FindViewById<EditText>(Resource.Id.Twitter_text);
                Txt_VK_icon = FindViewById<AppCompatTextView>(Resource.Id.VK_icon);
                Txt_VK_text = FindViewById<EditText>(Resource.Id.VK_text);
                Txt_Instagram_icon = FindViewById<AppCompatTextView>(Resource.Id.Instagram_icon);
                Txt_Instagram_text = FindViewById<EditText>(Resource.Id.Instagram_text);
                Txt_Youtube_icon = FindViewById<AppCompatTextView>(Resource.Id.Youtube_icon);
                Txt_Youtube_text = FindViewById<EditText>(Resource.Id.Youtube_text);

                IMethods.Set_TextViewIcon("1", Txt_gender_icon, IonIcons_Fonts.Male);
                Txt_gender_icon.SetTextColor(Color.ParseColor("#4693d8"));

                IMethods.Set_TextViewIcon("1", Txt_location_icon, IonIcons_Fonts.Location);
                Txt_location_icon.SetTextColor(Color.ParseColor(AppSettings.MainColor));

                IMethods.Set_TextViewIcon("1", Txt_mobile_icon, IonIcons_Fonts.AndroidCall);
                Txt_mobile_icon.SetTextColor(Color.ParseColor("#fa6670"));

                IMethods.Set_TextViewIcon("1", Txt_website_icon, IonIcons_Fonts.AndroidGlobe);
                Txt_website_icon.SetTextColor(Color.ParseColor("#6b38d1"));

                IMethods.Set_TextViewIcon("1", Txt_work_icon, IonIcons_Fonts.Briefcase);
                Txt_work_icon.SetTextColor(Color.ParseColor("#eca72c"));

                EditProfile_button = FindViewById<CircleButton>(Resource.Id.Edit_button);
                //EditProfile_button.Click += EditProfileButtonOnClick;
                EditProfile_button.Visibility = ViewStates.Invisible;


                IMethods.Set_TextViewIcon("1", Txt_name_icon, IonIcons_Fonts.Person);
                IMethods.Set_TextViewIcon("1", Txt_facebook_icon, IonIcons_Fonts.SocialFacebook);
                IMethods.Set_TextViewIcon("1", Txt_Google_icon, IonIcons_Fonts.SocialGoogle);
                IMethods.Set_TextViewIcon("1", Txt_Twitter_icon, IonIcons_Fonts.SocialTwitter);
                IMethods.Set_TextViewIcon("4", Txt_VK_icon, "\uf189");
                IMethods.Set_TextViewIcon("1", Txt_Instagram_icon, IonIcons_Fonts.SocialInstagram);
                IMethods.Set_TextViewIcon("1", Txt_Youtube_icon, IonIcons_Fonts.SocialYoutube);

                AdsGoogle.Ad_Interstitial(this);

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
                    mAdView.Visibility = ViewStates.Gone;
                }

                Txt_gender_text.SetOnClickListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnStart()
        {
            try
            {
                base.OnStart();

                Get_MyProfileData_Local();
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
                Image_Userprofile.Click += ImageUserprofileOnClick;
                Image_UserCover.Click += ImageUserCoverOnClick;

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
                Image_Userprofile.Click -= ImageUserprofileOnClick;
                Image_UserCover.Click -= ImageUserCoverOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
 
        #region Method Upload Photo >> Cover Or Avatar

        private void ImageUserCoverOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                try
                {
                    TypeIMG = "Cover";
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        //Open Image 
                        Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimMyApp,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(this.GetText(Resource.String.Lbl_Done))
                            .SetActivityTitle(this.GetText(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                        return;
                    }
                    else
                    {
                        if (CropImage.IsExplicitCameraPermissionRequired(this))
                        {
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.Camera,
                                Manifest.Permission.ReadExternalStorage
                            }, CropImage.PickImagePermissionsRequestCode);
                        }
                        else
                        {
                            //Open Image 
                            Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimMyApp,
                                IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                            CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true)
                                .SetMaxZoom(4).SetGuidelines(CropImageView.Guidelines.On)
                                .SetCropMenuCropButtonTitle(this.GetText(Resource.String.Lbl_Done)).SetActivityTitle(this.GetText(Resource.String.Lbl_Crop))
                                .SetOutputUri(myUri).Start(this);
                            return;
                        }

                        if (!ShouldShowRequestPermissionRationale(Manifest.Permission.ReadExternalStorage))
                        {
                            //Open Image 
                            Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimMyApp,
                                IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                            CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true)
                                .SetMaxZoom(4).SetGuidelines(CropImageView.Guidelines.On)
                                .SetCropMenuCropButtonTitle(this.GetText(Resource.String.Lbl_Done)).SetActivityTitle(this.GetText(Resource.String.Lbl_Crop))
                                .SetOutputUri(myUri).Start(this);
                            return;
                        }
                        else
                        {
                            Toast.MakeText(this, this.GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ImageUserprofileOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                TypeIMG = "Avatar";
                if ((int) Build.VERSION.SdkInt < 23)
                {
                    //Open Image 
                    Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimMyApp,
                        IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(this.GetText(Resource.String.Lbl_Done))
                        .SetActivityTitle(this.GetText(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                    return;
                }
                else
                {
                    if (CropImage.IsExplicitCameraPermissionRequired(this))
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.ReadExternalStorage
                        }, CropImage.PickImagePermissionsRequestCode);
                    }
                    else
                    {
                        //Open Image 
                        Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimMyApp,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(this.GetText(Resource.String.Lbl_Done))
                            .SetActivityTitle(this.GetText(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                        return;
                    }

                    if (!ShouldShowRequestPermissionRationale(Manifest.Permission.ReadExternalStorage))
                    {
                        //Open Image 
                        Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimMyApp,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(this.GetText(Resource.String.Lbl_Done))
                            .SetActivityTitle(this.GetText(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                        return;
                    }
                    else
                    {
                        Toast.MakeText(this, this.GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void Update_Image_Api(string type, string path)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Short)
                        .Show();
                }
                else
                {
                    if (type == "Avatar")
                    {
                        var (api_status, respond) = await Global.Update_User_Avatar(path);
                        if (api_status == 200)
                        {
                            if (respond is MessageObject result)
                            {
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();

                                var file = Uri.FromFile(new File(path));
                                //Set image 
                                ImageCacheLoader.LoadImage(file.Path,Image_Userprofile,false,true);

                            }
                        }
                        else if (api_status == 400)
                        {
                            if (respond is ErrorObject error)
                            {
                                var errortext = error._errors.Error_text;
                                Toast.MakeText(this, errortext, ToastLength.Short).Show();

                                if (errortext.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (api_status == 404)
                        {
                            var error = respond.ToString();
                            Toast.MakeText(this, error, ToastLength.Short).Show();
                        }
                    }
                    else if (type == "Cover")
                    {
                        var (api_status, respond) = await Global.Update_User_Cover(path);
                        if (api_status == 200)
                        {
                            if (respond is MessageObject result)
                            {
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();

                                //Set image 
                                var file = Uri.FromFile(new File(path));
                                ImageCacheLoader.LoadImage(file.Path, Image_UserCover, false, false);
                                
                            }
                        }
                        else if (api_status == 400)
                        {
                            if (respond is ErrorObject error)
                            {
                                var errortext = error._errors.Error_text;
                                Toast.MakeText(this, errortext, ToastLength.Short).Show();

                                if (errortext.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (api_status == 404)
                        {
                            var error = respond.ToString();
                            Toast.MakeText(this, error, ToastLength.Short).Show();
                        }
                    }
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

                if (requestCode == CropImage.PickImagePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //Open Image 
                        Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimMyApp,
                            IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(this.GetText(Resource.String.Lbl_Done))
                            .SetActivityTitle(this.GetText(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                        return;
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == CropImage.CameraCapturePermissionsRequestCode)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        CropImage.StartPickImageActivity(this);
                    else
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                            .Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                //If its from Camera or Gallary
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        var imageUri = CropImage.GetPickImageResultUri(this, data);

                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                var pathimg = "";
                                if (TypeIMG == "Cover")
                                {
                                    pathimg = resultUri.Path;
                                    Update_Image_Api(TypeIMG, pathimg);
                                }
                                else if (TypeIMG == "Avatar")
                                {
                                    pathimg = resultUri.Path;
                                    Update_Image_Api(TypeIMG, pathimg);
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong),
                                    ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long)
                                .Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long)
                            .Show();
                    }
                }
                else if (requestCode == CropImage.CropImageActivityResultErrorCode)
                {
                    var result = CropImage.GetActivityResult(data);
                    Exception error = result.Error;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }
        }

        #endregion

        #region Get Data User

        //Get Data User From Database 
        public void Get_MyProfileData_Local()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var dataUser = dbDatabase.Get_MyProfile(UserDetails.User_id);
                if (dataUser != null)
                {
                    if (Classes.MyProfileList.Count > 0)
                    {
                        var local = Classes.MyProfileList.FirstOrDefault(a => a.user_id == S_UserId);
                        if (local != null) LoadDataUser(local);
                    }
                    else
                    {
                        Classes.MyProfileList = dataUser;
                    }
                }

                dbDatabase.Dispose();

                Get_MyProfileData_Api();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get Data My Profile API
        public async void Get_MyProfileData_Api()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection),
                        ToastLength.Short).Show();
                }
                else
                {
                    var (Api_status, Respond) = await Global.Get_User_Data(S_UserId);
                    if (Api_status == 200)
                    {
                        if (Respond is GetUserDataObject result)
                        {
                            var dbDatabase = new SqLiteDatabase();

                            //Add Data User
                            //=======================================
                            // user_data
                            if (result.user_data != null)
                            {
                                LoadDataUser(result.user_data);
                                
                                dbDatabase.Insert_Or_Update_To_MyProfileTable(result.user_data);
                            }

                            dbDatabase.Dispose();
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Get_MyProfileData_Api();
            }
        }

        private void LoadDataUser(GetUserDataObject.User_Data data)
        {
            try
            { 
                //Cover
                var CoverSplit = data.cover.Split('/').Last();
                var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                if (getImage_Cover != "File Dont Exists")
                {
               
                    ImageCacheLoader.LoadImage(data.cover, Image_UserCover, false, false); 
                }
                else
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, data.cover);
                   
                    ImageCacheLoader.LoadImage(data.cover, Image_UserCover, false, false);
                }

                //profile_picture
                var AvatarSplit = data.avatar.Split('/').Last();
                var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                if (getImage_Avatar != "File Dont Exists")
                {
                    ImageCacheLoader.LoadImage(data.avatar, Image_Userprofile, false, true);
                   
                }
                else
                {
                    IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, data.avatar);
                  
                    ImageCacheLoader.LoadImage(data.avatar, Image_Userprofile, false, true);
                }

                UserDetails.Full_name = data.name;

                string name = IMethods.Fun_String.DecodeString(data.name);
                Txt_Fullname.Text = name;
                Txt_UserName.Text = "@" + data.username;
                Txt_FirstName.Text = IMethods.Fun_String.DecodeString(data.first_name);
                Txt_LastName.Text = IMethods.Fun_String.DecodeString(data.last_name);

                S_About = data.about;
                S_School = data.school;

                var following = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(data.details.following_count));
                var followers = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(data.details.followers_count));

                if (AppSettings.ConnectivitySystem == "1")
                {
                    Txt_Following.Visibility = ViewStates.Visible;
                    Txt_FollowingCount.Visibility = ViewStates.Visible;

                    Txt_Followers.Visibility = ViewStates.Visible;
                    Txt_FollowersCount.Visibility = ViewStates.Visible;

                    Txt_Following.Text = this.GetText(Resource.String.Lbl_Following);
                    Txt_FollowingCount.Text = following;

                    Txt_Followers.Text = this.GetText(Resource.String.Lbl_Followers);
                    Txt_FollowersCount.Text = followers;
                }
                else
                {
                    Txt_Following.Visibility = ViewStates.Visible;
                    Txt_FollowingCount.Visibility = ViewStates.Visible;

                    Txt_Followers.Visibility = ViewStates.Gone;
                    Txt_FollowersCount.Visibility = ViewStates.Gone;

                    Txt_Following.Text = this.GetText(Resource.String.Lbl_Friends);
                    Txt_FollowingCount.Text = following;
                }

                switch (data.gender.ToLower())
                {
                    case "male":
                        Txt_gender_text.Text = this.GetText(Resource.String.Radio_Male);
                        break;
                    case "female":
                        Txt_gender_text.Text = this.GetText(Resource.String.Radio_Female);
                        break;
                    default:
                        Txt_gender_text.Text = data.gender;
                        break;
                }

                Txt_location_text.Text = data.address;
                Txt_mobile_text.Text = data.phone_number;
                Txt_website_text.Text = data.website;
                Txt_work_text.Text = data.working;

                S_Facebook = data.facebook;
                S_Google = data.google;
                S_Twitter = data.twitter;
                S_Youtube = data.youtube;
                S_VK = data.vk;
                S_Instagram = data.instagram;

                Txt_facebook_text.Text = S_Facebook;
                Txt_Google_text.Text = S_Google;
                Txt_Twitter_text.Text = S_Twitter;
                Txt_VK_text.Text = S_VK;
                Txt_Instagram_text.Text = S_Instagram;
                Txt_Youtube_text.Text = S_Youtube;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private async void EditProfileButtonOnClick()
        {
            try
            {
                var local = Classes.MyProfileList.FirstOrDefault(a => a.user_id == S_UserId);
                if (local != null)
                {
                    local.first_name = Txt_FirstName.Text;
                    local.last_name = Txt_LastName.Text;
                    local.address = Txt_location_text.Text;
                    local.working = Txt_work_text.Text;
                    local.about = S_About;
                    local.gender = Txt_gender_text.Text;
                    local.website = Txt_website_text.Text;
                    local.facebook = Txt_facebook_text.Text;
                    local.google = Txt_Google_text.Text;
                    local.twitter = Txt_Twitter_text.Text;
                    local.youtube = Txt_Youtube_text.Text;
                    local.vk = Txt_VK_text.Text;
                    local.instagram = Txt_Instagram_text.Text;
                    local.phone_number = Txt_mobile_text.Text;

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    dbDatabase.Insert_Or_Update_To_MyProfileTable(local);
                    dbDatabase.Dispose();
                }

                if (IMethods.CheckConnectivity())
                {
                    var dictionary = new Dictionary<string, string>();

                    if (!string.IsNullOrEmpty(Txt_FirstName.Text))
                        dictionary.Add("first_name", Txt_FirstName.Text);

                    if (!string.IsNullOrEmpty(Txt_LastName.Text))
                        dictionary.Add("last_name", Txt_LastName.Text);

                    if (!string.IsNullOrEmpty(S_About))
                        dictionary.Add("about", S_About);

                    if (!string.IsNullOrEmpty(Txt_facebook_text.Text))
                        dictionary.Add("facebook", Txt_facebook_text.Text);

                    if (!string.IsNullOrEmpty(Txt_Google_text.Text))
                        dictionary.Add("google", Txt_Google_text.Text);

                    if (!string.IsNullOrEmpty(Txt_Twitter_text.Text))
                        dictionary.Add("twitter", Txt_Twitter_text.Text);

                    if (!string.IsNullOrEmpty(Txt_Youtube_text.Text))
                        dictionary.Add("youtube", Txt_Youtube_text.Text);

                    if (!string.IsNullOrEmpty(Txt_Instagram_text.Text))
                        dictionary.Add("instagram", Txt_Instagram_text.Text);

                    if (!string.IsNullOrEmpty(Txt_VK_text.Text))
                        dictionary.Add("vk", Txt_VK_text.Text);

                    if (!string.IsNullOrEmpty(Txt_website_text.Text))
                        dictionary.Add("website", Txt_website_text.Text);

                    if (!string.IsNullOrEmpty(S_School))
                        dictionary.Add("school", S_School);

                    if (!string.IsNullOrEmpty(Txt_location_text.Text))
                        dictionary.Add("address", Txt_location_text.Text);

                    if (!string.IsNullOrEmpty(Txt_gender_text.Text))
                        dictionary.Add("gender", Txt_gender_text.Text.ToLower());

                    if (!string.IsNullOrEmpty(Txt_mobile_text.Text))
                        dictionary.Add("phone", Txt_mobile_text.Text);
                     
                    var (Api_status, Respond) = await Global.Update_User_Data(dictionary);
                    if (Api_status == 200)
                    {
                        if (Respond is MessageObject result)
                        {
                            if (result.Message.Contains("updated"))
                            {
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();
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
                    EditProfileButtonOnClick();
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
                mAdView?.Destroy();
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == Txt_gender_text.Id)
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(this);

                    arrayAdapter.Add(GetText(Resource.String.Radio_Male));
                    arrayAdapter.Add(GetText(Resource.String.Radio_Female));

                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                switch (itemId)
                {
                    // Radio_Male 
                    case 0:
                        Txt_gender_text.Text = this.GetText(Resource.String.Radio_Male);
                        break;
                    // Radio_Female 
                    case 1:
                        Txt_gender_text.Text = this.GetText(Resource.String.Radio_Female);
                        break; 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}