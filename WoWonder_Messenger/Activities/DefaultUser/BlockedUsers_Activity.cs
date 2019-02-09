using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using WoWonder.Activities.DialogUserFragment;
using WoWonder.Adapters;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace WoWonder.Activities.DefaultUser
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class BlockedUsers_Activity : AppCompatActivity
    {
        #region Variables Basic

        private RecyclerView BlockedUsersRecyler;
        private LinearLayoutManager mLayoutManager;
        public static BlockedUsers_Adapter mAdapter;

        private LinearLayout BlockedUsers_Empty;
        private TextView Icon_blockedusers;

        public SwipeRefreshLayout swipeRefreshLayout;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "BlockedUsers_Layout" layout resource
                SetContentView(Resource.Layout.BlockedUsers_Layout);

                //Get values
                BlockedUsersRecyler = FindViewById<RecyclerView>(Resource.Id.BlockRecylerview);
                BlockedUsers_Empty = FindViewById<LinearLayout>(Resource.Id.Block_LinerEmpty);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                Icon_blockedusers = FindViewById<TextView>(Resource.Id.blockedusers_icon);

                BlockedUsersRecyler.Visibility = ViewStates.Visible;
                BlockedUsers_Empty.Visibility = ViewStates.Gone;

                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                IMethods.Set_TextViewIcon("2", Icon_blockedusers, "\uf235");
                Icon_blockedusers.SetTextColor(Android.Graphics.Color.ParseColor(AppSettings.MainColor));

                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                ToolBar.Title = this.GetText(Resource.String.Lbl_BlockedUsers);

                SetSupportActionBar(ToolBar);

                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                //Set Adapter
                mLayoutManager = new LinearLayoutManager(this);
                BlockedUsersRecyler.SetLayoutManager(mLayoutManager);
                mAdapter = new BlockedUsers_Adapter(this);
                BlockedUsersRecyler.SetAdapter(mAdapter);

                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;
                Get_BlockedList();

                AdsGoogle.Ad_Interstitial(this);
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
                mAdapter.ItemClick += MAdapterOnItemClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
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
                mAdapter.ItemClick -= MAdapterOnItemClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Get_BlockedList()
        {
            try
            {
                swipeRefreshLayout.Refreshing = true;

                //Get All User Block From Database 
                var dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.Get_Blocked_Users();
                if (localList != null)
                {
                    mAdapter.mBlockedUsers =
                        new ObservableCollection<GetBlockedUsersObject.BlockedUsers>(localList);
                    mAdapter.BindEnd();
                }

                dbDatabase.Dispose();

                Get_BlockedList_API();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void Get_BlockedList_API()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    swipeRefreshLayout.Refreshing = false;
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Short)
                        .Show();
                }
                else
                {
                    var (Api_status, Respond) = await Global.Get_Blocked_Users();
                    if (Api_status == 200)
                    {
                        if (Respond is GetBlockedUsersObject result)
                        {
                            if (result.blocked_users.Length <= 0)
                            {
                                swipeRefreshLayout.Refreshing = false;
                            }

                            var dbDatabase = new SqLiteDatabase();
                            if (mAdapter.mBlockedUsers.Count > 0)
                            {
                                //Bring new users
                                var listnew = result.blocked_users.Where(c =>
                                    !mAdapter.mBlockedUsers.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                                if (listnew.Count > 0)
                                {
                                    //Results differ
                                    Classes.AddRange(mAdapter.mBlockedUsers, listnew);

                                    //Insert Or Replace Just New Data To Database
                                    dbDatabase.Insert_Or_Replace_BlockedUsersTable(
                                        new ObservableCollection<GetBlockedUsersObject.BlockedUsers>(listnew));
                                }
                            }
                            else
                            {
                                mAdapter.mBlockedUsers =
                                    new ObservableCollection<GetBlockedUsersObject.BlockedUsers>(
                                        result.blocked_users);
                                mAdapter.BindEnd();

                                //Insert Or Replace Data To Database
                                dbDatabase.Insert_Or_Replace_BlockedUsersTable(mAdapter.mBlockedUsers);
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

                //Show Empty Page >> 
                //===============================================================
                if (mAdapter.mBlockedUsers.Count > 0)
                {
                    BlockedUsersRecyler.Visibility = ViewStates.Visible;
                    BlockedUsers_Empty.Visibility = ViewStates.Gone;
                }
                else
                {
                    BlockedUsersRecyler.Visibility = ViewStates.Gone;
                    BlockedUsers_Empty.Visibility = ViewStates.Visible;
                }

                swipeRefreshLayout.Refreshing = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Get_BlockedList_API();
            }
        }

        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                mAdapter.Clear();
                Get_BlockedList_API();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MAdapterOnItemClick(object sender, BlockedUsers_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = mAdapter.GetItem(Position);
                    if (item != null)
                    {
                        //Pull up dialog
                        FragmentTransaction transaction = FragmentManager.BeginTransaction();
                        Dialog_BlockUser UserDialog = new Dialog_BlockUser(item.user_id, item);
                        UserDialog.Show(transaction, "dialog fragment");
                        UserDialog._OnBlockUserUpComplete += UserDialogOnOnBlockUserUpComplete;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UserDialogOnOnBlockUserUpComplete(object sender,
            Dialog_BlockUser.OnBlockUserUp_EventArgs onBlockUserUpEventArgs)
        {
            try
            {
                Thread th = new Thread(ActLikeARequest);
                th.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ActLikeARequest()
        {
            int x = Resource.Animation.slide_right;
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