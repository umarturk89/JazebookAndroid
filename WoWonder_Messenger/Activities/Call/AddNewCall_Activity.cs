using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using WoWonder.Activities.DefaultUser;
using WoWonder.Adapters;
using WoWonder.Frameworks.Agora;
using WoWonder.Frameworks.Twilio;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using Toolbar = Android.Support.V7.Widget.Toolbar;


namespace WoWonder.Activities.Call
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class AddNewCall_Activity : AppCompatActivity
    {
        #region Variables Basic

        public LinearLayoutManager ContactsLayoutManager;

        public AddNewCall_Adapter CallAdapter;
        public RecyclerView CalluserRecylerView;

        private LinearLayout Calluser_Empty;
        private AppCompatTextView Icon_Calluser;
        public SwipeRefreshLayout swipeRefreshLayout;

        private Button ButtonSearchRandom;

        private string LastCalluserid = "";

        public XamarinRecyclerViewOnScrollListener OnMainScrolEvent;

        public string TimeNow = DateTime.Now.ToString("hh:mm");
        public static Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public string Time = Convert.ToString(unixTimestamp);


        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                // Create your application here
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                SetContentView(Resource.Layout.AddNewCall_layout);

                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                if (ToolBar != null)
                {
                    ToolBar.Title = GetText(Resource.String.Lbl_Select_contact);

                    SetSupportActionBar(ToolBar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }

                CalluserRecylerView = FindViewById<RecyclerView>(Resource.Id.CallRecylerview);
                Calluser_Empty = FindViewById<LinearLayout>(Resource.Id.Callusercontacts_LinerEmpty);

                Icon_Calluser = FindViewById<AppCompatTextView>(Resource.Id.Callusercontacts_icon);
                IMethods.Set_TextViewIcon("1", Icon_Calluser, IonIcons_Fonts.IosPeopleOutline);
                Icon_Calluser.SetTextColor(Color.ParseColor(AppSettings.MainColor));

                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                 
                ButtonSearchRandom = FindViewById<Button>(Resource.Id.SearchRandom_Button);
                 
                CalluserRecylerView.Visibility = ViewStates.Visible;
                Calluser_Empty.Visibility = ViewStates.Gone;

                CallAdapter = new AddNewCall_Adapter(this);
                ContactsLayoutManager = new LinearLayoutManager(this);
                CalluserRecylerView.SetLayoutManager(ContactsLayoutManager);
                CallAdapter.mCallUserContacts = new ObservableCollection<Classes.UserContacts.User>();
                CalluserRecylerView.SetAdapter(CallAdapter);

                LoadContacts();

                AdsGoogle.Ad_RewardedVideo(this);
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
                ButtonSearchRandom.Click += ButtonSearchRandomOnClick;
                CallAdapter.AudioCallClick += CallAdapter_OnAudioCallClick;
                CallAdapter.VideoCallClick += CallAdapter_OnVideoCallClick;
                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                ButtonSearchRandom.Click -= ButtonSearchRandomOnClick;
                CallAdapter.AudioCallClick -= CallAdapter_OnAudioCallClick;
                CallAdapter.VideoCallClick -= CallAdapter_OnVideoCallClick;
                swipeRefreshLayout.Refresh -= SwipeRefreshLayoutOnRefresh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadContacts(int afterContact = 0)
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.Get_MyContact(afterContact, 100);
                if (localList?.Count > 0)
                {
                    RunOnUiThread(() =>
                    {
                        var listNew = localList?.Where(c => !CallAdapter.mCallUserContacts.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                        if (listNew.Count > 0)
                        {
                            Classes.AddRange(CallAdapter.mCallUserContacts, localList);
                            var lastCountItem = CallAdapter.ItemCount;
                            CallAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                            CallAdapter.BindEnd();
                        }
                        else
                        {
                            Get_Contacts_APi();
                        }
                    });
                }
                else
                {
                    Get_Contacts_APi();
                }

                RunOnUiThread(() =>
                { 
                    //Set Event Scroll
                    if (OnMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener =new XamarinRecyclerViewOnScrollListener(ContactsLayoutManager);
                        OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMainScrolEvent.LoadMoreEvent += MyContact_OnScroll_OnLoadMoreEvent;
                        CalluserRecylerView.AddOnScrollListener(OnMainScrolEvent);
                        CalluserRecylerView.AddOnScrollListener(new ScrollDownDetector());
                    }
                    else
                    {
                        OnMainScrolEvent.IsLoading = false;
                    }
                });

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LoadContacts(afterContact);
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
                    var lastIdUser = CallAdapter.mCallUserContacts?.LastOrDefault()?.UserId ?? "0";

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
                                    var listNew = result.Users?.Where(c => !CallAdapter.mCallUserContacts.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                                    if (listNew.Count > 0)
                                    {
                                        Classes.AddRange(CallAdapter.mCallUserContacts, listNew);
                                         
                                        var lastCountItem = CallAdapter.ItemCount;
                                        CallAdapter.NotifyItemRangeInserted(lastCountItem, listNew.Count);
                                        CallAdapter.BindEnd();
                                        CallAdapter.NotifyItemChanged(0);

                                        //Insert Or Update All data UsersContact to database
                                        var dbDatabase = new SqLiteDatabase();
                                        dbDatabase.Insert_Or_Replace_MyContactTable(CallAdapter.mCallUserContacts);
                                        dbDatabase.Dispose();
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

                //Show Empty Page >> 
                //===============================================================
                RunOnUiThread(() =>
                {
                    if (CallAdapter.mCallUserContacts?.Count > 0)
                    {
                        Calluser_Empty.Visibility = ViewStates.Gone;
                        CalluserRecylerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        Calluser_Empty.Visibility = ViewStates.Visible;
                        CalluserRecylerView.Visibility = ViewStates.Gone;
                    }

                    swipeRefreshLayout.Refreshing = false;

                    //Set Event Scroll
                    if (OnMainScrolEvent == null)
                    {
                        var xamarinRecyclerViewOnScrollListener = new XamarinRecyclerViewOnScrollListener(ContactsLayoutManager);
                        OnMainScrolEvent = xamarinRecyclerViewOnScrollListener;
                        OnMainScrolEvent.LoadMoreEvent += MyContact_OnScroll_OnLoadMoreEvent;
                        CalluserRecylerView.AddOnScrollListener(OnMainScrolEvent);
                        CalluserRecylerView.AddOnScrollListener(new ScrollDownDetector());
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
                Get_Contacts_APi();
            }
        }
         
        private void CallAdapter_OnVideoCallClick(object sender, AddNewCall_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                TimeNow = DateTime.Now.ToString("hh:mm");
                Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                Time = Convert.ToString(unixTimestamp);

                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = CallAdapter.GetItem(Position);
                    if (item != null)
                    {
                        Intent IntentOfvideoCall = new Intent(this, typeof(TwilioVideoCallActivity));
                        if (AppSettings.Use_Agora_Library && AppSettings.Use_Twilio_Library == false)
                        {
                            IntentOfvideoCall = new Intent(this, typeof(AgoraVideoCallActivity));
                            IntentOfvideoCall.PutExtra("type", "Agora_video_calling_start");
                        }
                        else if (AppSettings.Use_Agora_Library == false && AppSettings.Use_Twilio_Library)
                        {
                            IntentOfvideoCall = new Intent(this, typeof(TwilioVideoCallActivity));
                            IntentOfvideoCall.PutExtra("type", "Twilio_video_calling_start");
                        }

                        IntentOfvideoCall.PutExtra("UserID", item.UserId);
                        IntentOfvideoCall.PutExtra("avatar", item.Avatar);
                        IntentOfvideoCall.PutExtra("name", item.Name);
                        IntentOfvideoCall.PutExtra("time", TimeNow);
                        IntentOfvideoCall.PutExtra("CallID", Time);
                        IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                        IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                        IntentOfvideoCall.PutExtra("from_id", "0");
                        IntentOfvideoCall.PutExtra("active", "0");
                        IntentOfvideoCall.PutExtra("status", "0");
                        IntentOfvideoCall.PutExtra("room_name", "TestRoom");

                        StartActivity(IntentOfvideoCall);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CallAdapter_OnAudioCallClick(object sender, AddNewCall_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                TimeNow = DateTime.Now.ToString("hh:mm");
                Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                Time = Convert.ToString(unixTimestamp);

                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = CallAdapter.GetItem(Position);
                    if (item != null)
                    {
                        Intent IntentOfvideoCall = new Intent(this, typeof(TwilioVideoCallActivity));
                        if (AppSettings.Use_Agora_Library && AppSettings.Use_Twilio_Library == false)
                        {
                            IntentOfvideoCall = new Intent(this, typeof(AgoraAudioCallActivity));
                            IntentOfvideoCall.PutExtra("type", "Agora_audio_calling_start");
                        }
                        else if (AppSettings.Use_Agora_Library == false && AppSettings.Use_Twilio_Library)
                        {
                            IntentOfvideoCall = new Intent(this, typeof(TwilioAudioCallActivity));
                            IntentOfvideoCall.PutExtra("type", "Twilio_audio_calling_start");
                        }

                        IntentOfvideoCall.PutExtra("UserID", item.UserId);
                        IntentOfvideoCall.PutExtra("avatar", item.Avatar);
                        IntentOfvideoCall.PutExtra("name", item.Name);
                        IntentOfvideoCall.PutExtra("time", TimeNow);
                        IntentOfvideoCall.PutExtra("CallID", Time);
                        IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                        IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                        IntentOfvideoCall.PutExtra("from_id", "0");
                        IntentOfvideoCall.PutExtra("active", "0");
                        IntentOfvideoCall.PutExtra("status", "0");
                        IntentOfvideoCall.PutExtra("room_name", "TestRoom");
                        StartActivity(IntentOfvideoCall);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Refresh Data Page
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                CallAdapter?.Clear();

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
         
        //Event Open Search And Get Users Random
        private void ButtonSearchRandomOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(this, typeof(OnlineSearch_Activity));
                intent.PutExtra("Key", "Random");
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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

        #region Scroll

        //Event Scroll #MyContact
        private void MyContact_OnScroll_OnLoadMoreEvent(object sender, EventArgs eventArgs)
        {
            try
            {
                //Code get last id where LoadMore >>
                int afterContact = CallAdapter.mCallUserContacts.Count();
                if (afterContact >= 0)
                    try
                    {
                        //Run Load More Api 
                        Task.Run(() => { LoadContacts(afterContact); });
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

    }
}