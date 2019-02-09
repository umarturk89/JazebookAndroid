using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Clans.Fab;
using Com.Theartofdev.Edmodo.Cropper;
using FFImageLoading;
using Java.IO;
using ME.Wangyuwei.Flipshare;
using WoWonder.Activities.Call;
using WoWonder.Activities.DefaultUser;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Story;
using WoWonder.Adapters;
using WoWonder.Functions;
using ActionBar = Android.Support.V7.App.ActionBar;
using Console = System.Console;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace WoWonder.Activities.Tab
{
    [Activity(Theme = "@style/MyTheme")]
    public class Tabbed_Main_Page : AppCompatActivity
    {

        #region Variables  

        private FlipShareView share;

        private TabLayout tabs;
        private ViewPager viewPager;

        private FloatingActionButton FloatingActionButtonView;

        public static Last_Messages_Fragment Last_Messages_Fragment_page;
        public static Last_Stroies_Fragment Last_Stroies_Fragment_page;
        public static Last_Calls_Fragment Last_Calls_Fragment_page;
        public FloatingActionMenu StoryMultiButtons;

        #endregion
         
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "Tabbed_Main_Page" layout resource
                SetContentView(Resource.Layout.Tabbed_Main_Page);

                Toolbar toolBar = FindViewById<Toolbar>(Resource.Id.toolbar);

                if (AppSettings.Show_Title_Username)
                {
                    toolBar.Title = UserDetails.Username;
                }
                else
                {
                    toolBar.Title = AppSettings.Application_Name;
                }

                SetSupportActionBar(toolBar);

                ActionBar ab = SupportActionBar;

                tabs = FindViewById<TabLayout>(Resource.Id.tabs);
                viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);
                StoryMultiButtons = FindViewById<FloatingActionMenu>(Resource.Id.multistroybutton);
                StoryMultiButtons.Visibility = ViewStates.Invisible;

                StoryMultiButtons.GetChildAt(0).Click += OnImage_Button_Click;
                StoryMultiButtons.GetChildAt(1).Click += OnVideo_Button_Click;
                viewPager.PageScrolled += ViewPager_PageScrolled;
                viewPager.PageSelected += ViewPager_OnPageSelected;
                FloatingActionButtonView.Click += FloatingActionButtonView_Click;

                viewPager.OffscreenPageLimit = 3;
                SetUpViewPager(viewPager);
                tabs.SetupWithViewPager(viewPager);
                FloatingActionButtonView.Tag = "LastMessages";

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
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private void OnVideo_Button_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(Intent.ActionPick, MediaStore.Video.Media.ExternalContentUri);
                intent.SetType("video/*");
                StartActivityForResult(intent, 100);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task GetLocationPermissionAsync()
        {
            RequestPermissions(new String[]
            {
                Manifest.Permission.Camera,
            }, 104);
        }

        private async void OnImage_Button_Click(object sender, EventArgs e)
        {
            try
            {
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
                        await GetLocationPermissionAsync();
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

                    await Task.Delay(2000);

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
            catch (Exception exe)
            {
                Console.WriteLine(exe);
            }
        }

        #region Method Add Story

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                //If its from Camera or Gallary
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    if (requestCode == 203 && resultCode == Result.Ok)
                    {
                        CropImage.ActivityResult result = CropImage.GetActivityResult(data);

                        if (result.IsSuccessful)
                        {
                            Android.Net.Uri resultUri = result.Uri;

                            if (!String.IsNullOrEmpty(resultUri.Path))
                            {
                                Intent intent = new Intent(this, typeof(AddStory_Activity));
                                intent.PutExtra("Uri", resultUri.Path);
                                intent.PutExtra("Type", "image");
                                StartActivity(intent);
                            }
                            else
                            {
                                Toast.MakeText(this, this.GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                            }
                        }
                    }
                }
                else if (requestCode == 100 && resultCode == Result.Ok)
                {
                    string FullPath = IMethods.MultiMedia.GetRealVideoPathFromURI(data.Data);
                    if (!string.IsNullOrEmpty(FullPath))
                    {
                        Intent intent = new Intent(this, typeof(AddStory_Activity));
                        intent.PutExtra("Uri", FullPath);
                        intent.PutExtra("Type", "video");
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(this, this.GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private void FloatingActionButtonView_Click(object sender, EventArgs e)
        {
            if (FloatingActionButtonView.Tag.ToString() == "LastMessages")
            {
                StartActivity(new Intent(this, typeof(UserContacts_Activity)));
            }
            else if (FloatingActionButtonView.Tag.ToString() == "Story")
            {

            }
            else if (FloatingActionButtonView.Tag.ToString() == "Call")
            {
                StartActivity(new Intent(this, typeof(AddNewCall_Activity)));
            }
        }

        private void ViewPager_PageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position == 0) // LastMessages
                {
                    if (FloatingActionButtonView.Tag.ToString() != "LastMessages")
                    {
                        FloatingActionButtonView.Tag = "LastMessages";
                        FloatingActionButtonView.SetImageResource(Resource.Drawable.ic_contacts);
                        FloatingActionButtonView.Visibility = ViewStates.Visible;
                        StoryMultiButtons.Visibility = ViewStates.Invisible;
                    }
                }
                else if (position == 1) // Story
                {
                    if (FloatingActionButtonView.Tag.ToString() != "Story")
                    {
                        FloatingActionButtonView.Tag = "Story";
                        FloatingActionButtonView.SetImageResource(Resource.Drawable.ic_story_camera);
                        FloatingActionButtonView.Visibility = ViewStates.Invisible;
                        StoryMultiButtons.Visibility = ViewStates.Visible;
                    }
                }
                else if (position == 2) // Call
                {
                    if (FloatingActionButtonView.Tag.ToString() != "Call")
                    {
                        FloatingActionButtonView.Tag = "Call";
                        FloatingActionButtonView.SetImageResource(Resource.Drawable.ic_phone_user);
                        FloatingActionButtonView.Visibility = ViewStates.Visible;
                        StoryMultiButtons.Visibility = ViewStates.Invisible;
                    }
                }
                else
                {

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ViewPager_OnPageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position == 0)
                {

                }
                else if (position == 1)
                {
                   // Last_Stroies_Fragment_page.GetStory_Api();
                }
                else if (position == 2)
                {

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                Last_Messages_Fragment_page = new Last_Messages_Fragment();
                Last_Stroies_Fragment_page = new Last_Stroies_Fragment();
                Last_Calls_Fragment_page = new Last_Calls_Fragment();

                MainTab_Adapter adapter = new MainTab_Adapter(SupportFragmentManager);
                adapter.AddFragment(Last_Messages_Fragment_page, this.GetText(Resource.String.Lbl_Tab_Chats));
                adapter.AddFragment(Last_Stroies_Fragment_page, this.GetText(Resource.String.Lbl_Tab_Stories));
                if (AppSettings.Enable_Audio_Video_Call)
                {
                    adapter.AddFragment(Last_Calls_Fragment_page, this.GetText(Resource.String.Lbl_Tab_Calls));
                }
                else
                {
                    viewPager.OffscreenPageLimit = 2;
                }

                viewPager.Adapter = adapter;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Main_Menu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.searchMainbutton)
            {
                StartActivity(new Intent(Application.Context, typeof(OnlineSearch_Activity)));
            }
            else if (item.ItemId == Resource.Id.menue_blockList)
            {
                StartActivity(new Intent(Application.Context, typeof(BlockedUsers_Activity)));
            }
            else if (item.ItemId == Resource.Id.menue_Settings)
            {
                StartActivity(new Intent(Application.Context, typeof(Settings_Activity)));
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