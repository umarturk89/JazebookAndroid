using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Luseen.Autolinklibrary;
using FFImageLoading;
using Newtonsoft.Json;
using WoWonder.Activities.DefaultUser;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;
using Console = System.Console;


namespace WoWonder.Activities
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/TranslucenActivty",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class UserProfile_Activity : AppCompatActivity
    {
        #region Variables Basic

        private TextView Txt_UserName;
        private TextView Txt_Fullname;
        private TextView Txt_Following;
        private TextView Txt_FollowingCount;
        private TextView Txt_Followers;
        private TextView Txt_FollowersCount;

        private AutoLinkTextView Txt_About;

        private AppCompatTextView Txt_gender_icon;
        private TextView Txt_gender_text;
        private AppCompatTextView Txt_location_icon;
        private TextView Txt_location_text;
        private AppCompatTextView Txt_mobile_icon;
        private TextView Txt_mobile_text;

        private AppCompatTextView Txt_website_icon;
        private TextView Txt_website_text;

        private AppCompatTextView Txt_work_icon;
        private TextView Txt_work_text;

        private ImageView Image_Userprofile;
        private ImageView Image_UserCover;

        private CircleButton AddFriendorFollowButton;

        private CircleButton Btn_facebook;
        private CircleButton Btn_instegram;
        private CircleButton Btn_twitter;
        private CircleButton Btn_google;
        private CircleButton Btn_vk;
        private CircleButton Btn_youtube;

        private LinearLayout gender_Liner;
        private LinearLayout location_Liner;
        private LinearLayout mobile_Liner;
        private LinearLayout website_Liner;
        private LinearLayout work_Liner;

        public AdView mAdView;

        private string S_UserId = "";
        private string S_Facebook = "";
        private string S_Google = "";
        private string S_Twitter = "";
        private string S_Linkedin = "";
        private string S_Youtube = "";
        private string S_VK = "";
        private string S_Instagram = "";
        private string S_Can_follow = "";
        private string S_UserType = "";

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "UserProfile_Layout" layout resource
                SetContentView(Resource.Layout.UserProfile_Layout);

                var data = Intent.GetStringExtra("UserId") ?? "Data not available";
                if (data != "Data not available" && !string.IsNullOrEmpty(data))
                {
                    S_UserId = data;
                }

                var UserType = Intent.GetStringExtra("UserType") ?? "Data not available";
                if (UserType != "Data not available" && !string.IsNullOrEmpty(data))
                {
                    S_UserType = UserType;
                }

                Txt_Fullname = FindViewById<TextView>(Resource.Id.Txt_fullname);
                Txt_UserName = FindViewById<TextView>(Resource.Id.username);

                Txt_Followers = FindViewById<TextView>(Resource.Id.Txt_flowersView);
                Txt_FollowersCount = FindViewById<TextView>(Resource.Id.Txt_flowers_count);

                Txt_Following = FindViewById<TextView>(Resource.Id.flowinglabelView);
                Txt_FollowingCount = FindViewById<TextView>(Resource.Id.Txt_flowing_countView);

                Txt_About = FindViewById<AutoLinkTextView>(Resource.Id.Txt_AboutUser);

                Image_Userprofile = FindViewById<ImageView>(Resource.Id.profile_image);
                Image_UserCover = FindViewById<ImageView>(Resource.Id.coverImageView);

                gender_Liner = FindViewById<LinearLayout>(Resource.Id.genderLiner);
                Txt_gender_icon = FindViewById<AppCompatTextView>(Resource.Id.gender_icon);
                Txt_gender_text = FindViewById<TextView>(Resource.Id.gender_text);

                location_Liner = FindViewById<LinearLayout>(Resource.Id.locationLiner);
                Txt_location_icon = FindViewById<AppCompatTextView>(Resource.Id.location_icon);
                Txt_location_text = FindViewById<TextView>(Resource.Id.location_text);

                mobile_Liner = FindViewById<LinearLayout>(Resource.Id.mobileLiner);
                Txt_mobile_icon = FindViewById<AppCompatTextView>(Resource.Id.mobile_icon);
                Txt_mobile_text = FindViewById<TextView>(Resource.Id.mobile_text);

                website_Liner = FindViewById<LinearLayout>(Resource.Id.websiteLiner);
                Txt_website_icon = FindViewById<AppCompatTextView>(Resource.Id.website_icon);
                Txt_website_text = FindViewById<TextView>(Resource.Id.website_text);

                work_Liner = FindViewById<LinearLayout>(Resource.Id.workLiner);
                Txt_work_icon = FindViewById<AppCompatTextView>(Resource.Id.work_icon);
                Txt_work_text = FindViewById<TextView>(Resource.Id.work_text);

                IMethods.Set_TextViewIcon("1", Txt_gender_icon, IonIcons_Fonts.Male);
                Txt_gender_icon.SetTextColor(Android.Graphics.Color.ParseColor("#4693d8"));

                IMethods.Set_TextViewIcon("1", Txt_location_icon, IonIcons_Fonts.Location);
                Txt_location_icon.SetTextColor(Android.Graphics.Color.ParseColor(AppSettings.MainColor));

                IMethods.Set_TextViewIcon("1", Txt_mobile_icon, IonIcons_Fonts.AndroidCall);
                Txt_mobile_icon.SetTextColor(Android.Graphics.Color.ParseColor("#fa6670"));

                IMethods.Set_TextViewIcon("1", Txt_website_icon, IonIcons_Fonts.AndroidGlobe);
                Txt_website_icon.SetTextColor(Android.Graphics.Color.ParseColor("#6b38d1"));

                IMethods.Set_TextViewIcon("1", Txt_work_icon, IonIcons_Fonts.Briefcase);
                Txt_work_icon.SetTextColor(Android.Graphics.Color.ParseColor("#eca72c"));

                AddFriendorFollowButton = FindViewById<CircleButton>(Resource.Id.follow_button);
                AddFriendorFollowButton.Tag = "Add";

                Btn_facebook = FindViewById<CircleButton>(Resource.Id.facebook_button);
                Btn_facebook.Tag = "facebook";

                Btn_instegram = FindViewById<CircleButton>(Resource.Id.instegram_button);
                Btn_instegram.Tag = "instegram";

                Btn_twitter = FindViewById<CircleButton>(Resource.Id.twitter_button);
                Btn_twitter.Tag = "twitter";

                Btn_google = FindViewById<CircleButton>(Resource.Id.google_button);
                Btn_google.Tag = "google";

                Btn_vk = FindViewById<CircleButton>(Resource.Id.vk_button);
                Btn_vk.Tag = "vk";

                Btn_youtube = FindViewById<CircleButton>(Resource.Id.youtube_button);
                Btn_youtube.Tag = "youtube";

                Txt_About.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag,AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                Txt_About.SetPhoneModeColor(ContextCompat.GetColor(this, Resource.Color.left_ModePhone_color));
                Txt_About.SetEmailModeColor(ContextCompat.GetColor(this, Resource.Color.left_ModeEmail_color));
                Txt_About.SetHashtagModeColor(ContextCompat.GetColor(this, Resource.Color.left_ModeHashtag_color));
                Txt_About.SetUrlModeColor(ContextCompat.GetColor(this, Resource.Color.left_ModeUrl_color));
                Txt_About.SetMentionModeColor(ContextCompat.GetColor(this, Resource.Color.left_ModeMention_color));

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

                Get_UserProfileData_Local();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TxtAboutOnAutoLinkOnClick(object sender, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                var typetext = IMethods.Fun_String.Check_Regex(autoLinkOnClickEventArgs.P1);
                if (typetext == "Email")
                {
                    IMethods.IApp.SendEmail(this, autoLinkOnClickEventArgs.P1);
                }
                else if (typetext == "Website")
                {
                    var url = autoLinkOnClickEventArgs.P1;
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1;
                    }

                    IMethods.IApp.OpenWebsiteUrl(this, url);
                }
                else if (typetext == "Hashtag")
                {
                   
                }
                else if (typetext == "Mention")
                {
                    var intent = new Intent(Application.Context, typeof(OnlineSearch_Activity));
                    intent.PutExtra("Key", autoLinkOnClickEventArgs.P1.Replace("@", ""));
                    this.StartActivity(intent);
                }
                else if (typetext == "Number")
                {
                    IMethods.IApp.SaveContacts(this, autoLinkOnClickEventArgs.P1, "", "2");
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
                AddFriendorFollowButton.Click += AddFriendorFollowButton_Click;
                Btn_facebook.Click += Btn_facebook_OnClick;
                Btn_instegram.Click += Btn_instegram_OnClick;
                Btn_twitter.Click += Btn_twitter_OnClick;
                Btn_google.Click += Btn_google_OnClick;
                Btn_vk.Click += Btn_vk_OnClick;
                Btn_youtube.Click += Btn_youtube_OnClick;
                Txt_About.AutoLinkOnClick += TxtAboutOnAutoLinkOnClick;

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
                AddFriendorFollowButton.Click -= AddFriendorFollowButton_Click;
                Btn_facebook.Click -= Btn_facebook_OnClick;
                Btn_instegram.Click -= Btn_instegram_OnClick;
                Btn_twitter.Click -= Btn_twitter_OnClick;
                Btn_google.Click -= Btn_google_OnClick;
                Btn_vk.Click -= Btn_vk_OnClick;
                Btn_youtube.Click -= Btn_youtube_OnClick;
                Txt_About.AutoLinkOnClick -= TxtAboutOnAutoLinkOnClick;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #region Get Data User

        //Get Data User From Database 
        public void Get_UserProfileData_Local()
        {
            try
            {
                switch (S_UserType)
                {
                    case "Chat_User":
                        loadData_Item_ChatUser();
                        break;
                    case "Chat_UserContacts":
                        loadData_Item_ChatUserContacts();
                        break;
                    case "Chat_SearchUser":
                        loadData_Item_ChatSearchUser();
                        break;
                    default:
                        break;
                }

                Get_ProfileData_Api();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Get_ProfileData_Api();
            }
        }

        public void loadData_Item_ChatUser()
        {
            try
            {
                var local = JsonConvert.DeserializeObject<Classes.Get_Users_List_Object.User>(Intent.GetStringExtra("UserItem"));
                if (local != null)
                {
                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    GetUserDataObject.User_Data user = dbDatabase.Get_UserProfile(local.UserId);
                    if (user != null)
                    {
                        LoadDataUser(user); 
                    } 
                    else
                    {
                        Classes.UserContacts.User contact = dbDatabase.Get_DataOneUser(local.UserId);
                        if (contact != null)
                        {
                            //Cover
                            var CoverSplit = contact.Cover.Split('/').Last();
                            var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                            if (getImage_Cover != "File Dont Exists")
                            {
                                ImageCacheLoader.LoadImage(contact.Cover, Image_UserCover, false, false);
                              
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, contact.Cover);
                                
                                ImageCacheLoader.LoadImage(contact.Cover, Image_UserCover, false, false);
                            }

                            //profile_picture
                            var AvatarSplit = contact.Avatar.Split('/').Last();
                            var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                            if (getImage_Avatar != "File Dont Exists")
                            {
                               
                                ImageCacheLoader.LoadImage(contact.Avatar, Image_Userprofile, false, true);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, contact.Avatar);
                               
                                ImageCacheLoader.LoadImage(contact.Avatar, Image_Userprofile, false, true);
                            }

                            if (IMethods.Fun_String.StringNullRemover(contact.About) != "Empty")
                                Txt_About.SetAutoLinkText(IMethods.Fun_String.DecodeString(contact.About));
                            else
                                Txt_About.SetAutoLinkText(this.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.Application_Name);

                            string name = IMethods.Fun_String.DecodeString(local.Name);
                            Txt_Fullname.Text = name;
                            Txt_UserName.Text = "@" + local.Username;

                            var following = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(contact.Details?.following_count));
                            var followers = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(contact.Details?.followers_count));

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

                            if (contact.IsFollowing == 1) // My Friend
                            {
                                AddFriendorFollowButton.Visibility = ViewStates.Visible;
                                AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                                AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                                AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                                AddFriendorFollowButton.Tag = "friends";
                            }
                            else if (contact.IsFollowing == 2) // Request
                            {
                                AddFriendorFollowButton.Visibility = ViewStates.Visible;
                                AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                                AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_requestAdd);
                                AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                                AddFriendorFollowButton.Tag = "request";
                            }
                            else if (contact.IsFollowing == 0) //Not Friend
                            {
                                AddFriendorFollowButton.Visibility = ViewStates.Visible;

                                AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#444444"));
                                AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_add);
                                AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#ffffff"));
                                AddFriendorFollowButton.Tag = "Add";
                            }

                            switch (contact.Gender.ToLower())
                            {
                                case "male":
                                    Txt_gender_text.Text = this.GetText(Resource.String.Radio_Male);
                                    break;
                                case "female":
                                    Txt_gender_text.Text = this.GetText(Resource.String.Radio_Female);
                                    break;
                                default:
                                    Txt_gender_text.Text = contact.Gender;
                                    break;
                            }

                            if (IMethods.Fun_String.StringNullRemover(contact.Address) != "Empty")
                            {
                                location_Liner.Visibility = ViewStates.Visible;
                                Txt_location_text.Text = contact.Address;
                            }
                            else
                            {
                                location_Liner.Visibility = ViewStates.Gone;
                            }

                            if (IMethods.Fun_String.StringNullRemover(contact.PhoneNumber) != "Empty")
                            {
                                mobile_Liner.Visibility = ViewStates.Visible;
                                Txt_mobile_text.Text = contact.PhoneNumber;
                            }
                            else
                            {
                                mobile_Liner.Visibility = ViewStates.Gone;
                            }

                            if (IMethods.Fun_String.StringNullRemover(contact.Website) != "Empty")
                            {
                                website_Liner.Visibility = ViewStates.Visible;
                                Txt_website_text.Text = contact.Website;
                            }
                            else
                            {
                                website_Liner.Visibility = ViewStates.Gone;
                            }

                            if (IMethods.Fun_String.StringNullRemover(contact.Working) != "Empty")
                            {
                                work_Liner.Visibility = ViewStates.Visible;
                                Txt_work_text.Text = contact.Working;
                            }
                            else
                            {
                                work_Liner.Visibility = ViewStates.Gone;
                            }

                            S_Facebook = IMethods.Fun_String.StringNullRemover(contact.Facebook);
                            S_Google = IMethods.Fun_String.StringNullRemover(contact.Google);
                            S_Twitter = IMethods.Fun_String.StringNullRemover(contact.Twitter);
                            S_Youtube = IMethods.Fun_String.StringNullRemover(contact.Youtube);
                            S_VK = IMethods.Fun_String.StringNullRemover(contact.Vk);
                            S_Instagram = IMethods.Fun_String.StringNullRemover(contact.Instagram);

                            if (S_Facebook == "Empty")
                            {
                                Btn_facebook.Enabled = false;
                                Btn_facebook.SetColor(Color.ParseColor("#8c8a8a"));
                            }
                            else
                            {
                                Btn_facebook.Enabled = true;
                            }

                            if (S_Google == "Empty")
                            {
                                Btn_google.Enabled = false;
                                Btn_google.SetColor(Color.ParseColor("#8c8a8a"));
                            }
                            else
                            {
                                Btn_google.Enabled = true;
                            }

                            if (S_Twitter == "Empty")
                            {
                                Btn_twitter.Enabled = false;
                                Btn_twitter.SetColor(Color.ParseColor("#8c8a8a"));
                            }
                            else
                            {
                                Btn_twitter.Enabled = true;
                            }

                            if (S_Youtube == "Empty")
                            {
                                Btn_youtube.Enabled = false;
                                Btn_youtube.SetColor(Color.ParseColor("#8c8a8a"));
                            }
                            else
                            {
                                Btn_youtube.Enabled = true;
                            }

                            if (S_VK == "Empty")
                            {
                                Btn_vk.Enabled = false;
                                Btn_vk.SetColor(Color.ParseColor("#8c8a8a"));
                            }
                            else
                            {
                                Btn_vk.Enabled = true;
                            }

                            if (S_Instagram == "Empty")
                            {
                                Btn_instegram.Enabled = false;
                                Btn_instegram.SetColor(Color.ParseColor("#8c8a8a"));
                            }
                            else
                            {
                                Btn_instegram.Enabled = true;
                            }
                        }
                        else
                        {
                            //Cover
                            var CoverSplit = local.CoverPicture.Split('/').Last();
                            var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                            if (getImage_Cover != "File Dont Exists")
                            {
                                ImageCacheLoader.LoadImage(getImage_Cover, Image_UserCover, false, false);
                              
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, local.CoverPicture);
                              
                                ImageCacheLoader.LoadImage(local.CoverPicture, Image_UserCover, false, false);
                            }

                            //profile_picture
                            var AvatarSplit = local.ProfilePicture.Split('/').Last();
                            var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                            if (getImage_Avatar != "File Dont Exists")
                            {
                              
                                ImageCacheLoader.LoadImage(getImage_Avatar, Image_Userprofile, false, true);
                            }
                            else
                            {
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, local.ProfilePicture);
                             
                                ImageCacheLoader.LoadImage(local.ProfilePicture, Image_Userprofile, false, true);
                            }

                            string name = IMethods.Fun_String.DecodeString(local.Name);
                            Txt_Fullname.Text = name;
                            Txt_UserName.Text = "@" + local.Username;

                            if (AppSettings.ConnectivitySystem == "1")
                            {
                                Txt_Following.Visibility = ViewStates.Visible;
                                Txt_FollowingCount.Visibility = ViewStates.Visible;

                                Txt_Followers.Visibility = ViewStates.Visible;
                                Txt_FollowersCount.Visibility = ViewStates.Visible;

                                Txt_Following.Text = this.GetText(Resource.String.Lbl_Following);
                                Txt_FollowingCount.Text = "";

                                Txt_Followers.Text = this.GetText(Resource.String.Lbl_Followers);
                                Txt_FollowersCount.Text = "";
                            }
                            else
                            {
                                Txt_Following.Visibility = ViewStates.Visible;
                                Txt_FollowingCount.Visibility = ViewStates.Visible;

                                Txt_Followers.Visibility = ViewStates.Gone;
                                Txt_FollowersCount.Visibility = ViewStates.Gone;

                                Txt_Following.Text = this.GetText(Resource.String.Lbl_Friends);
                                Txt_FollowingCount.Text = "";
                            }

                            location_Liner.Visibility = ViewStates.Gone;
                            mobile_Liner.Visibility = ViewStates.Gone;
                            website_Liner.Visibility = ViewStates.Gone;
                            work_Liner.Visibility = ViewStates.Gone;
                        } 
                    }

                    dbDatabase.Dispose();  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void loadData_Item_ChatUserContacts()
        {
            try
            { 
                var local = JsonConvert.DeserializeObject<Classes.UserContacts.User>(Intent.GetStringExtra("UserItem"));
                if (local != null)
                {
                    //Cover
                    var CoverSplit = local.Cover.Split('/').Last();
                    var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageCacheLoader.LoadImage(local.Cover, Image_UserCover, false, false);
                       
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, local.Cover);
                    
                        ImageCacheLoader.LoadImage(local.Cover, Image_UserCover, false, false);
                    }

                    //profile_picture
                    var AvatarSplit = local.Avatar.Split('/').Last();
                    var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                      
                        ImageCacheLoader.LoadImage(local.Avatar, Image_Userprofile, false, true);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, local.Avatar);
                      
                        ImageCacheLoader.LoadImage(local.Avatar, Image_Userprofile, false, true);
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.About) != "Empty")
                        Txt_About.SetAutoLinkText(IMethods.Fun_String.DecodeString(local.About));
                    else
                        Txt_About.SetAutoLinkText(this.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.Application_Name);

                    string name = IMethods.Fun_String.DecodeString(local.Name);
                    Txt_Fullname.Text = name;
                    Txt_UserName.Text = "@" + local.Username;

                    var following = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(local.Details?.following_count));
                    var followers = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(local.Details?.followers_count));

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

                    AddFriendorFollowButton.Visibility = ViewStates.Visible;
                    AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                    AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                    AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                    AddFriendorFollowButton.Tag = "friends";

                    switch (local.Gender.ToLower())
                    {
                        case "male":
                            Txt_gender_text.Text = this.GetText(Resource.String.Radio_Male);
                            break;
                        case "female":
                            Txt_gender_text.Text = this.GetText(Resource.String.Radio_Female);
                            break;
                        default:
                            Txt_gender_text.Text = local.Gender;
                            break;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.Address) != "Empty")
                    {
                        location_Liner.Visibility = ViewStates.Visible;
                        Txt_location_text.Text = local.Address;
                    }
                    else
                    {
                        location_Liner.Visibility = ViewStates.Gone;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.PhoneNumber) != "Empty")
                    {
                        mobile_Liner.Visibility = ViewStates.Visible;
                        Txt_mobile_text.Text = local.PhoneNumber;
                    }
                    else
                    {
                        mobile_Liner.Visibility = ViewStates.Gone;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.Website) != "Empty")
                    {
                        website_Liner.Visibility = ViewStates.Visible;
                        Txt_website_text.Text = local.Website;
                    }
                    else
                    {
                        website_Liner.Visibility = ViewStates.Gone;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.Working) != "Empty")
                    {
                        work_Liner.Visibility = ViewStates.Visible;
                        Txt_work_text.Text = local.Working;
                    }
                    else
                    {
                        work_Liner.Visibility = ViewStates.Gone;
                    }

                    S_Facebook = IMethods.Fun_String.StringNullRemover(local.Facebook);
                    S_Google = IMethods.Fun_String.StringNullRemover(local.Google);
                    S_Twitter = IMethods.Fun_String.StringNullRemover(local.Twitter);
                    S_Youtube = IMethods.Fun_String.StringNullRemover(local.Youtube);
                    S_VK = IMethods.Fun_String.StringNullRemover(local.Vk);
                    S_Instagram = IMethods.Fun_String.StringNullRemover(local.Instagram);

                    if (S_Facebook == "Empty")
                    {
                        Btn_facebook.Enabled = false;
                        Btn_facebook.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_facebook.Enabled = true;
                    }

                    if (S_Google == "Empty")
                    {
                        Btn_google.Enabled = false;
                        Btn_google.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_google.Enabled = true;
                    }

                    if (S_Twitter == "Empty")
                    {
                        Btn_twitter.Enabled = false;
                        Btn_twitter.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_twitter.Enabled = true;
                    }

                    if (S_Youtube == "Empty")
                    {
                        Btn_youtube.Enabled = false;
                        Btn_youtube.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_youtube.Enabled = true;
                    }

                    if (S_VK == "Empty")
                    {
                        Btn_vk.Enabled = false;
                        Btn_vk.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_vk.Enabled = true;
                    }

                    if (S_Instagram == "Empty")
                    {
                        Btn_instegram.Enabled = false;
                        Btn_instegram.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_instegram.Enabled = true;
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void loadData_Item_ChatSearchUser()
        {
            try
            {
                var local = JsonConvert.DeserializeObject<GetSearchObject.User>(Intent.GetStringExtra("UserItem"));
                if (local != null)
                {
                    //Cover
                    var CoverSplit = local.Cover.Split('/').Last();
                    var getImage_Cover = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, CoverSplit);
                    if (getImage_Cover != "File Dont Exists")
                    {
                        ImageCacheLoader.LoadImage(local.Cover, Image_UserCover, false, false);
                      
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, local.Cover);
                    
                        ImageCacheLoader.LoadImage(local.Cover, Image_UserCover, false, false);
                    }

                    //profile_picture
                    var AvatarSplit = local.Avatar.Split('/').Last();
                    var getImage_Avatar = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                    if (getImage_Avatar != "File Dont Exists")
                    {
                      
                        ImageCacheLoader.LoadImage(local.Avatar, Image_Userprofile, false, true);
                    }
                    else
                    {
                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, local.Avatar);
                      
                        ImageCacheLoader.LoadImage(local.Avatar, Image_Userprofile, false, true);
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.About) != "Empty")
                        Txt_About.SetAutoLinkText(IMethods.Fun_String.DecodeString(local.About));
                    else
                        Txt_About.SetAutoLinkText(this.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.Application_Name);

                    string name = IMethods.Fun_String.DecodeString(local.Name);
                    Txt_Fullname.Text = name;
                    Txt_UserName.Text = "@" + local.Username;

                    var following = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(local.Details?.following_count));
                    var followers = IMethods.Fun_String.FormatPriceValue(Convert.ToInt32(local.Details?.followers_count));

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

                    if (local.is_following == "1" || local.is_following == "yes" || local.is_following == "Yes") // My Friend
                    {
                        AddFriendorFollowButton.Visibility = ViewStates.Visible;
                        AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                        AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                        AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                        AddFriendorFollowButton.Tag = "friends";
                    }
                    else if (local.is_following == "2" || local.is_following == "Request" || local.is_following == "request") // Request
                    {
                        AddFriendorFollowButton.Visibility = ViewStates.Visible;
                        AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                        AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_requestAdd);
                        AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                        AddFriendorFollowButton.Tag = "request";
                    }
                    else if (local.is_following == "0" || local.is_following == "no" || local.is_following == "No") //Not Friend
                    {
                        AddFriendorFollowButton.Visibility = ViewStates.Visible;

                        AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#444444"));
                        AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_add);
                        AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#ffffff"));
                        AddFriendorFollowButton.Tag = "Add";
                    }

                    switch (local.Gender.ToLower())
                    {
                        case "male":
                            Txt_gender_text.Text = this.GetText(Resource.String.Radio_Male);
                            break;
                        case "female":
                            Txt_gender_text.Text = this.GetText(Resource.String.Radio_Female);
                            break;
                        default:
                            Txt_gender_text.Text = local.Gender;
                            break;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.Address) != "Empty")
                    {
                        location_Liner.Visibility = ViewStates.Visible;
                        Txt_location_text.Text = local.Address;
                    }
                    else
                    {
                        location_Liner.Visibility = ViewStates.Gone;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.PhoneNumber) != "Empty")
                    {
                        mobile_Liner.Visibility = ViewStates.Visible;
                        Txt_mobile_text.Text = local.PhoneNumber;
                    }
                    else
                    {
                        mobile_Liner.Visibility = ViewStates.Gone;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.Website) != "Empty")
                    {
                        website_Liner.Visibility = ViewStates.Visible;
                        Txt_website_text.Text = local.Website;
                    }
                    else
                    {
                        website_Liner.Visibility = ViewStates.Gone;
                    }

                    if (IMethods.Fun_String.StringNullRemover(local.Working) != "Empty")
                    {
                        work_Liner.Visibility = ViewStates.Visible;
                        Txt_work_text.Text = local.Working;
                    }
                    else
                    {
                        work_Liner.Visibility = ViewStates.Gone;
                    }
                     
                    S_Facebook = IMethods.Fun_String.StringNullRemover(local.Facebook);
                    S_Google = IMethods.Fun_String.StringNullRemover(local.Google);
                    S_Twitter = IMethods.Fun_String.StringNullRemover(local.Twitter);
                    S_Youtube = IMethods.Fun_String.StringNullRemover(local.Youtube);
                    S_VK = IMethods.Fun_String.StringNullRemover(local.Vk);
                    S_Instagram = IMethods.Fun_String.StringNullRemover(local.Instagram);

                    if (S_Facebook == "Empty")
                    {
                        Btn_facebook.Enabled = false;
                        Btn_facebook.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_facebook.Enabled = true;
                    }

                    if (S_Google == "Empty")
                    {
                        Btn_google.Enabled = false;
                        Btn_google.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_google.Enabled = true;
                    }

                    if (S_Twitter == "Empty")
                    {
                        Btn_twitter.Enabled = false;
                        Btn_twitter.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_twitter.Enabled = true;
                    }

                    if (S_Youtube == "Empty")
                    {
                        Btn_youtube.Enabled = false;
                        Btn_youtube.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_youtube.Enabled = true;
                    }

                    if (S_VK == "Empty")
                    {
                        Btn_vk.Enabled = false;
                        Btn_vk.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_vk.Enabled = true;
                    }

                    if (S_Instagram == "Empty")
                    {
                        Btn_instegram.Enabled = false;
                        Btn_instegram.SetColor(Color.ParseColor("#8c8a8a"));
                    }
                    else
                    {
                        Btn_instegram.Enabled = true;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        //Get Data Profile API
        public async void Get_ProfileData_Api()
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
                                //Insert Or Update All data User From Database 
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
                Get_ProfileData_Api();
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

                if (IMethods.Fun_String.StringNullRemover(data.about) != "Empty")
                    Txt_About.SetAutoLinkText(IMethods.Fun_String.DecodeString(data.about));
                else
                    Txt_About.SetAutoLinkText(this.GetString(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.Application_Name);
                 
                Txt_Fullname.Text = data.name;
                Txt_UserName.Text = "@" + data.username;

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


                if (data.is_following == 1) // My Friend
                {
                    AddFriendorFollowButton.Visibility = ViewStates.Visible;
                    AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                    AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                    AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                    AddFriendorFollowButton.Tag = "friends";
                }
                else if (data.is_following == 2) // Request
                {
                    AddFriendorFollowButton.Visibility = ViewStates.Visible;
                    AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                    AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_requestAdd);
                    AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                    AddFriendorFollowButton.Tag = "request";
                }
                else if (data.is_following == 0) //Not Friend
                {
                    AddFriendorFollowButton.Visibility = ViewStates.Visible;

                    AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#444444"));
                    AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_add);
                    AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#ffffff"));
                    AddFriendorFollowButton.Tag = "Add";
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

                if (IMethods.Fun_String.StringNullRemover(data.address) != "Empty")
                {
                    location_Liner.Visibility = ViewStates.Visible;
                    Txt_location_text.Text = data.address;
                }
                else
                {
                    location_Liner.Visibility = ViewStates.Gone;
                }

                if (IMethods.Fun_String.StringNullRemover(data.phone_number) != "Empty")
                {
                    mobile_Liner.Visibility = ViewStates.Visible;
                    Txt_mobile_text.Text = data.phone_number;
                }
                else
                {
                    mobile_Liner.Visibility = ViewStates.Gone;
                }

                if (IMethods.Fun_String.StringNullRemover(data.website) != "Empty")
                {
                    website_Liner.Visibility = ViewStates.Visible;
                    Txt_website_text.Text = data.website;
                }
                else
                {
                    website_Liner.Visibility = ViewStates.Gone;
                }

                if (IMethods.Fun_String.StringNullRemover(data.working) != "Empty")
                {
                    work_Liner.Visibility = ViewStates.Visible;
                    Txt_work_text.Text = data.working;
                }
                else
                {
                    work_Liner.Visibility = ViewStates.Gone;
                }
                

                S_Facebook = IMethods.Fun_String.StringNullRemover(data.facebook);
                S_Google = IMethods.Fun_String.StringNullRemover(data.google);
                S_Twitter = IMethods.Fun_String.StringNullRemover(data.twitter);
                S_Youtube = IMethods.Fun_String.StringNullRemover(data.youtube);
                S_VK = IMethods.Fun_String.StringNullRemover(data.vk);
                S_Instagram = IMethods.Fun_String.StringNullRemover(data.instagram);

                if (S_Facebook == "Empty")
                {
                    Btn_facebook.Enabled = false;
                    Btn_facebook.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    Btn_facebook.Enabled = true;
                }

                if (S_Google == "Empty")
                {
                    Btn_google.Enabled = false;
                    Btn_google.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    Btn_google.Enabled = true;
                }

                if (S_Twitter == "Empty")
                {
                    Btn_twitter.Enabled = false;
                    Btn_twitter.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    Btn_twitter.Enabled = true;
                }

                if (S_Youtube == "Empty")
                {
                    Btn_youtube.Enabled = false;
                    Btn_youtube.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    Btn_youtube.Enabled = true;
                }

                if (S_VK == "Empty")
                {
                    Btn_vk.Enabled = false;
                    Btn_vk.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    Btn_vk.Enabled = true;
                }

                if (S_Instagram == "Empty")
                {
                    Btn_instegram.Enabled = false;
                    Btn_instegram.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    Btn_instegram.Enabled = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private void AddFriendorFollowButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (AddFriendorFollowButton.Tag.ToString() == "Add") //(is_following == "0") >> Not Friend
                    {
                        AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                        AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                        AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                        AddFriendorFollowButton.Tag = "friends";
                    }
                    else if (AddFriendorFollowButton.Tag.ToString() == "request") //(is_following == "2") >> Request
                    {
                        AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#efefef"));
                        AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                        AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#444444"));
                        AddFriendorFollowButton.Tag = "Add";
                    }
                    else //(is_following == "1") >> Friend
                    {
                        AddFriendorFollowButton.SetColor(Android.Graphics.Color.ParseColor("#444444"));
                        AddFriendorFollowButton.SetImageResource(Resource.Drawable.ic_add);
                        AddFriendorFollowButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#ffffff"));

                        AddFriendorFollowButton.Tag = "Add";
                    }

                    var result = Global.Follow_User(S_UserId).ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(this, this.GetText(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Event social media

        private void Btn_facebook_OnClick(object sender, EventArgs e)
        {
            try
            {
                IMethods.IApp.OpenWebsiteUrl(this, "https://www.facebook.com/" + S_Facebook);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Btn_instegram_OnClick(object sender, EventArgs e)
        {
            try
            {
                IMethods.IApp.OpenWebsiteUrl(this, "https://www.instegram.com/" + S_Instagram);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Btn_twitter_OnClick(object sender, EventArgs e)
        {
            try
            {
                IMethods.IApp.OpenWebsiteUrl(this, "https://twitter.com/" + S_Twitter);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Btn_google_OnClick(object sender, EventArgs e)
        {
            try
            {
                IMethods.IApp.OpenWebsiteUrl(this, "https://plus.google.com/u/0/" + S_Google);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Btn_vk_OnClick(object sender, EventArgs e)
        {
            try
            {
                IMethods.IApp.OpenWebsiteUrl(this, "https://vk.com/" + S_VK);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Btn_youtube_OnClick(object sender, EventArgs e)
        {
            try
            {
                IMethods.IApp.OpenWebsiteUrl(this, "https://www.youtube.com/channel/" + S_Youtube);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
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