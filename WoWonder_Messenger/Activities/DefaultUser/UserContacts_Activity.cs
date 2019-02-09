using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Newtonsoft.Json;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Adapters;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace WoWonder.Activities.DefaultUser
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class UserContacts_Activity : AppCompatActivity
    {
        #region Variables Basic

        public RecyclerView ContactsRecyler;
        private LinearLayoutManager ContactsLayoutManager;
        public static UserContact_Adapter ContactAdapter;
        public SearchView _SearchView;
        private LinearLayout usercontacts_Empty;
        private Button Btn_SearchRandom;
        private TextView Icon_UserContacts;
        public SwipeRefreshLayout swipeRefreshLayout;

        public ObservableCollection<Classes.UserContacts.User> UserContactsList = new ObservableCollection<Classes.UserContacts.User>();

        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        public bool ShowSnackbar = true;
        public bool ShowSnackbarNoMore = true;

        public string TimerWork = "Working";

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "UserContacts_Layout" layout resource
                SetContentView(Resource.Layout.UserContacts_Layout);

                //Get values
                ContactsRecyler = FindViewById<RecyclerView>(Resource.Id.usercontactsRecyler);
                usercontacts_Empty = (LinearLayout) FindViewById(Resource.Id.usercontacts_LinerEmpty);
                Btn_SearchRandom = (Button) FindViewById(Resource.Id.SearchRandom_Button);
                Icon_UserContacts = FindViewById<AppCompatTextView>(Resource.Id.usercontacts_icon);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);

                //Set ToolBar
                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                ToolBar.Title = this.GetText(Resource.String.Lbl_MyContacts);
                SetSupportActionBar(ToolBar);

                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);


                IMethods.Set_TextViewIcon("1", Icon_UserContacts, IonIcons_Fonts.IosPeopleOutline);
                Icon_UserContacts.SetTextColor(Android.Graphics.Color.ParseColor(AppSettings.MainColor));
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,Android.Resource.Color.HoloRedLight);

                ContactsLayoutManager = new LinearLayoutManager(this);
                ContactsRecyler.SetLayoutManager(ContactsLayoutManager);

                ContactsRecyler.HasFixedSize = true;
                ContactsRecyler.SetItemViewCacheSize(10);
                ContactsRecyler.GetLayoutManager().ItemPrefetchEnabled = true;
                ContactsRecyler.DrawingCacheEnabled = true;
                ContactsRecyler.DrawingCacheQuality = DrawingCacheQuality.High;

                usercontacts_Empty.Visibility = ViewStates.Gone;
                ContactsRecyler.Visibility = ViewStates.Visible;

                ContactAdapter = new UserContact_Adapter(this, new JavaList<Classes.UserContacts.User>(UserContactsList), ContactsRecyler);
                //Event

                Btn_SearchRandom.Click += BtnSearchRandomOnClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                ContactAdapter.ItemClick += ContactAdapter_OnItemClick; 
                ContactsRecyler.SetAdapter(ContactAdapter);
                 
                Get_MyContact();

                AdsGoogle.Ad_RewardedVideo(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

         
        public void Get_MyContact(int lastId = 0)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    if (AppSettings.ConnectivitySystem == "1") // Following
                        SupportActionBar.Title = GetText(Resource.String.Lbl_Following);
                    else // Friend
                        SupportActionBar.Title = GetText(Resource.String.Lbl_Friends);
                });

                //Get All User From Database 
                var dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.Get_MyContact(lastId, 25);
                if (localList != null)
                {
                    RunOnUiThread(() =>
                    {
                        var list = new JavaList<Classes.UserContacts.User>(localList);
                        if (list.Count > 0)
                        {
                            var listNew = list?.Where(c => !UserContactsList.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                            if (listNew.Count > 0 )
                            {
                                Classes.AddRange(UserContactsList, listNew);
                                var listOrder = new JavaList<Classes.UserContacts.User>(UserContactsList.OrderBy(a => a.Name));

                                                 
                                ContactAdapter.mMyContactsList = new JavaList<Classes.UserContacts.User>(listOrder);
                                ContactAdapter.ItemClick += ContactAdapter_OnItemClick;
                                

                                ContactAdapter.NotifyDataSetChanged();
                            }
                            else
                            {
                                if (ShowSnackbar)
                                {
                                    
                                    ShowSnackbar = false;
                                }

                                Get_Contacts_APi();
                            }

                            if (swipeRefreshLayout != null)
                                swipeRefreshLayout.Refreshing = false;
                        }
                        else
                        {
                            if (ShowSnackbar)
                            {
                                
                                ShowSnackbar = false;
                            }

                            Get_Contacts_APi();
                        }

                        //Set Event Scroll
                        if (OnMainScrolEvent == null)
                        {
                            var xamarinRecyclerViewOnScrollListener =
                                new XamarinRecyclerViewOnScrollListener(ContactsLayoutManager, swipeRefreshLayout);
                            OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                            OnMainScrolEvent.LoadMoreEvent += MyContact_OnScroll_OnLoadMoreEvent;
                            ContactsRecyler.AddOnScrollListener(OnMainScrolEvent);
                            ContactsRecyler.AddOnScrollListener(new ScrollDownDetector());
                        }
                        else
                        {
                            OnMainScrolEvent.IsLoading = false;
                        }
                    });
                }
                else
                { 
                    Get_Contacts_APi();
                }

                dbDatabase.Dispose();

                if (UserContactsList?.Count <= 24 || UserContactsList?.Count == 0)
                {
                 
                    Get_Contacts_APi();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Api
        public async void Get_Contacts_APi()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    RunOnUiThread(() => { swipeRefreshLayout.Refreshing = false; });
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Short)
                        .Show();
                }
                else
                {
                    if (TimerWork == "Working")
                    {
                        TimerWork = "Stop";
                        var lastIdUser = UserContactsList?.LastOrDefault()?.UserId ?? "0";

                        var (api_status, respond) = await API_Request.Get_users_friends_Async(lastIdUser);
                        if (api_status == 200)
                        {
                            if (respond is Classes.UserContacts result)
                                RunOnUiThread(() =>
                                {
                                    if (result.Users.Count <= 0)
                                    {
                                    }
                                    else if (result.Users.Count > 0)
                                    {
                                        var listNew = result.Users?.Where(c => !UserContactsList.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            Classes.AddRange(UserContactsList, listNew);

                                            var listOrder = new JavaList<Classes.UserContacts.User>(UserContactsList.OrderBy(a => a.Name));
                                           
                                            ContactAdapter.mMyContactsList = new JavaList<Classes.UserContacts.User>(listOrder);
                                            ContactAdapter.ItemClick += ContactAdapter_OnItemClick;                                          
                                            ContactAdapter.NotifyDataSetChanged();
                             
                                            var dbDatabase = new SqLiteDatabase();
                                            dbDatabase.Insert_Or_Replace_MyContactTable(UserContactsList);
                                            dbDatabase.Dispose();
                                        }
                                        else
                                        {
                                            if (ShowSnackbarNoMore)
                                            {
                                               
                                                ShowSnackbarNoMore = false;
                                            }
                                        }

                                        if (swipeRefreshLayout != null)
                                            swipeRefreshLayout.Refreshing = false;
                                    }
                                });
                        }
                        else if (api_status == 400)
                        {
                            if (respond is ErrorObject error)
                            {
                                var errortext = error._errors.Error_text;
                                

                                if (errortext.Contains("Invalid or expired access_token"))
                                    API_Request.Logout(this);
                            }
                        }
                        else if (api_status == 404)
                        {
                            var error = respond.ToString();
                         
                        }
                    }
                    TimerWork = "Working";
                }

                //Show Empty Page >> 
                //===============================================================
                RunOnUiThread(() =>
                {
                    if (UserContactsList?.Count > 0)
                    {
                        usercontacts_Empty.Visibility = ViewStates.Gone;
                        ContactsRecyler.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        usercontacts_Empty.Visibility = ViewStates.Visible;
                        ContactsRecyler.Visibility = ViewStates.Gone;
                    }

                    swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =
                            new XamarinRecyclerViewOnScrollListener(ContactsLayoutManager, swipeRefreshLayout);
                        OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMainScrolEvent.LoadMoreEvent += MyContact_OnScroll_OnLoadMoreEvent;
                        ContactsRecyler.AddOnScrollListener(OnMainScrolEvent);
                        ContactsRecyler.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        OnMainScrolEvent.IsLoading = false;
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TimerWork = "Working";
                Get_Contacts_APi();
            }
        }

        private void ContactAdapter_OnItemClick(object sender, AdapterClickEvents adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = ContactAdapter.GetItem(position);
                    if (item != null)
                    {
                        Intent intent = new Intent(this, typeof(ChatWindow_Activity));
                        intent.PutExtra("UserID", item.UserId);
                        intent.PutExtra("TypeChat", "Contact");
                        intent.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                        StartActivity(intent);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                ContactsRecyler.Visibility = ViewStates.Gone;
                UserContactsList.Clear();
                ContactAdapter?.Clear();

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.Clear_UsersContact();
                dbDatabase.Dispose();

                Get_Contacts_APi(); 

                swipeRefreshLayout.Refreshing = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.User_Search_Menu, menu);

            var item = menu.FindItem(Resource.Id.searchUserBar);
            var searchItem = MenuItemCompat.GetActionView(item);

            _SearchView = searchItem.JavaCast<SearchView>();
            _SearchView.SetIconifiedByDefault(true);
            _SearchView.QueryTextChange += _SearchView_OnTextChange;
            _SearchView.QueryTextSubmit += _SearchView_OnTextSubmit;
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                case Resource.Id.menue_invite_friend:
                    invite_friend_OnClick();
                    break;

                case Resource.Id.menue_refresh:
                    refresh_OnClick();
                    break;

                case Resource.Id.menue_blockList:
                    blockList_OnClick();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void blockList_OnClick()
        {
            try
            {
                var Intent = new Intent(Application.Context, typeof(BlockedUsers_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void refresh_OnClick()
        {
            try
            {
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                ContactAdapter?.Clear();
                Get_MyContact();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void invite_friend_OnClick()
        {
            try
            {
                var Intent = new Intent(Application.Context, typeof(Invite_Friends_Activity));
                StartActivity(Intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void _SearchView_OnTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                ContactAdapter.Filter.InvokeFilter(e.Query);
                e.Handled = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            } 
        }

        private void _SearchView_OnTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            try
            {
                TimerWork = "Stop";

                if (string.IsNullOrEmpty(e.NewText))
                {
                    TimerWork = "Working";

                }

                ContactAdapter.Filter.InvokeFilter(e.NewText);
                ContactsRecyler.Invalidate();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //event Search Random
        private void BtnSearchRandomOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var intent = new Intent(this, typeof(OnlineSearch_Activity));
                intent.PutExtra("Key", "Random");
                StartActivity(intent);
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
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Scroll

        //Event Scroll #MyContact
        private void MyContact_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                int afterContact = UserContactsList.Count();
                if (afterContact >= 0)
                    try
                    {
                        //Run Load More Api 
                        if(TimerWork =="Stopped")
                        Task.Run(() => { Get_MyContact(afterContact); });
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                swipeRefreshLayout.Refreshing = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            public delegate void LoadMoreEventHandler(object sender, EventArgs e);

            public bool IsLoading;
            public LinearLayoutManager LayoutManager;
            public SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager,
                SwipeRefreshLayout swipeRefreshLayout)
            {
                try
                {
                    LayoutManager = layoutManager;
                    SwipeRefreshLayout = swipeRefreshLayout;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public event LoadMoreEventHandler LoadMoreEvent;

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    var visibleItemCount = recyclerView.ChildCount;
                    var totalItemCount = recyclerView.GetAdapter().ItemCount;

                    var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();
                    if (visibleItemCount + pastVisiblesItems + 8 >= totalItemCount)
                        if (IsLoading == false)
                        {
                            LoadMoreEvent?.Invoke(this, null);
                            IsLoading = true;
                        }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public class ScrollDownDetector : RecyclerView.OnScrollListener
        {
            public Action Action;
            private bool readyForAction;

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                try
                {
                    base.OnScrollStateChanged(recyclerView, newState);

                    if (newState == RecyclerView.ScrollStateDragging)
                    {
                        //The user starts scrolling
                        readyForAction = true;

                        if (recyclerView.ScrollState == (int) ScrollState.Fling)
                            ImageService.Instance
                                .SetPauseWork(true); // all image loading requests will be silently canceled
                        else if (recyclerView.ScrollState == (int) ScrollState.Fling)
                            ImageService.Instance.SetPauseWork(false);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    if (readyForAction && dy > 0)
                    {
                        //The scroll direction is down
                        readyForAction = false;
                        Action();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        #endregion

    }
}