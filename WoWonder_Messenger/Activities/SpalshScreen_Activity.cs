using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Util;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API;

namespace WoWonder.Activities
{
    [Activity(MainLauncher = true, Theme = "@style/SpalchScreenTheme", NoHistory = true,ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Locale)]
    public class SpalshScreen_Activity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            { 
                base.OnCreate(savedInstanceState);

                // Create your application here
                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = (int)uiOptions;

                newUiOptions |= (int)SystemUiFlags.Fullscreen;
                newUiOptions |= (int)SystemUiFlags.HideNavigation;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if ((int) Build.VERSION.SdkInt < 23)
            {
                Task startupWork = new Task(SimulateStartup);
                startupWork.Start();
            }
            else
            {
                if ((CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted) &&
                    (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted))
                {
                    Task startupWork = new Task(SimulateStartup);
                    startupWork.Start();
                }
                else
                {
                    RequestPermissions(new string[]
                    {
                        Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage,
                    }, 101);
                }
            }
        }

        public void SimulateStartup()
        {
            try
            {
                

                FirstRunExcute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 101)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    FirstRunExcute();
                }
                else
                {
                    Toast.MakeText(this, this.GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    Finish();
                }
            }
        }

        public void FirstRunExcute()
        {
            try
            {
                var AppSettings = new AppSettings();
                var client = new Client(AppSettings.TripleDesAppServiceProvider);

                if (AppSettings.Show_ADMOB_Banner || AppSettings.Show_ADMOB_Interstitial || AppSettings.Show_ADMOB_RewardVideo)
                    MobileAds.Initialize(this, AppSettings.Ad_App_ID);
                 
                ImageCacheLoader.InitImageLoader(this);

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var result = dbDatabase.Get_data_Login_Credentials();
                if (result != null)
                {
                    Task.Run(() =>
                    {
                        dbDatabase = new SqLiteDatabase();
                        Classes.UserList = new ObservableCollection<Classes.Get_Users_List_Object.User>();
                        var list = dbDatabase.Get_LastUsersChat_List();
                        Classes.UserList = new ObservableCollection<Classes.Get_Users_List_Object.User>(list.OrderByDescending(a => a.LastMessage.Time));
                    });
                     
                    UserDetails.access_token = result.access_token;
                    UserDetails.User_id = result.UserID;
                    UserDetails.Username = result.Username;
                    UserDetails.Password = result.Password;
                    UserDetails.Full_name = result.Username;
                    try
                    {
                        if (AppSettings.Lang != "")
                        {
                            if (AppSettings.Lang == "ar")
                            {
                                AppSettings.FlowDirection_RightToLeft = true;
                            }

                            Configuration config = new Configuration();
                       
                            config.Locale = Locale.Default = new Locale(AppSettings.Lang);

                            Resources.UpdateConfiguration(config, null);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    var data = Intent.GetStringExtra("UserID") ?? "Data not available";
                    if (data != "Data not available" && !string.IsNullOrEmpty(data))
                    {
                        Intent intent = new Intent(Application.Context, typeof(ChatWindow_Activity));
                        intent.PutExtra("UserID", data); // to_id
                        intent.PutExtra("Notfi", "Notfi");
                        intent.PutExtra("App", "Timeline");
                        intent.PutExtra("Name", Intent.GetStringExtra("Name"));
                        intent.PutExtra("Username", Intent.GetStringExtra("Username"));
                        intent.PutExtra("Time", Intent.GetStringExtra("Time"));
                        intent.PutExtra("LastSeen", Intent.GetStringExtra("LastSeen"));
                        intent.PutExtra("About", Intent.GetStringExtra("About"));
                        intent.PutExtra("Address", Intent.GetStringExtra("Address"));
                        intent.PutExtra("Phone", Intent.GetStringExtra("Phone"));
                        intent.PutExtra("Website", Intent.GetStringExtra("Website"));
                        intent.PutExtra("Working", Intent.GetStringExtra("Working"));
                        StartActivity(intent);
                    }
                    else
                    {
                        StartActivity(new Intent(Application.Context, typeof(Tabbed_Main_Page)));
                    } 
                }
                else
                {
                    if (AppSettings.Lang != "")
                    {
                        if (AppSettings.Lang == "ar")
                        {
                            AppSettings.FlowDirection_RightToLeft = true;
                        }

                        Configuration config = new Configuration();
                        
                        config.Locale = Locale.Default = new Locale(AppSettings.Lang);

                        Resources.UpdateConfiguration(config, null);
                    }

                    var data = Intent.GetStringExtra("UserID") ?? "Data not available";
                    if (data != "Data not available" && !string.IsNullOrEmpty(data))
                    {
                        Intent intent = new Intent(Application.Context, typeof(ChatWindow_Activity));
                        intent.PutExtra("UserID", data); // to_id
                        intent.PutExtra("Notfi", "Notfi");
                        intent.PutExtra("App", "Timeline");
                        intent.PutExtra("Name", Intent.GetStringExtra("Name"));
                        intent.PutExtra("Username", Intent.GetStringExtra("Username"));
                        intent.PutExtra("Time", Intent.GetStringExtra("Time"));
                        intent.PutExtra("LastSeen", Intent.GetStringExtra("LastSeen"));
                        intent.PutExtra("About", Intent.GetStringExtra("About"));
                        intent.PutExtra("Address", Intent.GetStringExtra("Address"));
                        intent.PutExtra("Phone", Intent.GetStringExtra("Phone"));
                        intent.PutExtra("Website", Intent.GetStringExtra("Website"));
                        intent.PutExtra("Working", Intent.GetStringExtra("Working"));
                        StartActivity(intent);
                    }
                    else
                    {
                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
               
                Console.WriteLine(e);
            }
        }
    } 
}