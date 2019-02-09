using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View.Animation;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Theartofdev.Edmodo.Cropper;
using Hani.Momanii.Supernova_emoji_library.Actions;
using Hani.Momanii.Supernova_emoji_library.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WoWonder.Activities.DefaultUser;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.Adapters;
using WoWonder.Frameworks.Agora;
using WoWonder.Frameworks.Twilio;
using WoWonder.Functions;
using Console = System.Console;
using SupportFragment = Android.Support.V4.App.Fragment;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using Exception = System.Exception;
using File = Java.IO.File;
using String = System.String;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using FFImageLoading;
using Java.Lang;
using UniversalImageLoader.Core;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Activities.ChatWindow
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ResizeableActivity = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class ChatWindow_Activity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, View.IOnLayoutChangeListener
    {
        #region Variables Basic

        private bool isrecording = false;

        public AppCompatImageView ChatEmojiImage;

        public RelativeLayout RootView;
        public LinearLayout FirstBoxonLinear;

        public EmojiconEditText EmojiconEditTextView;
        public Timer Timer = new Timer();
        public CircleButton ChatColorButton, ChatContactButton, ChatStickerButton, ChatMediaButton, ChatSendButton;

        public Toolbar TopChatToolBar;
        public RecyclerView ChatBoxListView;
        public Chat_Colors_Fragment ChatColorBoxFragment;
        public Chat_Recourd_Sound_Fragment Chat_Recourd_Sound_BoxFragment;
        public Chat_StickersTab_Fragment Chat_StickersTab_BoxFragment;

        public FrameLayout ButtomFragmentHolder;
        public FrameLayout TopFragmentHolder;

        public LinearLayoutManager MLayoutManager;
        public static Message_Adapter MAdapter;

        private SupportFragment MainFragmentOpened;

        public IMethods.AudioRecorderAndPlayer RecorderService;

        private FastOutSlowInInterpolator interplator;
        public static string MainChatColor = AppSettings.MainColor;

        public static string Userid = ""; // to_id
        public string Lastseen = "";

        public string TimeNow = DateTime.Now.ToString("hh:mm");
        public static Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public string Time = Convert.ToString(unixTimestamp);

        public string before_message_id = "0";
        private string FirstMessageid = "";
        public string LastMessageid = "";
        public Timer timer;
        public string TaskWork = "";
        public SwipeRefreshLayout swipeRefreshLayout;

        private Classes.Get_Users_List_Object.User _datauser = null;
        private Classes.UserContacts.User _userContacts = null;
        private GetSearchObject.User _SearchUser = null;
        private static string Notfi = "";
        private string TypeChat = "";
        public string GifFile = "";

        public ObservableCollection<Classes.UserChat> ListDataUserChat = new ObservableCollection<Classes.UserChat>();

        #endregion

        public Chat_Colors_Fragment newInstance(int someInt)
        {
            ChatColorBoxFragment = new Chat_Colors_Fragment();

            Bundle args = new Bundle();
            args.PutInt("userid", someInt);
            ChatColorBoxFragment.Arguments = args;

            return ChatColorBoxFragment;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                Window.SetSoftInputMode(SoftInput.AdjustResize);

                base.OnCreate(savedInstanceState);
                var data = Intent.GetStringExtra("UserID") ?? "Data not available";
                if (data != "Data not available" && !String.IsNullOrEmpty(data))
                {
                    Userid = data; // to_id
                }

                newInstance(int.Parse(Userid));

                IMethods.IApp.FullScreenApp(this);

                Window.SetBackgroundDrawableResource(Resource.Drawable.chatBackground3);

                var type = Intent.GetStringExtra("TypeChat") ?? "Data not available";
                if (type != "Data not available" && !String.IsNullOrEmpty(type))
                {
                    TypeChat = type;
                    if (type == "LastMessenger")
                    {
                        var dataUser = Last_Messages_Fragment.mAdapter.MLastMessagesUser?.FirstOrDefault(a => a.UserId == Userid);
                        if (dataUser != null)
                        {
                            _datauser = dataUser;

                            MainChatColor = _datauser.ChatColor.Contains("rgb") ? IMethods.Fun_String.ConvertColorRGBtoHex(_datauser.ChatColor) : _datauser.ChatColor;

                            Window.SetStatusBarColor(Color.ParseColor(MainChatColor));
                        }
                        else
                        {
                            MainChatColor = AppSettings.MainColor;
                            Window.SetStatusBarColor(Color.ParseColor(MainChatColor));
                        }

                        
                    }
                    else if (type == "Contact")
                    {
                        var item = JsonConvert.DeserializeObject<Classes.UserContacts.User>(Intent.GetStringExtra("UserItem"));
                        if (item != null)
                        {
                            if (item.St_ChatColor == null)
                                item.St_ChatColor = AppSettings.MainColor;

                            _userContacts = item;
                            MainChatColor = item.St_ChatColor.Contains("rgb") ? IMethods.Fun_String.ConvertColorRGBtoHex(item.St_ChatColor) : item.St_ChatColor;
                            Window.SetStatusBarColor(Color.ParseColor(MainChatColor));
                        }
                        else
                        {
                            MainChatColor = AppSettings.MainColor;
                            Window.SetStatusBarColor(Color.ParseColor(MainChatColor));
                        }
                    }
                    else
                    {
                        MainChatColor = AppSettings.MainColor;
                        Window.SetStatusBarColor(Color.ParseColor(MainChatColor));
                    }
                }
                else
                {
                    Get_UserProfileData_Api();
                }

                SetTheme(MainChatColor);

                // Set our view from the "ChatWindow" layout resource
                SetContentView(Resource.Layout.ChatWindow);

                //Audio FrameWork initialize 
                RecorderService = new IMethods.AudioRecorderAndPlayer(AppSettings.Application_Name);

                interplator = new FastOutSlowInInterpolator();

                ChatColorBoxFragment = new Chat_Colors_Fragment();
                Bundle args = new Bundle();
                args.PutString("userid", Userid);
                ChatColorBoxFragment.Arguments = args;

                Chat_Recourd_Sound_BoxFragment = new Chat_Recourd_Sound_Fragment();
                Chat_StickersTab_BoxFragment = new Chat_StickersTab_Fragment();

                //Get values
                RootView = FindViewById<RelativeLayout>(Resource.Id.rootChatWindowView);
                FirstBoxonLinear = FindViewById<LinearLayout>(Resource.Id.firstBoxonButtom);

                ChatEmojiImage = FindViewById<AppCompatImageView>(Resource.Id.emojiicon);
                EmojiconEditTextView = FindViewById<EmojiconEditText>(Resource.Id.EmojiconEditText5);
                ChatSendButton = FindViewById<CircleButton>(Resource.Id.sendButton);
                ChatBoxListView = FindViewById<RecyclerView>(Resource.Id.recyler);
                ChatColorButton = FindViewById<CircleButton>(Resource.Id.colorButton);
                ChatStickerButton = FindViewById<CircleButton>(Resource.Id.stickerButton);
                ChatMediaButton = FindViewById<CircleButton>(Resource.Id.mediaButton);
                ChatContactButton = FindViewById<CircleButton>(Resource.Id.contactButton);
                ButtomFragmentHolder = FindViewById<FrameLayout>(Resource.Id.ButtomFragmentHolder);
                TopFragmentHolder = FindViewById<FrameLayout>(Resource.Id.TopFragmentHolder);
                swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);

                TopChatToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);

                SupportFragmentManager.BeginTransaction().Add(ButtomFragmentHolder.Id, ChatColorBoxFragment, "ChatColorBoxFragment");
                SupportFragmentManager.BeginTransaction().Add(TopFragmentHolder.Id, Chat_Recourd_Sound_BoxFragment, "Chat_Recourd_Sound_Fragment");

                SetSupportActionBar(TopChatToolBar);

                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                ChatBoxListView.SetItemAnimator(null);

                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);

                MLayoutManager = new LinearLayoutManager(this);

               
                ChatBoxListView.SetLayoutManager(MLayoutManager);

                MAdapter = new Message_Adapter(this);
                MAdapter.mmessage = new ObservableCollection<Classes.Message>();
                MAdapter.ItemLongClick += Copytext_OnItemLongClick;
                ChatBoxListView.SetAdapter(MAdapter);
                
                //Add Event
                ChatSendButton.Touch += Chat_sendButton_Touch;
                ChatMediaButton.Click += ChatMediaButtonOnClick;
                if (AppSettings.ShowButton_RecourdSound)
                {
                    ChatSendButton.LongClickable = true;
                    ChatSendButton.LongClick += Chat_sendButton_LongClick;

                    ChatSendButton.Tag = "Free";
                    ChatSendButton.SetImageResource(Resource.Drawable.microphone);
                }
                else
                {
                    ChatSendButton.Tag = "Text";
                    ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
                }

                if (AppSettings.Show_Button_color)
                {
                    ChatColorButton.Visibility = ViewStates.Visible;
                    ChatColorButton.Click += Chat_colorButton_Click;
                    ChatColorButton.Tag = "Closed";
                }
                else
                {
                    ChatColorButton.Visibility = ViewStates.Gone;
                }

                if (AppSettings.Show_Button_stickers)
                {
                    ChatStickerButton.Visibility = ViewStates.Visible;
                    ChatStickerButton.Click += Chat_stickerButton_Click;
                    ChatStickerButton.Tag = "Closed";
                }
                else
                {
                    ChatStickerButton.Visibility = ViewStates.Gone;
                }

                if (AppSettings.Show_Button_contact)
                {
                    ChatContactButton.Visibility = ViewStates.Visible;
                    ChatContactButton.Click += Chat_contactButton_Click;
                }
                else
                {
                    ChatContactButton.Visibility = ViewStates.Gone;
                }


                var dataNotfi = Intent.GetStringExtra("Notfi") ?? "Data not available";
                if (dataNotfi != "Data not available" && !String.IsNullOrEmpty(data))
                {
                    Notfi = dataNotfi;
                    if (Notfi == "Notfi")
                    {
                        string dataApp = Intent.GetStringExtra("App");
                        if (dataApp == "Timeline")
                        {
                            string name = Intent.GetStringExtra("Name");
                            string username = Intent.GetStringExtra("Username");
                            string about = Intent.GetStringExtra("About");
                            string address = Intent.GetStringExtra("Address");
                            string phone = Intent.GetStringExtra("Phone");
                            string website = Intent.GetStringExtra("Website");
                            string working = Intent.GetStringExtra("Working");
                            string time = Intent.GetStringExtra("Time");
                            Lastseen = Intent.GetStringExtra("LastSeen") ?? "off";

                            SupportActionBar.Title = name;

                            //Online Or offline
                            if (Lastseen == "on")
                            {
                                SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Online);
                                Lastseen = GetString(Resource.String.Lbl_Online);
                            }
                            else
                            {
                                SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Last_seen) + " " + time;
                                Lastseen = GetString(Resource.String.Lbl_Last_seen) + " " + time;
                            }
                        }

                        Get_UserProfileData_Api();
                    }
                }

                //Set ToolBar and data chat
                loadData_ItemUser();

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


                if (timer != null)
                {
                    timer.Enabled = true;
                    timer.Start();
                }
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

                if (timer != null)
                {
                    timer.Enabled = false;
                    timer.Stop();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get Data User API
        public async void Get_UserProfileData_Api()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                }
                else
                {
                    var (apiStatus, respond) = await Global.Get_User_Data( Userid);
                    if (apiStatus == 200)
                    {
                        if (respond is GetUserDataObject result)
                        {
                            //Add Data User
                            //=======================================
                            if (result.user_data != null)
                            {
                                var user = result.user_data;
                                ListDataUserChat.Clear();
                                ListDataUserChat.Add(new Classes.UserChat
                                {
                                    UserId = user.user_id,
                                    Username = user.username,
                                    Email = user.email,
                                    FirstName = user.first_name,
                                    LastName = user.last_name,
                                    Avatar = user.avatar,
                                    Cover = user.cover,
                                    RelationshipId = user.relationship_id,  
                                    Address = user.address,
                                    Working = user.working,
                                    WorkingLink = user.working_link,
                                    About = user.about,
                                    School = user.school,
                                    Gender = user.gender,
                                    Birthday = user.birthday,
                                    Website = user.website,
                                    Facebook = user.facebook,
                                    Google = user.google,
                                    Twitter = user.twitter,
                                    Linkedin = user.linkedin,
                                    Youtube = user.youtube,
                                    Vk = user.vk,
                                    Instagram = user.instagram,
                                    Language = user.language,
                                    IpAddress = user.ip_address,
                                    FollowPrivacy = user.follow_privacy,
                                    FriendPrivacy = user.friend_privacy,
                                    PostPrivacy = user.post_privacy,
                                    MessagePrivacy = user.message_privacy,
                                    ConfirmFollowers = user.confirm_followers,
                                    ShowActivitiesPrivacy = user.show_activities_privacy,
                                    BirthPrivacy = user.birth_privacy,
                                    VisitPrivacy = user.visit_privacy,
                                    Lastseen = user.lastseen,
                                 
                                    SentmeMsg = user.e_sentme_msg,
                                    LastNotif = user.e_last_notif,
                                    Status = user.status,
                                    Active = user.active,
                                    Admin = user.admin,
                                    Registered = user.registered,
                                    PhoneNumber = user.phone_number,
                                    IsPro = user.is_pro,
                                    ProType = user.pro_type,
                              
                                    Timezone = user.timezone,
                                    Referrer = user.referrer,
                                    Balance = user.balance,
                                    PaypalEmail = user.paypal_email,
                                    NotificationsSound = user.notifications_sound,
                                    OrderPostsBy = user.order_posts_by,
                                    
                                    DeviceId = user.device_id,
                                    WebDeviceId = user.web_device_id,
                                    Wallet = user.wallet,
                                    Lat = user.lat,
                                    Lng = user.lng,
                                    LastDataUpdate = user.last_location_update,
                                    ShareMyLocation = user.share_my_location,
                                    Url = user.url,
                                    Name = user.name,
                                    LastseenUnixTime = user.lastseen_unix_time,
                  
                                    Details = new Details()
                                    {
                                        post_count = user.details.post_count,
                                        album_count = user.details.album_count,
                                        following_count = user.details.following_count,
                                        followers_count = user.details.followers_count,
                                        groups_count = user.details.groups_count,
                                        likes_count = user.details.likes_count,
                                    },
                                });

                                RunOnUiThread(() =>
                                {
                                    SupportActionBar.Title = result.user_data.name;

                                    //Online Or offline
                                    if (result.user_data.lastseen_status == "on")
                                    {
                                        SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Online);
                                        Lastseen = GetString(Resource.String.Lbl_Online);
                                    }
                                    else
                                    {
                                        SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Last_seen) + " " + IMethods.ITime.TimeAgo(int.Parse(result.user_data.lastseen_unix_time));
                                        Lastseen = GetString(Resource.String.Lbl_Last_seen) + " " + IMethods.ITime.TimeAgo(int.Parse(result.user_data.lastseen_unix_time));
                                    }
                                });
                            }
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errortext = error._errors.Error_text;
                           

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(this);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                        Console.WriteLine(error);
                      
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Get_UserProfileData_Api();
            }
        }

        public void loadData_ItemUser()
        {
            try
            {
                if (TypeChat == "LastMessenger")
                {
                    if (_datauser != null)
                    {
                        SupportActionBar.Title = _datauser.Name;

                        //Online Or offline
                        if (_datauser.Lastseen == "on")
                        {
                            SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Online);
                            Lastseen = GetString(Resource.String.Lbl_Online);
                        }
                        else
                        {
                            SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Last_seen) + " " + IMethods.ITime.TimeAgo(int.Parse(_datauser.LastseenUnixTime));
                            Lastseen = GetString(Resource.String.Lbl_Last_seen) + " " + IMethods.ITime.TimeAgo(int.Parse(_datauser.LastseenUnixTime));
                        }
                    }
                }
                else if (TypeChat == "Contact")
                {
                    if (_userContacts != null)
                    {
                        SupportActionBar.Title = _userContacts.Name;

                        //Online Or offline
                        if (_userContacts.Lastseen == "on")
                        {
                            SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Online);
                            Lastseen = GetString(Resource.String.Lbl_Online);
                        }
                        else
                        {
                            SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Last_seen) + " " + IMethods.ITime.TimeAgo(int.Parse(_userContacts.LastseenUnixTime));
                            Lastseen = GetString(Resource.String.Lbl_Last_seen) + " " + IMethods.ITime.TimeAgo(int.Parse(_userContacts.LastseenUnixTime));
                        }

                        MainChatColor = _userContacts.St_ChatColor;
                    }
                }
                else
                {
                    var dataSearchUserList = OnlineSearch_Activity.mAdapter?.mSearchUserList?.FirstOrDefault(a => a.UserId == Userid);
                    if (dataSearchUserList != null)
                    {
                        _SearchUser = dataSearchUserList;
                        SupportActionBar.Title = dataSearchUserList.Name;
                        //Online Or offline
                        if (dataSearchUserList.LastseenStatus == "on")
                        {
                            SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Online);
                            Lastseen = GetString(Resource.String.Lbl_Online);
                        }
                        else
                        {
                            var time = IMethods.ITime.TimeAgo(int.Parse(dataSearchUserList.LastseenUnixTime));

                            SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Last_seen) + " " + time;
                            Lastseen = GetString(Resource.String.Lbl_Last_seen) + " " + time;
                        }

                        MainChatColor = AppSettings.MainColor;
                    }
                }

                Get_Messages();

                ChatEmojiImage.Click += Chat_EmojiImage_Click;

                var Emojiicon = new EmojIconActions(this, RootView, EmojiconEditTextView, ChatEmojiImage);
                Emojiicon.ShowEmojIcon();
                EmojiconEditTextView.TextChanged += EmojiconEditTexte_TextChanged;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Update_One_Messeges(Classes.Message Message)
        {
            try
            {
                MAdapter.Update(Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void Get_Messages()
        {
            try
            {
                before_message_id = "0";
                MAdapter.Clear();

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.GetMessages_CredentialsList(UserDetails.User_id, Userid, before_message_id);

                var onScrollListener = new XamarinRecyclerViewOnScrollListener(MLayoutManager, swipeRefreshLayout);
                onScrollListener.LoadMoreEvent += OnScrollLoadMorefromTop_Event;
                ChatBoxListView.AddOnScrollListener(onScrollListener);

                if (localList == "1") //Database.. Get Messages Local
                {
                    MAdapter.BindEnd();

                    //Scroll Down >> 
                    ChatBoxListView.ScrollToPosition(MAdapter.mmessage.Count - 1);
                    swipeRefreshLayout.Refreshing = false;
                    swipeRefreshLayout.Enabled = false;
                }
                else //Or server.. Get Messages Api
                {
                    swipeRefreshLayout.Refreshing = true;
                    swipeRefreshLayout.Enabled = true;
                    GetMessages_Api();
                }

                TaskWork = "Working";

                //Run timer
                timer = new Timer();
                timer.Interval = AppSettings.MessageRequestSpeed;
                timer.Elapsed += TimerOnElapsed;
                timer.Enabled = true;
                timer.Start();

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                //Code get last Message id where Updater >>
                LastMessageid = MAdapter.mmessage.Last().M_id;
                RunOnUiThread(() => { MessageUpdater(); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Get Messages

        public async void GetMessages_Api()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var lastidmessage = "0";
                    var data = await API_Request.Get_User_Messages_Async(Userid, before_message_id, lastidmessage);
                    if (data != null)
                    {
                        foreach (var messageInfo in data)
                        {
                            Classes.Message m = new Classes.Message();

                            var mId = messageInfo["id"].ToString();
                            var fromId = messageInfo["from_id"].ToString(); // My user id
                            var groupId = messageInfo["group_id"].ToString();
                            var toId = messageInfo["to_id"].ToString();
                            var text = messageInfo["text"].ToString();
                            var longtext = messageInfo["text"].ToString();
                            var media = messageInfo["media"].ToString();
                            var mediaFileName = messageInfo["mediaFileName"].ToString();
                            var mediaFileNames = messageInfo["mediaFileNames"].ToString();
                            var time = messageInfo["time"].ToString();
                            var seen = messageInfo["seen"].ToString();
                            var deletedOne = messageInfo["deleted_one"].ToString();
                            var deletedTwo = messageInfo["deleted_two"].ToString();
                            var sentPush = messageInfo["sent_push"].ToString();
                            var notificationId = messageInfo["notification_id"].ToString();
                            var typeTwo = messageInfo["type_two"].ToString();
                            var stickers = IMethods.Fun_String.StringNullRemover(messageInfo["stickers"].ToString());
                            var timeText = messageInfo["time_text"].ToString();
                            var position = messageInfo["position"].ToString();
                            var type = messageInfo["type"].ToString();
                            var fileSize = messageInfo["file_size"].ToString();

                            JObject chatlistUserdata = JObject.FromObject(messageInfo);
                            var Blal = chatlistUserdata["messageUser"];
                            var avatar = Blal["avatar"].ToString();

                            m.M_id = mId;
                            m.from_id = fromId;
                            m.group_id = groupId;
                            m.to_id = toId;
                            var Decode = text.Contains("http");
                            if (Decode)
                            {
                                m.text = text;
                            }
                            else
                            {
                                m.text = IMethods.Fun_String.DecodeString(text);
                            }

                            m.media = media;
                            m.mediaFileName = mediaFileName;
                            m.mediaFileNames = mediaFileNames;
                            m.time = time;
                            m.seen = seen;
                            m.deleted_one = deletedOne;
                            m.deleted_two = deletedTwo;
                            m.sent_push = sentPush;
                            m.notification_id = notificationId;
                            m.type_two = typeTwo;
                            m.stickers = stickers;
                            m.time_text = timeText;
                            m.position = position;
                            m.type = type;
                            m.file_size = fileSize;
                            m.avatar = avatar;

                            m.ChatColor = MainChatColor;

                            if (type == "right_contact" || type == "left_contact")
                            {
                                string[] stringSeparators = new string[] { "&quot;" };
                                var name = longtext.Split(stringSeparators, StringSplitOptions.None);
                                var string_name = name[3];
                                var string_number = name[7];
                                m.ContactName = string_name;
                                m.ContactNumber = string_number;
                            }

                            MAdapter.Add(m);
                        }
                    }

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    // Insert data user in database
                    dbDatabase.Insert_Or_Replace_MessagesTable(MAdapter.mmessage);
                    dbDatabase.Dispose();

                    //Scroll Down >> 
                    ChatBoxListView.ScrollToPosition(MAdapter.mmessage.Count - 1);

                    swipeRefreshLayout.Refreshing = false;
                    swipeRefreshLayout.Enabled = false;
                }
                else
                {
                    swipeRefreshLayout.Refreshing = false;
                    swipeRefreshLayout.Enabled = false;
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                GetMessages_Api();
            }
        }

        #endregion

        #region Loadmore_Messages

        //Show Load More Event when scroll to the top Of Recycle
        private async void OnScrollLoadMorefromTop_Event(object sender, EventArgs e)
        {
            try
            {
                //Start Loader Get from Database or API Request >>
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                FirstMessageid = "";

                //Code get first Message id where LoadMore >>
                var mes = MAdapter.mmessage.FirstOrDefault();
                if (mes != null)
                {
                    FirstMessageid = mes.M_id;
                }

                if (FirstMessageid != "")
                {
                    var local = Loadmore_Messages_Database();
                    if (local == "1")
                    {

                    }
                    else
                    {
                        var API = await Loadmore_Messages_API(FirstMessageid);
                        if (API != null)
                        {

                        }
                        else
                        {
                            swipeRefreshLayout.Refreshing = false;
                            swipeRefreshLayout.Enabled = false;
                        }
                    }
                }

                swipeRefreshLayout.Refreshing = false;
                swipeRefreshLayout.Enabled = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public string Loadmore_Messages_Database()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var LocalList = dbDatabase.GetMessageList(UserDetails.User_id, Userid, FirstMessageid);
                if (LocalList?.Count > 0) //Database.. Get Messages Local
                {
                    LocalList = new List<DataTables.MessageTB>(LocalList.OrderByDescending(a => a.M_id));

                    foreach (var message in LocalList)
                    {
                        Classes.Message m = new Classes.Message
                        {
                            M_id = message.M_id,
                            from_id = message.from_id,
                            group_id = message.group_id,
                            to_id = message.to_id,
                            text = message.text,
                            media = message.media,
                            mediaFileName = message.mediaFileName,
                            mediaFileNames = message.mediaFileNames,
                            time = message.time,
                            seen = message.seen,
                            deleted_one = message.deleted_one,
                            deleted_two = message.deleted_two,
                            sent_push = message.sent_push,
                            notification_id = message.notification_id,
                            type_two = message.type_two,
                            stickers = message.stickers,
                            time_text = message.time_text,
                            position = message.position,
                            type = message.type,
                            file_size = message.file_size,
                            avatar = message.avatar,
                            ContactName = message.ContactName,
                            ContactNumber = message.ContactNumber,
                            ChatColor = MainChatColor,
                        };

                        RunOnUiThread(() => { MAdapter.Insert(m, FirstMessageid); });
                    }

                    dbDatabase.Dispose();
                    return "1";
                }
                else
                {
                    dbDatabase.Dispose();
                    return "0";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "0";
            }
        }

        public async Task<string> Loadmore_Messages_API(string before_MessageId)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var dataMes = await API_Request.Messages_Loadmore_Async(Userid, before_MessageId);
                    if (dataMes?.Count > 0)
                    {
                        var listAPI = new List<Classes.Message>();
                        foreach (var messageInfo in dataMes)
                        {
                            Classes.Message m = new Classes.Message();

                            var mId = messageInfo["id"].ToString();
                            var fromId = messageInfo["from_id"].ToString(); // user id
                            var groupId = messageInfo["group_id"].ToString();
                            var toId = messageInfo["to_id"].ToString();
                            var text = messageInfo["text"].ToString();
                            var longtext = messageInfo["text"].ToString();
                            var media = messageInfo["media"].ToString();
                            var mediaFileName = messageInfo["mediaFileName"].ToString();
                            var mediaFileNames = messageInfo["mediaFileNames"].ToString();
                            var time = messageInfo["time"].ToString();
                            var seen = messageInfo["seen"].ToString();
                            var deletedOne = messageInfo["deleted_one"].ToString();
                            var deletedTwo = messageInfo["deleted_two"].ToString();
                            var sentPush = messageInfo["sent_push"].ToString();
                            var notificationId = messageInfo["notification_id"].ToString();
                            var typeTwo = messageInfo["type_two"].ToString();
                            var stickers = IMethods.Fun_String.StringNullRemover(messageInfo["stickers"].ToString());
                            var timeText = messageInfo["time_text"].ToString();
                            var position = messageInfo["position"].ToString();
                            var type = messageInfo["type"].ToString();
                            var fileSize = messageInfo["file_size"].ToString();

                            JObject chatlistUserdata = JObject.FromObject(messageInfo);
                            var Blal = chatlistUserdata["messageUser"];
                            var avatar = Blal["avatar"].ToString();

                            m.M_id = mId;
                            m.from_id = fromId;
                            m.group_id = groupId;
                            m.to_id = toId;

                            var Decode = text.Contains("http");
                            if (Decode)
                            {
                                m.text = text;
                            }
                            else
                            {
                                m.text = IMethods.Fun_String.DecodeString(text);
                            }

                            m.media = media;
                            m.mediaFileName = mediaFileName;
                            m.mediaFileNames = mediaFileNames;
                            m.time = time;
                            m.seen = seen;
                            m.deleted_one = deletedOne;
                            m.deleted_two = deletedTwo;
                            m.sent_push = sentPush;
                            m.notification_id = notificationId;
                            m.type_two = typeTwo;
                            m.stickers = stickers;
                            m.time_text = timeText;
                            m.position = position;
                            m.type = type;
                            m.file_size = fileSize;
                            m.avatar = avatar;

                            m.ChatColor = MainChatColor;

                            if (type == "right_contact" || type == "left_contact")
                            {
                                string[] stringSeparators = new string[] { "&quot;" };
                                var name = longtext.Split(stringSeparators, StringSplitOptions.None);
                                var string_name = name[3];
                                var string_number = name[7];
                                m.ContactName = string_name;
                                m.ContactNumber = string_number;
                            }

                            listAPI.Add(m);
                        }

                        listAPI = new List<Classes.Message>(listAPI.OrderByDescending(a => a.M_id));
                        foreach (var mess in listAPI)
                        {
                            MAdapter.Insert(mess, FirstMessageid);
                        }

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        // Insert data user in database
                        dbDatabase.Insert_Or_Replace_MessagesTable(MAdapter.mmessage);
                        dbDatabase.Dispose();
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Loadmore_Messages_API(before_MessageId);
            }

            return null;
        }

        #endregion

        #region Updater_Messages

        public async void MessageUpdater()
        {
            try
            {
                if (TaskWork == "Working")
                {
                    TaskWork = "Stop";

                    if (IMethods.CheckConnectivity())
                    {
                     
                        var datacontent = await API_Request.Updater_Messages_Async(Userid, "0", LastMessageid);
                        if (datacontent != null)
                        {
                            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(datacontent);
                            try
                            {
                                var typping = data["typing"].ToString();
                                if (typping == "1")
                                {
                                    SupportActionBar.Subtitle = GetString(Resource.String.Lbl_Typping);
                                }
                                else
                                {
                                    SupportActionBar.Subtitle = Lastseen;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            string apiStatus = data["api_status"].ToString();
                            if (apiStatus == "200")
                            {
                                var messages = JObject.Parse(datacontent).SelectToken("messages").ToString();
                                JArray ChatMessages = JArray.Parse(messages);

                                if (ChatMessages.Count > 0)
                                {
                                    foreach (var messageInfo in ChatMessages)
                                    {
                                        Classes.Message m = new Classes.Message();

                                        var M_id = messageInfo["id"].ToString();
                                        var from_id = messageInfo["from_id"].ToString(); // user id
                                        var group_id = messageInfo["group_id"].ToString();
                                        var to_id = messageInfo["to_id"].ToString();
                                        var text = messageInfo["text"].ToString();
                                        var textlong = messageInfo["text"].ToString();
                                        var media = messageInfo["media"].ToString();
                                        var mediaFileName = messageInfo["mediaFileName"].ToString();
                                        var mediaFileNames = messageInfo["mediaFileNames"].ToString();
                                        var time = messageInfo["time"].ToString();
                                        var seen = messageInfo["seen"].ToString();
                                        var deleted_one = messageInfo["deleted_one"].ToString();
                                        var deleted_two = messageInfo["deleted_two"].ToString();
                                        var sent_push = messageInfo["sent_push"].ToString();
                                        var notification_id = messageInfo["notification_id"].ToString();
                                        var type_two = messageInfo["type_two"].ToString();
                                        var stickers = IMethods.Fun_String.StringNullRemover(messageInfo["stickers"].ToString());
                                        var time_text = messageInfo["time_text"].ToString();
                                        var position = messageInfo["position"].ToString();
                                        var type = messageInfo["type"].ToString();
                                        var file_size = messageInfo["file_size"].ToString();

                                        JObject ChatlistUserdata = JObject.FromObject(messageInfo);
                                        var Blal = ChatlistUserdata["messageUser"];
                                        var avatar = Blal["avatar"].ToString();
                                        var user_id = Blal["user_id"].ToString();

                                        m.M_id = M_id;
                                        m.from_id = from_id;
                                        m.group_id = group_id;
                                        m.to_id = to_id;

                                        var Decode = text.Contains("http");
                                        if (Decode)
                                        {
                                            m.text = text;
                                        }
                                        else
                                        {
                                            m.text = IMethods.Fun_String.DecodeString(text);
                                        }

                                        m.media = media;
                                        m.mediaFileName = mediaFileName;
                                        m.mediaFileNames = mediaFileNames;
                                        m.time = time;
                                        m.seen = seen;
                                        m.deleted_one = deleted_one;
                                        m.deleted_two = deleted_two;
                                        m.sent_push = sent_push;
                                        m.notification_id = notification_id;
                                        m.type_two = type_two;
                                        m.stickers = stickers;
                                        m.time_text = time_text;
                                        m.position = position;
                                        m.type = type;
                                        m.file_size = file_size;
                                        m.avatar = avatar;
                                        m.ChatColor = MainChatColor;
                                        if (type == "right_contact" || type == "left_contact")
                                        {
                                            string[] stringSeparators = new string[] { "&quot;" };
                                            var name = textlong.Split(stringSeparators, StringSplitOptions.None);
                                            var string_name = name[3];
                                            var string_number = name[7];
                                            m.ContactName = string_name;
                                            m.ContactNumber = string_number;
                                        }

                                        var check = MAdapter.mmessage.FirstOrDefault(a => a.M_id == m.M_id);
                                        if (check == null)
                                        {
                                            RunOnUiThread(() => {

                                                MAdapter.Add(m);

                                                var dataUser = Last_Messages_Fragment.mAdapter.MLastMessagesUser?.FirstOrDefault(a => a.UserId == from_id);
                                                if (dataUser != null)
                                                {
                                                    dataUser.UserId = user_id;
                                                    dataUser.ProfilePicture = avatar;
                                                    dataUser.ChatColor = MainChatColor;

                                                    //last_message
                                                    dataUser.LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                                    {
                                                        Id = M_id,
                                                        FromId = from_id,
                                                        GroupId = group_id,
                                                        ToId = to_id,
                                                        Text = text,
                                                        Media = media,
                                                        MediaFileName = mediaFileName,
                                                        MediaFileNames = mediaFileNames,
                                                        Time = time,
                                                        Seen = "1",
                                                        DeletedOne = deleted_one,
                                                        DeletedTwo = deleted_two,
                                                        SentPush = sent_push,
                                                        NotificationId = notification_id,
                                                        TypeTwo = type_two,
                                                        Stickers = stickers,
                                                        // DateTime = dateTime,
                                                    };

                                                    Last_Messages_Fragment.mAdapter.Move(dataUser);
                                                    Last_Messages_Fragment.mAdapter.Update(dataUser);
                                                }

                                                if (SettingsPrefsFragment.S_SoundControl)
                                                    IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_GetMesseges.mp3");
                                            });
                                        }
                                    }

                                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                    // Insert data user in database
                                    dbDatabase.Insert_Or_Replace_MessagesTable(MAdapter.mmessage);
                                    dbDatabase.Dispose();
                                }
                                else
                                {
                                    TaskWork = "Working";
                                }
                            }
                        }
                    }

                    TaskWork = "Working";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                TaskWork = "Working";
            }
        }

        #endregion

        #region Event

        //Copy text
        private void Copytext_OnItemLongClick(object sender, Message_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = MAdapter.GetItem(Position);
                    if (item != null)
                    {
                        var clipboardManager = (ClipboardManager)this.GetSystemService(Context.ClipboardService);

                        ClipData clipData = ClipData.NewPlainText("text", item.text);
                        clipboardManager.PrimaryClip = clipData;

                        Toast.MakeText(this, GetString(Resource.String.Lbl_Copied), ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Event sent media (image , video , file , music )
        private void ChatMediaButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (AppSettings.Show_Button_image)
                    arrayAdapter.Add(GetText(Resource.String.Btn_Image));
                if (AppSettings.Show_Button_video)
                    arrayAdapter.Add(GetText(Resource.String.Btn_Video));
                if (AppSettings.Show_Button_attachfile)
                    arrayAdapter.Add(GetText(Resource.String.Lbl_File));
                if (AppSettings.Show_Button_Music)
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Music));
                if (AppSettings.Show_Button_Gif)
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Gif));

                dialogList.Title(GetString(Resource.String.Lbl_Select_what_you_want));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                switch (itemId)
                {
                    // image 
                    case 0:
                        Chat_ImageButton();
                        break;
                    // video 
                    case 1:
                        Chat_VideoButton();
                        break;
                    // File 
                    case 2:
                        Chat_FileButton();
                        break;
                    // Music 
                    case 3:
                        Chat_MusicButton();
                        break;
                    // Gif 
                    case 4:
                        Chat_GifButton();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Chat_GifButton()
        {
            try
            {
                StartActivityForResult(new Intent(this, typeof(Gif_Activity)), 300);
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

        // Send File
        private void Chat_FileButton()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    var fileIntent = new Intent(Intent.ActionPick);
                    fileIntent.SetAction(Intent.ActionGetContent);
                    fileIntent.SetAction(Intent.ActionOpenDocument);
                    fileIntent.SetType("application/*");
                    StartActivityForResult(Intent.CreateChooser(fileIntent, GetText(Resource.String.Lbl_SelectFile)), 208);
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        var fileIntent = new Intent(Intent.ActionPick);
                        fileIntent.SetAction(Intent.ActionGetContent);
                        fileIntent.SetAction(Intent.ActionOpenDocument);
                        fileIntent.SetType("application/*");
                        StartActivityForResult(Intent.CreateChooser(fileIntent, GetText(Resource.String.Lbl_SelectFile)), 208);
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage
                        }, 208);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Send Music
        public void Chat_MusicButton()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                  
                    var intent = new Intent(Intent.ActionPick);
                    intent.SetAction(Intent.ActionView);
                    intent.SetType("audio/*");
                    StartActivityForResult(intent, 209);
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage
                        }, 209);
                    }
                    else
                    {
                        var intent = new Intent(Intent.ActionPick);
                        intent.SetAction(Intent.ActionView);
                        intent.SetType("audio/*");
                        StartActivityForResult(intent, 209);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void RequestPermission(int idPermissions)
        {
            if (idPermissions == 101)
            {
                RequestPermissions(new String[] {
                    Manifest.Permission.ReadExternalStorage,
                        Manifest.Permission.WriteExternalStorage,
                }, 101);
            }
            else if (idPermissions == 102)
            {
                RequestPermissions(new String[] {              
                    Manifest.Permission.ReadContacts,
                        Manifest.Permission.ReadPhoneNumbers,
                }, 102);
            }
            else if (idPermissions == 103)
            {
                RequestPermissions(new String[] {
                    Manifest.Permission.RecordAudio,
                        Manifest.Permission.ModifyAudioSettings,
                }, 103);
            }
            else if (idPermissions == 104)
            {
                RequestPermissions(new String[] {
                    Manifest.Permission.Camera,
                }, 104);
            }
            else if (idPermissions == 106)
            {
                RequestPermissions(new String[] {
                    Manifest.Permission.RecordAudio,
                        Manifest.Permission.ModifyAudioSettings,
                }, 106);
            }
            else if (idPermissions == 107)
            {
                RequestPermissions(new String[] {
                    Manifest.Permission.Camera,
                        Manifest.Permission.RecordAudio,
                        Manifest.Permission.ModifyAudioSettings,
                }, 107);
            }

        }

        private void Chat_contactButton_Click(object sender, EventArgs e)
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Intent pickcontact = new Intent(Intent.ActionPick, ContactsContract.Contacts.ContentUri);
                    pickcontact.SetType(ContactsContract.CommonDataKinds.Phone.ContentType);
                    StartActivityForResult(pickcontact, 3);
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.ReadContacts) == Permission.Granted)
                    {
                        Intent pickcontact = new Intent(Intent.ActionPick, ContactsContract.Contacts.ContentUri);
                        pickcontact.SetType(ContactsContract.CommonDataKinds.Phone.ContentType);
                        StartActivityForResult(pickcontact, 3);
                    }
                    else
                    {
                        RequestPermission(102);

                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 102) //Contacts 
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Intent pickcontact = new Intent(Intent.ActionPick, ContactsContract.Contacts.ContentUri);
                        pickcontact.SetType(ContactsContract.CommonDataKinds.Phone.ContentType);
                        StartActivityForResult(pickcontact, 3);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 103) //Sound Record
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {

                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 104) //Image Picker
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimImage, IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4).SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(GetString(Resource.String.Lbl_Done))
                            .SetActivityTitle(GetString(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 106) //Audio Call
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        StartCall();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 107) //Video call
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        StartVideoCall();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 208) //File
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var fileIntent = new Intent(Intent.ActionPick);
                        fileIntent.SetAction(Intent.ActionGetContent);
                        fileIntent.SetAction(Intent.ActionOpenDocument);
                        fileIntent.SetType("application/*");
                        StartActivityForResult(fileIntent, 208);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 209) //Music
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var intent = new Intent(Intent.ActionPick);
                        intent.SetAction(Intent.ActionGetContent);
                        intent.SetType("audio/*");
                        StartActivityForResult(intent, 209);
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Send video
        private void Chat_VideoButton()
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

        // Send image
        private void Chat_ImageButton()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    //Open Image 
                    Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimImage, IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4).SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(GetString(Resource.String.Lbl_Done))
                        .SetActivityTitle(GetString(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                    return;
                }
                else
                {
                    if (CropImage.IsExplicitCameraPermissionRequired(this))
                    {
                        RequestPermission(104);

                    }
                    else
                    {
                        //Open Image 
                        Android.Net.Uri myUri = Android.Net.Uri.FromFile(new File(IMethods.IPath.FolderDcimImage, IMethods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder().SetInitialCropWindowPaddingRatio(0).SetAutoZoomEnabled(true).SetMaxZoom(4).SetGuidelines(CropImageView.Guidelines.On).SetCropMenuCropButtonTitle(GetString(Resource.String.Lbl_Done))
                            .SetActivityTitle(GetString(Resource.String.Lbl_Crop)).SetOutputUri(myUri).Start(this);
                        return;
                    }

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Send Message type => "right_audio" Or "right_text"
        public void OnClick_OfSendButton()
        {
            try
            {
                isrecording = false;

                unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var time2 = unixTimestamp.ToString();

                if (ChatSendButton.Tag.ToString() == "Audio")
                {
                    var interplator = new FastOutSlowInInterpolator();
                    TopFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                    SupportFragmentManager.BeginTransaction().Remove(Chat_Recourd_Sound_BoxFragment).Commit();

                    string filePath = RecorderService.GetRecorded_Sound_Path();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        Classes.Message m1 = new Classes.Message
                        {
                            M_id = time2,
                            from_id = UserDetails.User_id,
                            to_id = Userid,
                            media = filePath,
                            position = "right",
                            time_text = this.GetText(Resource.String.Lbl_Uploading),
                            MediaDuration = IMethods.AudioRecorderAndPlayer.GetTimeString(IMethods.AudioRecorderAndPlayer.Get_MediaFileDuration(filePath)),
                            type = "right_audio"
                        };

                        MAdapter.Add(m1);

                        //Here on This function will send Selected audio file to the user 
                        if (IMethods.CheckConnectivity())
                        {
                            Task.Run(() =>
                            {
                                MessageController.SendMessageTask(Userid, time2, EmojiconEditTextView.Text, "", filePath).ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                        }
                    }

                    ChatSendButton.Tag = "Free";
                    ChatSendButton.SetImageResource(Resource.Drawable.microphone);

                }
                else if (ChatSendButton.Tag.ToString() == "Text")
                {
                    if (String.IsNullOrEmpty(EmojiconEditTextView.Text))
                    {

                    }
                    else
                    {
                        //Here on This function will send Text Messages to the user 

                        //remove \n in a string
                        string replacement = Regex.Replace(EmojiconEditTextView.Text, @"\t|\n|\r", "");

                        Classes.Message m1 = new Classes.Message
                        {
                            M_id = time2,
                            from_id = UserDetails.User_id,
                            to_id = Userid,
                            text = replacement,
                            position = "right",
                            type = "right_text",
                            time_text = TimeNow
                        };

                        MAdapter.Add(m1);

                        if (IMethods.CheckConnectivity())
                        {
                            MessageController.SendMessageTask(Userid, time2, EmojiconEditTextView.Text).ConfigureAwait(false);
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                        }

                        EmojiconEditTextView.Text = "";
                    }

                    if (AppSettings.ShowButton_RecourdSound)
                    {
                        ChatSendButton.SetImageResource(Resource.Drawable.microphone);
                        ChatSendButton.Tag = "Free";
                    }
                    else
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Send Message type => "right_image" Or Video
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                TimeNow = DateTime.Now.ToString("hh:mm");
                unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var time2 = unixTimestamp.ToString();

                //If its from Camera or Gallery   
                if (requestCode == CropImage.CropImageActivityRequestCode && resultCode == Result.Ok)
                {
                    CropImage.ActivityResult result = CropImage.GetActivityResult(data);

                    if (result.IsSuccessful)
                    {
                        Android.Net.Uri resultUri = result.Uri;

                        if (!string.IsNullOrEmpty(resultUri.Path))
                        {
                            Classes.Message m1 = new Classes.Message
                            {
                                M_id = time2,
                                from_id = UserDetails.User_id,
                                to_id = Userid,
                                media = resultUri.Path,
                                position = "right",
                                type = "right_image",
                                time_text = TimeNow
                            };
                            MAdapter.Add(m1);

                            //Send image function
                            if (IMethods.CheckConnectivity())
                            {
                                Task.Run(() =>
                                {
                                    MessageController.SendMessageTask(Userid, time2, EmojiconEditTextView.Text, "", resultUri.Path).ConfigureAwait(false);
                                });
                            }
                            else
                            {
                                Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details) + " ", ToastLength.Long).Show();
                        }
                    }
                }
                else if (requestCode == 100 && resultCode == Result.Ok)
                {
                    string FullPath = IMethods.MultiMedia.GetRealVideoPathFromURI(data.Data);
                    if (!string.IsNullOrEmpty(FullPath))
                    {
                        var NewCopyedFilepath = IMethods.MultiMedia.CopyMediaFileTo(FullPath, IMethods.IPath.FolderDcimVideo, false, true);
                        if (NewCopyedFilepath != "Path File Dont exits")
                        {
                            Classes.Message m1 = new Classes.Message
                            {
                                M_id = time2,
                                from_id = UserDetails.User_id,
                                to_id = Userid,
                                media = NewCopyedFilepath,
                                position = "right",
                                type = "right_video",
                                time_text = TimeNow
                            };
                            MAdapter.Add(m1);

                            //Send Video function
                            if (IMethods.CheckConnectivity())
                            {
                                Task.Run(() =>
                                {
                                    MessageController.SendMessageTask(Userid, time2, EmojiconEditTextView.Text, "", NewCopyedFilepath).ConfigureAwait(false);
                                });
                            }
                            else
                            {
                                Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details) + " ", ToastLength.Long).Show();
                        }
                    }
                }
                else if (requestCode == 3 && resultCode == Result.Ok)
                {
                    try
                    {
                        var Contact = IMethods.PhoneContactManager.Get_ContactInfoBy_Id(data.Data.LastPathSegment);
                        if (Contact != null)
                        {
                            var Name = Contact.UserDisplayName;
                            var Phone = Contact.PhoneNumber;

                            Classes.Message m1 = new Classes.Message
                            {
                                M_id = time2,
                                from_id = UserDetails.User_id,
                                to_id = Userid,
                                ContactName = Name,
                                ContactNumber = Phone,
                                time_text = TimeNow,
                                position = "right",
                                type = "right_contact"
                            };
                            MAdapter.Add(m1);

                            var Dictionary = new Dictionary<string, string>();

                            if (!Dictionary.ContainsKey(Name))
                            {
                                Dictionary.Add(Name, Phone);
                            }

                            string Data_contact = JsonConvert.SerializeObject(Dictionary.ToArray().FirstOrDefault(a => a.Key == Name));

                            if (IMethods.CheckConnectivity())
                            {
                                //Send contact function
                                MessageController.SendMessageTask(Userid, time2, Data_contact, "1").ConfigureAwait(false);
                            }
                            else
                            {
                                Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (requestCode == 208)
                {
                    string filepath = IMethods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        string totalSize = IMethods.Fun_String.Format_byte_size(filepath);
                        Classes.Message m1 = new Classes.Message
                        {
                            M_id = time2,
                            from_id = UserDetails.User_id,
                            to_id = Userid,
                            media = filepath,
                            file_size = totalSize,
                            time_text = TimeNow,
                            position = "right",
                            type = "right_file"
                        };
                        MAdapter.Add(m1);

                        //Send Video function
                        if (IMethods.CheckConnectivity())
                        {
                            Task.Run(() =>
                            {
                                MessageController.SendMessageTask(Userid, time2, EmojiconEditTextView.Text, "", filepath).ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 209 && resultCode == Result.Ok)// Music
                {
                    var filepath = IMethods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var type = IMethods.AttachmentFiles.Check_FileExtension(filepath);
                        if (type == "Audio")
                        {
                            var NewCopyedFilepath = IMethods.MultiMedia.CopyMediaFileTo(filepath, IMethods.IPath.FolderDcimSound, false, true);
                            if (NewCopyedFilepath != "Path File Dont exits")
                            {
                                string totalSize = IMethods.Fun_String.Format_byte_size(filepath);
                                Classes.Message m1 = new Classes.Message
                                {
                                    M_id = time2,
                                    from_id = UserDetails.User_id,
                                    to_id = Userid,
                                    media = NewCopyedFilepath,
                                    file_size = totalSize,
                                    position = "right",
                                    time_text = this.GetText(Resource.String.Lbl_Uploading),
                                    MediaDuration = IMethods.AudioRecorderAndPlayer.GetTimeString(IMethods.AudioRecorderAndPlayer.Get_MediaFileDuration(filepath)),
                                    type = "right_audio",
                                };

                                MAdapter.Add(m1);

                                //Send Video function
                                if (IMethods.CheckConnectivity())
                                {
                                    Task.Run(() =>
                                    {
                                        MessageController.SendMessageTask(Userid, time2, "", "", filepath).ConfigureAwait(false);
                                    });
                                }
                                else
                                {
                                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Long).Show();
                                }
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                    }
                }
                else if (requestCode == 300 && resultCode == Result.Ok)
                {
                    // G_fixed_height_small_url, // UrlGif - view  >>  mediaFileName
                    // G_fixed_height_small_mp4, //MediaGif - sent >>  media

                    var gifLink = data.GetStringExtra("MediaGif") ?? "Data not available";
                    if (gifLink != "Data not available" && !string.IsNullOrEmpty(gifLink))
                    {
                        var gifUrl = data.GetStringExtra("UrlGif") ?? "Data not available";
                        GifFile = gifLink;

                        Classes.Message m1 = new Classes.Message
                        {
                            M_id = time2,
                            from_id = UserDetails.User_id,
                            to_id = Userid,
                            media = GifFile,
                            mediaFileName = gifUrl,
                            position = "right",
                            type = "right_gif",
                            time_text = TimeNow
                        };

                        MAdapter.Add(m1);

                        //Send image function
                        if (IMethods.CheckConnectivity())
                        {
                            Task.Run(() =>
                            {
                                MessageController.SendMessageTask(Userid, time2, EmojiconEditTextView.Text, "", "", "", "", GifFile).ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details) + " ", ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Chat_sendButton_Touch(object sender, View.TouchEventArgs e)
        {
            var handled = false;

            if (e.Event.Action == MotionEventActions.Down)
            {
                OnClick_OfSendButton();

                handled = false;
            }

            if (e.Event.Action == MotionEventActions.Up)
            {
                try
                {
                    if (isrecording)
                    {
                        RecorderService.StopRecourding();
                        var filePath = RecorderService.GetRecorded_Sound_Path();

                        ChatSendButton.SetColor(Color.ParseColor(MainChatColor));
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);

                        if (EmojiconEditTextView.Text == GetString(Resource.String.Lbl_Recording))
                        {
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                Bundle bundle = new Bundle();
                                bundle.PutString("FilePath", filePath);
                                Chat_Recourd_Sound_BoxFragment.Arguments = bundle;
                                ReplaceTopFragment(Chat_Recourd_Sound_BoxFragment);
                            }

                            EmojiconEditTextView.Text = "";
                            EmojiconEditTextView.SetTextColor(Color.ParseColor("#444444"));
                        }

                        isrecording = false;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

                ChatSendButton.Pressed = false;
                handled = true;
            }

            e.Handled = handled;
        }

        private void Chat_colorButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChatColorButton.Tag.ToString() == "Closed")
                {
                    ResetButtonTags();
                    ChatColorButton.Tag = "Opened";
                    ChatColorButton.Drawable.SetTint(Color.ParseColor(AppSettings.MainColor));
                    ReplaceButtomFragment(ChatColorBoxFragment);
                }
                else
                {
                    ResetButtonTags();
                    ChatColorButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    TopFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                    SupportFragmentManager.BeginTransaction().Remove(ChatColorBoxFragment).Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //record voices
        private async void Chat_sendButton_LongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    if (ChatSendButton.Tag.ToString() == "Free")
                    {
                        //Set Recourd Style
                        isrecording = true;

                        if (SettingsPrefsFragment.S_SoundControl)
                            IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("RecourdVoiceButton.mp3");

                        if (EmojiconEditTextView.Text != GetString(Resource.String.Lbl_Recording))
                        {
                            EmojiconEditTextView.Text = GetString(Resource.String.Lbl_Recording);
                            EmojiconEditTextView.SetTextColor(Android.Graphics.Color.ParseColor("#FA3C4C"));
                        }

                        ChatSendButton.SetColor(Android.Graphics.Color.ParseColor("#FA3C4C"));
                        ChatSendButton.SetImageResource(Resource.Drawable.ic_stop_white_24dp);

                        RecorderService = new IMethods.AudioRecorderAndPlayer(AppSettings.Application_Name);
                        //Start Audio record
                        await Task.Delay(600);
                        RecorderService.StartRecourding();
                    }

                    return;
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted)
                    {
                        if (ChatSendButton.Tag.ToString() == "Free")
                        {
                            //Set Record Style
                            isrecording = true;

                            if (SettingsPrefsFragment.S_SoundControl)
                                IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("RecourdVoiceButton.mp3");

                            if (EmojiconEditTextView.Text != GetString(Resource.String.Lbl_Recording))
                            {
                                EmojiconEditTextView.Text = GetString(Resource.String.Lbl_Recording);
                                EmojiconEditTextView.SetTextColor(Color.ParseColor("#FA3C4C"));
                            }

                            ChatSendButton.SetColor(Color.ParseColor("#FA3C4C"));
                            ChatSendButton.SetImageResource(Resource.Drawable.ic_stop_white_24dp);

                            RecorderService = new IMethods.AudioRecorderAndPlayer(AppSettings.Application_Name);
                            //Start Audio record
                            await Task.Delay(600);
                            RecorderService.StartRecourding();
                        }

                        return;
                    }
                    else
                    {
                        RequestPermission(103);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Chat_stickerButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChatStickerButton.Tag.ToString() == "Closed")
                {
                    ResetButtonTags();
                    ChatStickerButton.Tag = "Opened";
                    ChatStickerButton.Drawable.SetTint(Android.Graphics.Color.ParseColor(AppSettings.MainColor));
                    ReplaceButtomFragment(Chat_StickersTab_BoxFragment);
                }
                else
                {
                    ResetButtonTags();
                    ChatStickerButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#888888"));
                    TopFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                    SupportFragmentManager.BeginTransaction().Remove(Chat_StickersTab_BoxFragment).Commit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void EmojiconEditTexte_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (AppSettings.ShowButton_RecourdSound)
                {
                    if (!ButtomFragmentHolder.TranslationY.Equals(1200))
                        ButtomFragmentHolder.TranslationY = 1200;

                    if (isrecording && EmojiconEditTextView.Text == GetString(Resource.String.Lbl_Recording))
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
                    }
                    else if (!string.IsNullOrEmpty(EmojiconEditTextView.Text))
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
                    }
                    else if (isrecording)
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
                    }
                    else
                    {
                        ChatSendButton.Tag = "Free";
                        ChatSendButton.SetImageResource(Resource.Drawable.microphone);
                    }
                }
                else
                {
                    ChatSendButton.Tag = "Text";
                    ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Chat_EmojiImage_Click(object sender, EventArgs e)
        {

        }

        #endregion

        public void SetTheme(string color)
        {
            try
            {
                if (color.Contains("b582af"))
                {
                    SetTheme(Resource.Style.Chatththemeb582af);
                }
                else if (color.Contains("a84849"))
                {
                    SetTheme(Resource.Style.Chatththemea84849);
                }
                else if (color.Contains("f9c270"))
                {
                    SetTheme(Resource.Style.Chatththemef9c270);
                }
                else if (color.Contains("70a0e0"))
                {
                    SetTheme(Resource.Style.Chatththeme70a0e0);
                }
                else if (color.Contains("56c4c5"))
                {
                    SetTheme(Resource.Style.Chatththeme56c4c5);
                }
                else if (color.Contains("f33d4c"))
                {
                    SetTheme(Resource.Style.Chatththemef33d4c);
                }
                else if (color.Contains("a1ce79"))
                {
                    SetTheme(Resource.Style.Chatththemea1ce79);
                }
                else if (color.Contains("a085e2"))
                {
                    SetTheme(Resource.Style.Chatththemea085e2);
                }
                else if (color.Contains("ed9e6a"))
                {
                    SetTheme(Resource.Style.Chatththemeed9e6a);
                }
                else if (color.Contains("2b87ce"))
                {
                    SetTheme(Resource.Style.Chatththeme2b87ce);
                }
                else if (color.Contains("f2812b"))
                {
                    SetTheme(Resource.Style.Chatththemef2812b);
                }
                else if (color.Contains("0ba05d"))
                {
                    SetTheme(Resource.Style.Chatththeme0ba05d);
                }
                else if (color.Contains("0e71ea"))
                {
                    SetTheme(Resource.Style.Chatththeme0e71ea);
                }
                else if (color.Contains("aa2294"))
                {
                    SetTheme(Resource.Style.Chatththemeaa2294);
                }
                else if (color.Contains("f9a722"))
                {
                    SetTheme(Resource.Style.Chatththemef9a722);
                }
                else if (color.Contains("008484"))
                {
                    SetTheme(Resource.Style.Chatththeme008484);
                }
                else if (color.Contains("5462a5"))
                {
                    SetTheme(Resource.Style.Chatththeme5462a5);
                }
                else if (color.Contains("fc9cde"))
                {
                    SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("fc9cde"))
                {
                    SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("51bcbc"))
                {
                    SetTheme(Resource.Style.Chatththeme51bcbc);
                }
                else if (color.Contains("c9605e"))
                {
                    SetTheme(Resource.Style.Chatththemec9605e);
                }
                else if (color.Contains("01a5a5"))
                {
                    SetTheme(Resource.Style.Chatththeme01a5a5);
                }
                else if (color.Contains("056bba"))
                {
                    SetTheme(Resource.Style.Chatththeme056bba);
                }
                else
                {
                    //Default Color >> AppSettings.MainColor
                    SetTheme(Resource.Style.Chatththemedefault);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ResetButtonTags()
        {
            try
            {
                ChatStickerButton.Tag = "Closed";
                ChatColorButton.Tag = "Closed";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Top Menu

        public void OnMenuPhoneCallIcon_Click()
        {
            bool granted = ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.RecordAudio) ==
                Permission.Granted;
            if (granted)
            {
                StartCall();
            }
            else
            {
                RequestPermission(106);
            }

        }

        public void StartCall()
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

            if (_datauser != null)
            {
                IntentOfvideoCall.PutExtra("UserID", _datauser.UserId);
                IntentOfvideoCall.PutExtra("avatar", _datauser.ProfilePicture);
                IntentOfvideoCall.PutExtra("name", _datauser.Name);
                IntentOfvideoCall.PutExtra("time", TimeNow);
                IntentOfvideoCall.PutExtra("CallID", Time);
                IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("from_id", "0");
                IntentOfvideoCall.PutExtra("active", "0");
                IntentOfvideoCall.PutExtra("status", "0");
                IntentOfvideoCall.PutExtra("room_name", "TestRoom");
            }
            else if (_userContacts != null)
            {
                IntentOfvideoCall.PutExtra("UserID", _userContacts.UserId);
                IntentOfvideoCall.PutExtra("avatar", _userContacts.Avatar);
                IntentOfvideoCall.PutExtra("name", _userContacts.Name);
                IntentOfvideoCall.PutExtra("time", TimeNow);
                IntentOfvideoCall.PutExtra("CallID", Time);
                IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("from_id", "0");
                IntentOfvideoCall.PutExtra("active", "0");
                IntentOfvideoCall.PutExtra("status", "0");
                IntentOfvideoCall.PutExtra("room_name", "TestRoom");
            }
            else
            {
                IntentOfvideoCall.PutExtra("UserID", _SearchUser.UserId);
                IntentOfvideoCall.PutExtra("avatar", _SearchUser.Avatar);
                IntentOfvideoCall.PutExtra("name", _SearchUser.Name);
                IntentOfvideoCall.PutExtra("time", TimeNow);
                IntentOfvideoCall.PutExtra("CallID", Time);
                IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("from_id", "0");
                IntentOfvideoCall.PutExtra("active", "0");
                IntentOfvideoCall.PutExtra("status", "0");
                IntentOfvideoCall.PutExtra("room_name", "TestRoom");
            }

            StartActivity(IntentOfvideoCall);
        }

        public void StartVideoCall()
        {

            TimeNow = DateTime.Now.ToString("hh:mm");
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Time = Convert.ToString(unixTimestamp);

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

            if (_datauser != null)
            {
                IntentOfvideoCall.PutExtra("UserID", _datauser.UserId);
                IntentOfvideoCall.PutExtra("avatar", _datauser.ProfilePicture);
                IntentOfvideoCall.PutExtra("name", _datauser.Name);
                IntentOfvideoCall.PutExtra("time", TimeNow);
                IntentOfvideoCall.PutExtra("CallID", Time);
                IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("from_id", "0");
                IntentOfvideoCall.PutExtra("active", "0");
                IntentOfvideoCall.PutExtra("status", "0");
                IntentOfvideoCall.PutExtra("room_name", "TestRoom");
            }
            else if (_userContacts != null)
            {
                IntentOfvideoCall.PutExtra("UserID", _userContacts.UserId);
                IntentOfvideoCall.PutExtra("avatar", _userContacts.Avatar);
                IntentOfvideoCall.PutExtra("name", _userContacts.Name);
                IntentOfvideoCall.PutExtra("time", TimeNow);
                IntentOfvideoCall.PutExtra("CallID", Time);
                IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("from_id", "0");
                IntentOfvideoCall.PutExtra("active", "0");
                IntentOfvideoCall.PutExtra("status", "0");
                IntentOfvideoCall.PutExtra("room_name", "TestRoom");
            }
            else
            {
                IntentOfvideoCall.PutExtra("UserID", _SearchUser.UserId);
                IntentOfvideoCall.PutExtra("avatar", _SearchUser.Avatar);
                IntentOfvideoCall.PutExtra("name", _SearchUser.Name);
                IntentOfvideoCall.PutExtra("time", TimeNow);
                IntentOfvideoCall.PutExtra("CallID", Time);
                IntentOfvideoCall.PutExtra("access_token", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                IntentOfvideoCall.PutExtra("from_id", "0");
                IntentOfvideoCall.PutExtra("active", "0");
                IntentOfvideoCall.PutExtra("status", "0");
                IntentOfvideoCall.PutExtra("room_name", "TestRoom");
            }

            StartActivity(IntentOfvideoCall);

        }

        public void OnMenuVideoCallIcon_Click()
        {

            bool granted = ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) == Permission.Granted && ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.RecordAudio) == Permission.Granted;
            if (granted)
            {
                StartVideoCall();
            }
            else
            {
                RequestPermission(107);
            }
        }

        //view Profile action!
        public void OnMenuViewProfile_Click()
        {
            try
            {
                Intent Int;
                if (Userid != UserDetails.User_id)
                {
                    Int = new Intent(this, typeof(UserProfile_Activity));
                    Int.PutExtra("UserId", Userid);
                    Int.AddFlags(ActivityFlags.NewTask);
                    if (_datauser != null)
                    {
                        Int.PutExtra("UserType", "Chat_User");
                        Int.PutExtra("UserItem", JsonConvert.SerializeObject(_datauser));
                    }
                    else if (_userContacts != null)
                    {
                        Int.PutExtra("UserType", "Chat_UserContacts");
                        Int.PutExtra("UserItem", JsonConvert.SerializeObject(_userContacts));
                    }
                    else if (_SearchUser != null)
                    {
                        Int.PutExtra("UserType", "Chat_SearchUser");
                        Int.PutExtra("UserItem", JsonConvert.SerializeObject(_SearchUser));
                    }
                }
                else
                {
                    Int = new Intent(this, typeof(MyProfile_Activity));
                    Int.PutExtra("UserId", Userid);
                }

                StartActivity(Int);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnMenuBlock_Click()
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    var data = Global.Block_User(Userid, false).ConfigureAwait(true); //true >> "block"
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Blocked_successfully), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnMenuClearChat_Click()
        {
            try
            {
                MAdapter.RemoveAll();

                var UsertoDelete = Last_Messages_Fragment.mAdapter.MLastMessagesUser?.FirstOrDefault(a => a.UserId == Userid);
                if (UsertoDelete != null)
                {
                    Last_Messages_Fragment.mAdapter.Remove(UsertoDelete);
                }

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                dbDatabase.DeleteAllMessagesUser(UserDetails.User_id, Userid);
                dbDatabase.Dispose();

                if (IMethods.CheckConnectivity())
                {
                    var data = Global.Delete_Conversation(Userid).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                case Resource.Id.menu_phonecall:
                    OnMenuPhoneCallIcon_Click();
                    break;
                case Resource.Id.menu_videocall:
                    OnMenuVideoCallIcon_Click();
                    break;
                case Resource.Id.menu_view_profile:
                    OnMenuViewProfile_Click();
                    break;
                case Resource.Id.menu_block:
                    OnMenuBlock_Click();
                    break;
                case Resource.Id.menu_clear_chat:
                    OnMenuClearChat_Click();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Chat_Window_Menu, menu);

            if (AppSettings.Enable_Audio_Video_Call == false)
            {
                if (AppSettings.Enable_Video_Call == false)
                {
                    var video = menu.FindItem(Resource.Id.menu_videocall);
                    video.SetVisible(false);
                }

                if (AppSettings.Enable_Audio_Call == false)
                {
                    var phonecall = menu.FindItem(Resource.Id.menu_phonecall);
                    phonecall.SetVisible(false);
                }
            }

            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
                ResetButtonTags();
                ChatColorButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#888888"));
                ChatStickerButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#888888"));

                if (SupportFragmentManager.Fragments.Count > 0)
                {
                    var FragmentManager = SupportFragmentManager.BeginTransaction();
                    foreach (var vrg in SupportFragmentManager.Fragments)
                    {
                        if (SupportFragmentManager.Fragments.Contains(ChatColorBoxFragment))
                        {
                            FragmentManager.Remove(ChatColorBoxFragment);
                        }
                        else if (SupportFragmentManager.Fragments.Contains(Chat_StickersTab_BoxFragment))
                        {
                            FragmentManager.Remove(Chat_StickersTab_BoxFragment);
                        }
                    }

                    FragmentManager.Commit();
                }
            }
            else
            {
                base.OnBackPressed();
            }
        }

        #endregion

        public void ReplaceButtomFragment(SupportFragment FragmentView)
        {
            try
            {
                if (FragmentView != MainFragmentOpened)
                {
                    if (MainFragmentOpened == ChatColorBoxFragment)
                    {
                        ChatColorButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    }
                    else if (MainFragmentOpened == Chat_StickersTab_BoxFragment)
                    {
                        ChatStickerButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    }
                }

               

                if (FragmentView.IsVisible)
                    return;

                var trans = SupportFragmentManager.BeginTransaction();
                trans.Replace(ButtomFragmentHolder.Id, FragmentView);

                if (SupportFragmentManager.BackStackEntryCount == 0)
                {
                    trans.AddToBackStack(null);
                }

                trans.Commit();

                ButtomFragmentHolder.TranslationY = 1200;
                ButtomFragmentHolder.Animate().SetInterpolator(new FastOutSlowInInterpolator()).TranslationYBy(-1200)
                    .SetDuration(500);
                MainFragmentOpened = FragmentView;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ReplaceTopFragment(SupportFragment FragmentView)
        {
            try
            {
               

                if (FragmentView.IsVisible)
                    return;

                var trans = SupportFragmentManager.BeginTransaction();
                trans.Replace(TopFragmentHolder.Id, FragmentView);

                if (SupportFragmentManager.BackStackEntryCount == 0)
                {
                    trans.AddToBackStack(null);
                }

                trans.Commit();

                TopFragmentHolder.TranslationY = 1200;
                TopFragmentHolder.Animate().SetInterpolator(new FastOutSlowInInterpolator()).TranslationYBy(-1200)
                    .SetDuration(500);
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
                if (timer != null)
                {
                    timer.Enabled = false;
                    timer.Stop();
                    timer.Dispose();
                }

                ImageLoader.Instance.ClearMemoryCache();
                ImageLoader.Instance.ClearDiskCache();

                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {
            ChatBoxListView.ScrollToPosition(MAdapter.mmessage.Count - 1);
        }
    }

    public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);

        public event LoadMoreEventHandler LoadMoreEvent;

        public LinearLayoutManager LayoutManager;
        public SwipeRefreshLayout SwipeRefreshLayout;

        public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager, SwipeRefreshLayout swipeRefreshLayout)
        {
            LayoutManager = layoutManager;
            SwipeRefreshLayout = swipeRefreshLayout;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            try
            {
                base.OnScrolled(recyclerView, dx, dy);

                var visibleItemCount = recyclerView.ChildCount;
                var totalItemCount = recyclerView.GetAdapter().ItemCount;

                var pastVisiblesItems = LayoutManager.FindFirstVisibleItemPosition();
                if (pastVisiblesItems == 0 && (visibleItemCount != totalItemCount))
                {
                    //Load More  from API Request
                    LoadMoreEvent(this, null);
                    //Start Load More messages From Database

                }
                else
                {
                    if (SwipeRefreshLayout.Refreshing)
                    {
                        SwipeRefreshLayout.Refreshing = false;
                        SwipeRefreshLayout.Enabled = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}