using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
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
using Android.Views.InputMethods;
using Android.Widget;
using FFImageLoading;
using Newtonsoft.Json;
using WoWonder.Activities.DialogUserFragment;
using WoWonder.Adapters;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;
using Exception = System.Exception;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.DefaultUser
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class OnlineSearch_Activity : AppCompatActivity,ViewTreeObserver.IOnGlobalLayoutListener
    {
        #region Variables Basic

        private RecyclerView SearchRecyler;
        private LinearLayoutManager mLayoutManager;
        public static SearchUser_Adapter mAdapter;

        private SearchView _SearchView;
        private Button Btn_SearchRandom;
        private LinearLayout OnlineSearch_Empty;
        //private TextView Icon_onlinesearch;

        public string search_key = "";

        private string LastUserid = "";
        public XamarinRecyclerViewOnScrollListener User_OnMainScrolEvent;

        private string Filter_gender = "";
        private string Filter_image = "";
        private string Filter_status = "";

        private FloatingActionButton FloatingActionButtonView;

        private SwipeRefreshLayout swipeRefreshLayout;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {

                this.Window.SetSoftInputMode(SoftInput.AdjustNothing);

                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "OnlineSearch_Layout" layout resource
                SetContentView(Resource.Layout.OnlineSearch_Layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.mainSearchtoolbar);
                SetSupportActionBar(ToolBar);
                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                //Get values
                SearchRecyler = FindViewById<RecyclerView>(Resource.Id.searchRecylerview);
                Btn_SearchRandom = FindViewById<Button>(Resource.Id.SearchRandom_Button);
                OnlineSearch_Empty = FindViewById<LinearLayout>(Resource.Id.OnlineSearch_LinerEmpty);
               

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight,
                    Android.Resource.Color.HoloRedLight);
                swipeRefreshLayout.Enabled = false;

                FloatingActionButtonView = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonView);

                //Set Adapter 
                mLayoutManager = new LinearLayoutManager(this);
                SearchRecyler.SetLayoutManager(mLayoutManager);
                mAdapter = new SearchUser_Adapter(this);
                mAdapter.mSearchUserList = new ObservableCollection<GetSearchObject.User>();
                SearchRecyler.SetAdapter(mAdapter);


                var data = Intent.GetStringExtra("Key") ?? "Data not available";
                if (data != "Data not available" && !String.IsNullOrEmpty(data))
                {
                    if (search_key == "Random")
                    {
                        search_key = "a";
                        GetSearch_Result("a");
                    }
                    else
                    {
                        search_key = data;
                        if (_SearchView != null)
                        {
                            _SearchView.SetQuery(search_key, false);
                            _SearchView.ClearFocus();
                            _SearchView.OnActionViewCollapsed();
                        }

                        GetSearch_Result(search_key);
                    }
                }

                //Close keybourd
                InputMethodManager inputManager = (InputMethodManager) this.GetSystemService(Activity.InputMethodService);
                if (inputManager.IsActive)
                {
                    if (ToolBar != null)
                    {
                        inputManager = (InputMethodManager) this.GetSystemService(Activity.InputMethodService);
                        inputManager.HideSoftInputFromWindow(ToolBar.WindowToken, 0);
                    }

                    
                }

                _SearchView.ClearFocus();

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

                if (_SearchView != null)
                {
                    _SearchView.SetQuery("", false);
                    _SearchView.ClearFocus();
                    _SearchView.SetIconifiedByDefault(false);
                    _SearchView.OnActionViewExpanded();
                }

                //Event
                mAdapter.ItemClick += MAdapterOnItemClick;
                mAdapter.ItemLongClick += MAdapterOnItemLongClick;
                
                Btn_SearchRandom.Click += BtnSearchRandomOnClick;
                FloatingActionButtonView.Click += Filter_OnClick;
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
                //Event
                mAdapter.ItemClick -= MAdapterOnItemClick;
                mAdapter.ItemLongClick -= MAdapterOnItemLongClick;
              
                Btn_SearchRandom.Click -= BtnSearchRandomOnClick;
                FloatingActionButtonView.Click -= Filter_OnClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
                base.OnPause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Filter_OnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(this, typeof(SearchFilter_Activity));
                StartActivityForResult(intent, 60000);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Search Api
        public async void GetSearch_Result(string key, string user_offset = "")
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
                    RunOnUiThread(() =>
                    {
                        if (user_offset == "") 
                        {
                            
                            swipeRefreshLayout.Refreshing = true;
                            swipeRefreshLayout.Enabled = true;
                        } 
                    });

                    var (api_status, respond) = await Global.Get_Search(UserDetails.User_id, key, "35",user_offset, "", "", Filter_gender);
                    if (api_status == 200)
                    {
                        if (respond is GetSearchObject result)
                        {
                            if (result.Users.Count <= 0 && result.Groups.Count <= 0 && result.Pages.Count <= 0)
                                return;

                            RunOnUiThread(() =>
                            {
                                //Add result users
                                //*===========================================*
                                if (result.Users.Count > 0 && result.Users.Count != 0)
                                {
                                    if (mAdapter.mSearchUserList.Count <= 0)
                                    {
                                        mAdapter.mSearchUserList =new ObservableCollection<GetSearchObject.User>(result.Users);
                                        mAdapter.BindEnd();
                                    }
                                    else
                                    {
                                        //Bring new item
                                        var listNew = result.Users?.Where(c =>!mAdapter.mSearchUserList.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                                        if (listNew.Count > 0)
                                        {
                                            var lastCountItem = mAdapter.ItemCount;

                                            //Results differ
                                            Classes.AddRange(mAdapter.mSearchUserList, listNew);
                                            mAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                                        }
                                    }
                                }
                            });
                        }
                    }
                    else if (api_status == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errorText = error._errors.Error_text;
                          

                            if (errorText.Contains("Invalid or expired access_token"))
                                API_Request.Logout(this);
                        }
                    }
                    else if (api_status == 404)
                    {
                        var error = respond.ToString();
                       
                    }

                    RunOnUiThread(() =>
                    {
                        //Show Empty Page
                        //===========================================
                        if (mAdapter.mSearchUserList.Count > 0)
                        {
                            SearchRecyler.Visibility = ViewStates.Visible;
                            OnlineSearch_Empty.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            SearchRecyler.Visibility = ViewStates.Gone;
                            OnlineSearch_Empty.Visibility = ViewStates.Visible;
                        }

                        //Set Event Scroll >> Users
                        if (User_OnMainScrolEvent == null)
                        {
                            var xamarinRecyclerViewOnScrollListener =
                                new XamarinRecyclerViewOnScrollListener(mLayoutManager);
                            User_OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                            User_OnMainScrolEvent.LoadMoreEvent += LastUsers_OnScroll_OnLoadMoreEvent;
                            SearchRecyler.AddOnScrollListener(User_OnMainScrolEvent);
                            SearchRecyler.AddOnScrollListener(new ScrollDownDetector());
                        }
                        else
                        {
                            User_OnMainScrolEvent.IsLoading = false;
                        }

                        _SearchView.ClearFocus();
                        swipeRefreshLayout.Refreshing = false; 
                      
                    });
                }
                RunOnUiThread(() => { swipeRefreshLayout.Refreshing = false; }); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                GetSearch_Result(search_key, LastUserid);
                RunOnUiThread(() =>
                {
                    swipeRefreshLayout.Refreshing = false;
                    if (mAdapter.mSearchUserList.Count > 0)
                    {
                        SearchRecyler.Visibility = ViewStates.Visible;
                        OnlineSearch_Empty.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        SearchRecyler.Visibility = ViewStates.Gone;
                        OnlineSearch_Empty.Visibility = ViewStates.Visible;
                    }

                });
            }
        }

        //Event Search Random 
        private void BtnSearchRandomOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                mAdapter.Clear();

                SearchRecyler.Visibility = ViewStates.Visible;
                OnlineSearch_Empty.Visibility = ViewStates.Gone;

                GetSearch_Result("a");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                mAdapter.Clear();

                SearchRecyler.Visibility = ViewStates.Visible;
                OnlineSearch_Empty.Visibility = ViewStates.Gone;

                GetSearch_Result(search_key);
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
                if (requestCode == 60000 && resultCode == Result.Ok)
                {
                    var gender = data.GetStringExtra("gender");
                    if (gender != "Data not available" && !string.IsNullOrEmpty(gender))
                        Filter_gender = gender;

                    var image = data.GetStringExtra("profilePicture");
                    if (image != "Data not available" && !string.IsNullOrEmpty(image))
                        Filter_image = image;

                    var status = data.GetStringExtra("status");
                    if (status != "Data not available" && !string.IsNullOrEmpty(status))
                        Filter_status = status;

                  
                }

                base.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            try
            {
                MenuInflater.Inflate(Resource.Menu.Search_menue, menu);

                var item = menu.FindItem(Resource.Id.searchUserBar);
                var searchItem = MenuItemCompat.GetActionView(item);
                _SearchView = searchItem.JavaCast<SearchView>();
                _SearchView.SetIconifiedByDefault(false);

                _SearchView.SetQuery("", false);
                _SearchView.ClearFocus();
                _SearchView.OnActionViewExpanded();

                _SearchView.Iconified = false;
                _SearchView.SetIconifiedByDefault(false);
                _SearchView.OnActionViewExpanded();

                //Remove Icon Search
                ImageView searchViewIcon = (ImageView)_SearchView.FindViewById(Resource.Id.search_mag_icon);
                ViewGroup linearLayoutSearchView = (ViewGroup)searchViewIcon.Parent;
                linearLayoutSearchView.RemoveView(searchViewIcon);

                _SearchView.QueryTextChange += _SearchView_OnTextChange;
                _SearchView.QueryTextSubmit += _SearchView_OnTextSubmit;

                InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
                inputManager.HideSoftInputFromWindow(_SearchView.WindowToken, 0);

                return base.OnCreateOptionsMenu(menu);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            } 
        }

        #region Menu

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

        private void _SearchView_OnTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            try
            {
                search_key = e.Query;

                mAdapter.Clear();

                SearchRecyler.Visibility = ViewStates.Visible;
                OnlineSearch_Empty.Visibility = ViewStates.Gone;

                GetSearch_Result(search_key);

                //Hide keyboard programmatically in MonoDroid
                e.Handled = true;
                var inputManager = (InputMethodManager) GetSystemService(InputMethodService);
                inputManager.HideSoftInputFromWindow(_SearchView.WindowToken, HideSoftInputFlags.None);

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
               
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        private void MAdapterOnImageClick(object sender, OnlineSearch_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = mAdapter.GetItem(position);
                    if (item != null)
                    {
                        if (item.UserId != UserDetails.User_id)
                        {
                            Intent Int = new Intent(this, typeof(UserProfile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                            Int.AddFlags(ActivityFlags.NewTask);
                            Int.PutExtra("UserType", "Chat_SearchUser");
                            Int.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                        }
                        else
                        {
                            Intent Int = new Intent(this, typeof(MyProfile_Activity));
                            Int.PutExtra("UserId", item.UserId);
                        } 
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MAdapterOnItemLongClick(object sender, SearchUser_AdapterClickEventArgs adapterClickEvents)
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
                        Dialog_User UserDialog = new Dialog_User(item.UserId, item);
                        UserDialog.Show(transaction, "dialog fragment");
                        UserDialog._OnUserUpComplete += SignUpDialogOnOnUserUpComplete;

                       
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SignUpDialogOnOnUserUpComplete(object sender, Dialog_User.OnUserUp_EventArgs onUserUpEventArgs)
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

        private void MAdapterOnItemClick(object sender, SearchUser_AdapterClickEventArgs adapterClickEvents)
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
                        Dialog_User UserDialog = new Dialog_User(item.UserId, item);
                        UserDialog.Show(transaction, "dialog fragment");
                        UserDialog._OnUserUpComplete += SignUpDialogOnOnUserUpComplete;
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
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Scroll

        //Event Scroll #LastUsers
        private void LastUsers_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                var item = mAdapter.mSearchUserList.LastOrDefault();
                if (item != null) LastUserid = item.UserId;

                if (LastUserid != "")
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { GetSearch_Result(search_key, LastUserid); });
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
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

            public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager)
            {
                try
                {
                    LayoutManager = layoutManager;
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

        public void OnGlobalLayout()
        {
           
        }
    }
}