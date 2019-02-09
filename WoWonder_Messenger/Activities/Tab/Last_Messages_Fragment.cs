using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Getkeepsafe.Relinker.Elf;
using FFImageLoading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniversalImageLoader.Core;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.DialogUserFragment;
using WoWonder.Adapters;
using WoWonder.Frameworks.Agora;
using WoWonder.Frameworks.Twilio;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using Timer = System.Timers.Timer;
using Fragment = Android.Support.V4.App.Fragment;


namespace WoWonder.Activities.Tab
{
    public class Last_Messages_Fragment : Fragment
    {
        #region Variables Basic

        public static RecyclerView LastMessageRecyler;
        public LinearLayout lastmessages_Empty;
        private TextView Icon_lastmesseges;
        public static LinearLayoutManager mLayoutManager;
        public static LastMessages_Adapter mAdapter;
        public SwipeRefreshLayout swipeRefreshLayout;
        private Timer timer;
        public bool CallActionPopupOpened = false;
        public static string TimerWork = "Working";
        private int CountTimerStory = 0;
        private int CountTimerRefreshDataBase = 0;
        #endregion

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Set our view from the "Last_Messages_Fragment" layout resource
                View view = inflater.Inflate(Resource.Layout.Last_Messages_Fragment, container, false);

                //Get values
                LastMessageRecyler = (RecyclerView)view.FindViewById(Resource.Id.lastmessagesrecyler);
                lastmessages_Empty = (LinearLayout)view.FindViewById(Resource.Id.lastmessages_LinerEmpty);
                Icon_lastmesseges = (AppCompatTextView)view.FindViewById(Resource.Id.lastmesseges_icon);

                swipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.lastmessages_swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);

                IMethods.Set_TextViewIcon("1", Icon_lastmesseges, IonIcons_Fonts.ChatbubbleWorking);
                Icon_lastmesseges.SetTextColor(Android.Graphics.Color.ParseColor(AppSettings.MainColor));

                LastMessageRecyler.Visibility = ViewStates.Visible;
                lastmessages_Empty.Visibility = ViewStates.Gone;

                mLayoutManager = new LinearLayoutManager(this.Context);
                LastMessageRecyler.SetLayoutManager(mLayoutManager);
                mAdapter = new LastMessages_Adapter(this.Activity);
                mAdapter.MLastMessagesUser = new ObservableCollection<Classes.Get_Users_List_Object.User>(Classes.UserList);
                if (mAdapter.MLastMessagesUser.Count > 0)
                    mAdapter.BindEnd();

                mAdapter.ItemClick += MAdapterOnItemClick;
                mAdapter.ItemLongClick += MAdapterOnItemLongClick;
                LastMessageRecyler.SetAdapter(mAdapter);

                swipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;

                Get_LastChat();

                API_Request.Get_MyProfileData_Api(this.Activity).ConfigureAwait(false);

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                if (mAdapter.MLastMessagesUser.Count > 0)
                {
                    LastMessageRecyler.Visibility = ViewStates.Visible;
                    lastmessages_Empty.Visibility = ViewStates.Gone;
                }

                if (timer != null)
                {
                    timer.Enabled = true;
                    timer.Start();
                }

                mAdapter.NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
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

        public async void Get_LastChat()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();

                var chatList = dbDatabase.Get_LastUsersChat_List();
                if (chatList?.Count > 0)
                {
                    mAdapter.MLastMessagesUser = new ObservableCollection<Classes.Get_Users_List_Object.User>(chatList);
                    Classes.UserList = mAdapter.MLastMessagesUser;
                    if (mAdapter?.MLastMessagesUser?.Count > 0)
                    {
                        this.Activity.RunOnUiThread(() =>
                        {
                            mAdapter.NotifyDataSetChanged();

                            lastmessages_Empty.Visibility = ViewStates.Gone;
                            LastMessageRecyler.Visibility = ViewStates.Visible;

                            swipeRefreshLayout.Refreshing = false;
                        });

                    }
                    else
                    {
                        swipeRefreshLayout.Refreshing = true;
                    }
                }
                else
                {
                    swipeRefreshLayout.Refreshing = true;
                }

                await LastChat_Users_Api();

                TimerWork = "Working";

                // Run timer 
                timer = new Timer();
                timer.Interval = AppSettings.RefreshChatActivitiesSecounds;
                timer.Elapsed += TimerOnElapsed;
                timer.Enabled = true;
                timer.Start();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                swipeRefreshLayout.Refreshing = false;
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (TimerWork != "Working" || !IMethods.CheckConnectivity()) return;

                LastChat_Users_Api(true).ConfigureAwait(false);

                //After this time set update API to get the stories. >> (10 * 6) == 60 Seconds
                if (CountTimerStory == AppSettings.UpdateStory)
                {
                    Task.Run(() =>
                    {
                        Tabbed_Main_Page.Last_Stroies_Fragment_page.GetStory_Api();
                    });
                    CountTimerStory = 0;
                }
                CountTimerStory++;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // Get Users List LastMessage 
        public async Task LastChat_Users_Api(bool runTimer = false)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                  
                }
                else
                {
                    TimerWork = "Stop";

                    string data = await API_Request.Get_users_list_Async();
                    if (!string.IsNullOrEmpty(data))
                    {
                        #region Last Messeges >> users

                        try
                        {
                            var users = JObject.Parse(data).SelectToken("users").ToString();
                            if (!string.IsNullOrEmpty(users))
                            {
                                JArray dataChatUsers = JArray.Parse(users);
                                if (dataChatUsers.Count > 0)
                                {
                                    foreach (var chatUser in dataChatUsers)
                                    {
                                        if (chatUser != null)
                                        {
                                            var user_id = "";
                                            var M_id = "";

                                            JObject Userdata = null;
                                            JObject userLastMessage = null;
                                            try
                                            {
                                                Userdata = JObject.FromObject(chatUser);
                                                userLastMessage = JObject.FromObject(Userdata["last_message"]);

                                                user_id = Userdata["user_id"].ToString();
                                                M_id = userLastMessage["id"].ToString();
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                            }

                                            if (Userdata != null && userLastMessage != null)
                                            {
                                                if (!string.IsNullOrEmpty(user_id) && !string.IsNullOrEmpty(M_id))
                                                {
                                                    var username = Userdata["username"].ToString();
                                                    var name = Userdata["name"].ToString();
                                                    var profile_picture = Userdata["profile_picture"].ToString();
                                                    var cover_picture = Userdata["cover_picture"].ToString();
                                                    var verified = Userdata["verified"].ToString();
                                                    var lastseen = Userdata["lastseen"].ToString();
                                                    var lastseen_unix_time = Userdata["lastseen_unix_time"].ToString();
                                                    var lastseen_time_text = Userdata["lastseen_time_text"].ToString();
                                                    var url = Userdata["url"].ToString();
                                                    var chat_color = AppSettings.MainColor;
                                                    try
                                                    {
                                                        chat_color = Userdata["chat_color"].ToString();
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.WriteLine(e);
                                                    }

                                                    //last_message
                                                    if (!string.IsNullOrEmpty(M_id))
                                                    {
                                                        var M_from_id = userLastMessage["from_id"].ToString();
                                                        var M_group_id = userLastMessage["group_id"].ToString();
                                                        var M_to_id = userLastMessage["to_id"].ToString();
                                                        var M_text = userLastMessage["text"].ToString();
                                                        var M_media = userLastMessage["media"].ToString();
                                                        var M_mediaFileName = userLastMessage["mediaFileName"].ToString();
                                                        var M_mediaFileNames = userLastMessage["mediaFileNames"].ToString();
                                                        var M_time = userLastMessage["time"].ToString();
                                                        var M_seen = userLastMessage["seen"].ToString();
                                                        var M_deleted_one = userLastMessage["deleted_one"].ToString();
                                                        var M_deleted_two = userLastMessage["deleted_two"].ToString();
                                                        var M_sent_push = userLastMessage["sent_push"].ToString();
                                                        var M_notification_id = userLastMessage["notification_id"].ToString();
                                                        var M_type_two = userLastMessage["type_two"].ToString();
                                                        var M_stickers = IMethods.Fun_String.StringNullRemover(userLastMessage["stickers"].ToString());
                                                        var M_date_time = userLastMessage["date_time"].ToString();

                                                        var checkUser = mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == user_id);
                                                        if (checkUser != null)
                                                        {
                                                            if (checkUser.Name != name ||
                                                                checkUser.ProfilePicture != profile_picture ||
                                                                checkUser.Lastseen != lastseen ||
                                                                checkUser.LastMessage.Text != M_text ||
                                                                checkUser.LastMessage.Media != M_media ||
                                                                checkUser.LastMessage.Seen != M_seen ||
                                                                checkUser.LastseenTimeText != lastseen_time_text)
                                                            {
                                                                checkUser.UserId = user_id;
                                                                checkUser.Username = username;
                                                              
                                                                checkUser.CoverPicture = cover_picture;
                                                            
                                                               
                                                                checkUser.Url = url;
                                                               
                                                                checkUser.LastseenUnixTime = lastseen_unix_time;
                                                                checkUser.ChatColor = chat_color;
                                                                checkUser.Verified = verified;

                                                                //last_message
                                                                if (checkUser.LastMessage == null)
                                                                    checkUser.LastMessage = new Classes.Get_Users_List_Object.LastMessage();

                                                                checkUser.LastMessage.Id = M_id;
                                                                checkUser.LastMessage.FromId = M_from_id;
                                                                checkUser.LastMessage.GroupId = M_group_id;
                                                                checkUser.LastMessage.ToId = M_to_id;
                                                              
                                                                checkUser.LastMessage.MediaFileName = M_mediaFileName;
                                                                checkUser.LastMessage.MediaFileNames = M_mediaFileNames;
                                                                checkUser.LastMessage.Time = M_time;
                                                                checkUser.LastMessage.Seen = M_seen;
                                                                checkUser.LastMessage.DeletedOne = M_deleted_one;
                                                                checkUser.LastMessage.DeletedTwo = M_deleted_two;
                                                                checkUser.LastMessage.SentPush = M_sent_push;
                                                                checkUser.LastMessage.NotificationId = M_notification_id;
                                                                checkUser.LastMessage.TypeTwo = M_type_two;
                                                                checkUser.LastMessage.Stickers = M_stickers;
                                                                checkUser.LastMessage.DateTime = M_date_time;

                                                                if (checkUser.ProfilePicture != profile_picture)
                                                                {
                                                                    checkUser.ProfilePicture = profile_picture;
                                                                }

                                                                if (checkUser.Lastseen != lastseen)
                                                                {
                                                                    checkUser.Lastseen = lastseen;
                                                                }

                                                                if (checkUser.LastseenTimeText != lastseen_time_text)
                                                                {
                                                                    checkUser.LastseenTimeText = lastseen_time_text;
                                                                }

                                                                if (checkUser.Name != name)
                                                                {
                                                                    checkUser.Name = name;
                                                                }

                                                                if (checkUser.LastMessage.Seen != M_seen)
                                                                {
                                                                    checkUser.LastMessage.Seen = M_seen;
                                                                }

                                                                if (checkUser.LastMessage.Text != M_text)
                                                                {
                                                                    checkUser.LastMessage.Text = M_text;

                                                                    var index = mAdapter.MLastMessagesUser.IndexOf(mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == user_id));
                                                                    if (index > -1)
                                                                    {
                                                                        mAdapter.MLastMessagesUser.Move(index, 0);
                                                                        Activity.RunOnUiThread(() =>
                                                                        {
                                                                            mAdapter.Move(checkUser);
                                                                        });
                                                                    }
                                                                }

                                                                if (checkUser.LastMessage.Media != M_media)
                                                                {
                                                                    checkUser.LastMessage.Media = M_media;
                                                                    var index = mAdapter.MLastMessagesUser.IndexOf(mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == user_id));
                                                                    if (index > -1)
                                                                    {
                                                                        mAdapter.MLastMessagesUser.Move(index, 0);
                                                                        Activity.RunOnUiThread(() =>
                                                                        {
                                                                            mAdapter.Move(checkUser);
                                                                        });
                                                                    }
                                                                }

                                                              
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                                            {
                                                                UserId = user_id,
                                                                Username = username,
                                                                ProfilePicture = profile_picture,
                                                                CoverPicture = cover_picture,
                                                                LastseenTimeText = lastseen_time_text,
                                                                Lastseen = lastseen,
                                                                Url = url,
                                                                Name = name,
                                                                LastseenUnixTime = lastseen_unix_time,
                                                                ChatColor = chat_color,
                                                                Verified = verified,
                                                                LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                                                {
                                                                    Id = M_id,
                                                                    FromId = M_from_id,
                                                                    GroupId = M_group_id,
                                                                    ToId = M_to_id,
                                                                    Text = M_text,
                                                                    Media = M_media,
                                                                    MediaFileName = M_mediaFileName,
                                                                    MediaFileNames = M_mediaFileNames,
                                                                    Time = M_time,
                                                                    Seen = M_seen,
                                                                    DeletedOne = M_deleted_one,
                                                                    DeletedTwo = M_deleted_two,
                                                                    SentPush = M_sent_push,
                                                                    NotificationId = M_notification_id,
                                                                    TypeTwo = M_type_two,
                                                                    Stickers = M_stickers,
                                                                    DateTime = M_date_time,
                                                                },
                                                            };

                                                            if (runTimer)
                                                            {
                                                                this.Activity.RunOnUiThread(() =>
                                                                {
                                                                    mAdapter.Insert(user);

                                                                    var dataUser = mAdapter.MLastMessagesUser.IndexOf(mAdapter.MLastMessagesUser.FirstOrDefault(a => a.LastMessage.Id == M_id));
                                                                    if (dataUser > -1)
                                                                        mAdapter.NotifyItemChanged(dataUser);
                                                                });
                                                            }
                                                            else
                                                            {
                                                                this.Activity.RunOnUiThread(() =>
                                                                {
                                                                    mAdapter.Add(user);
                                                                    var dataUser = mAdapter.MLastMessagesUser.IndexOf(mAdapter.MLastMessagesUser.FirstOrDefault(a => a.LastMessage.Id == M_id));
                                                                    if (dataUser > -1)
                                                                        mAdapter.NotifyItemChanged(dataUser);
                                                                });
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                     
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        #endregion

                        #region Call >> Video_Call_User

                        try
                        {
                            if (AppSettings.Enable_Audio_Video_Call)
                            {
                                var dataCall = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                                if (AppSettings.Use_Twilio_Library)
                                {
                                    bool Twilio_video_call = false;
                                    bool Twilio_audio_call = false;

                                    if (AppSettings.Enable_Video_Call)
                                    {
                                        Twilio_video_call = Boolean.Parse(dataCall["video_call"].ToString());

                                        #region Twilio Video call

                                        if (Twilio_video_call && CallActionPopupOpened == false)
                                        {
                                            var CallUser = JObject.Parse(data).SelectToken("video_call_user");
                                            if (CallUser != null)
                                            {
                                                CallActionPopupOpened = true; //Disable any new Popup Screens

                                                JObject User = JObject.FromObject(CallUser);

                                                var user_id = CallUser["user_id"].ToString();
                                                var avatar = CallUser["avatar"].ToString();
                                                var name = CallUser["name"].ToString();

                                                var Videos_data = JObject.Parse(CallUser.ToString()).SelectToken("data");
                                                if (Videos_data != null)
                                                {
                                                    JObject Videos = JObject.FromObject(Videos_data);
                                                    var id = Videos["id"].ToString(); //call_id
                                                    var access_token = Videos["access_token"].ToString();
                                                    var access_token_2 = Videos["access_token_2"].ToString();
                                                    var from_id = Videos["from_id"].ToString();
                                                    var active = Videos["active"].ToString();
                                                    var time = Videos["called"].ToString();
                                                    var declined = Videos["declined"].ToString();
                                                    var RoomName = Videos["room_name"].ToString();

                                                    Intent intent = new Intent(this.Context, typeof(TwilioVideoCallActivity));
                                                    intent.PutExtra("UserID", user_id);
                                                    intent.PutExtra("avatar", avatar);
                                                    intent.PutExtra("name", name);
                                                    intent.PutExtra("access_token", access_token);
                                                    intent.PutExtra("access_token_2", access_token_2);
                                                    intent.PutExtra("from_id", from_id);
                                                    intent.PutExtra("active", active);
                                                    intent.PutExtra("time", time);
                                                    intent.PutExtra("CallID", id);
                                                    intent.PutExtra("status", declined);
                                                    intent.PutExtra("room_name", RoomName);
                                                    intent.PutExtra("declined", declined);
                                                    intent.PutExtra("type", "Twilio_video_call");

                                                    string AvatarSplit = avatar.Split('/').Last();
                                                    var GetImg = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);
                                                    if (avatar != null)
                                                    {
                                                        if (GetImg == "File Dont Exists")
                                                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, avatar);
                                                    }

                                                    NotificationManagerClass.StartinCommingCall(intent, GetImg, this.GetText(Resource.String.Lbl_Video_call), name + " " + this.GetText(Resource.String.Lbl_is_calling_you), Int32.Parse(id));
                                                }
                                            }
                                        }
                                        else if (Twilio_video_call == false && Twilio_audio_call == false)
                                        {
                                            if (CallActionPopupOpened)
                                            {
                                                CallActionPopupOpened = false;
                                                var NotificationBroadcasterAction = new Intent(Application.Context, typeof(NotificationManagerClass.NotificationBroadcasterCloser));
                                                NotificationBroadcasterAction.PutExtra("type", "Twilio_video_call");
                                                NotificationBroadcasterAction.PutExtra("action", "dismiss");
                                                Application.Context.SendBroadcast(NotificationBroadcasterAction);
                                            }
                                        }

                                        #endregion

                                    }

                                    if (AppSettings.Enable_Audio_Call)
                                    {
                                        Twilio_audio_call = Boolean.Parse(dataCall["audio_call"].ToString());

                                        #region Twilio Audio call

                                        if (Twilio_audio_call && CallActionPopupOpened == false)
                                        {
                                            var CallUser = JObject.Parse(data).SelectToken("audio_call_user");
                                            if (CallUser != null)
                                            {
                                                CallActionPopupOpened = true; //Disable any new Popup Screens

                                                JObject User = JObject.FromObject(CallUser);

                                                var user_id = CallUser["user_id"].ToString();
                                                var avatar = User["avatar"].ToString();
                                                var name = User["name"].ToString();

                                                var Videos_data = JObject.Parse(CallUser.ToString()).SelectToken("data");
                                                if (Videos_data != null)
                                                {
                                                    JObject Videos = JObject.FromObject(Videos_data);
                                                    var id = Videos["id"].ToString(); //call_id
                                                    var access_token = Videos["access_token"].ToString();
                                                    var access_token_2 = Videos["access_token_2"].ToString();
                                                    var from_id = Videos["from_id"].ToString();
                                                    var active = Videos["active"].ToString();
                                                    var time = Videos["called"].ToString();
                                                    var declined = Videos["declined"].ToString();
                                                    var RoomName = Videos["room_name"].ToString();

                                                    Intent intent = new Intent(this.Context, typeof(TwilioAudioCallActivity));
                                                    intent.PutExtra("UserID", user_id);
                                                    intent.PutExtra("avatar", avatar);
                                                    intent.PutExtra("name", name);
                                                    intent.PutExtra("access_token", access_token);
                                                    intent.PutExtra("access_token_2", access_token_2);
                                                    intent.PutExtra("from_id", from_id);
                                                    intent.PutExtra("active", active);
                                                    intent.PutExtra("time", time);
                                                    intent.PutExtra("CallID", id);
                                                    intent.PutExtra("status", declined);
                                                    intent.PutExtra("room_name", RoomName);
                                                    intent.PutExtra("declined", declined);
                                                    intent.PutExtra("type", "Twilio_audio_call");

                                                    string AvatarSplit = avatar.Split('/').Last();
                                                    var GetImg = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);

                                                    if (avatar != null)
                                                    {
                                                        if (GetImg == "File Dont Exists")
                                                            IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, avatar);
                                                    }

                                                    NotificationManagerClass.StartinCommingCall(intent, GetImg, this.GetText(Resource.String.Lbl_Voice_call), name + " " + this.GetText(Resource.String.Lbl_is_calling_you), Int32.Parse(id));
                                                }
                                            }
                                        }
                                        else if (Twilio_audio_call == false && Twilio_video_call == false)
                                        {
                                            if (CallActionPopupOpened)
                                            {
                                                CallActionPopupOpened = false;
                                                var NotificationBroadcasterAction = new Intent(Application.Context, typeof(NotificationManagerClass.NotificationBroadcasterCloser));
                                                NotificationBroadcasterAction.PutExtra("type", "Twilio_audio_call");
                                                NotificationBroadcasterAction.PutExtra("action", "dismiss");
                                                Application.Context.SendBroadcast(NotificationBroadcasterAction);

                                            }
                                        }
                                        #endregion

                                    }
                                }
                                else if (AppSettings.Use_Agora_Library)
                                {
                                    #region Agora Audio/Video call

                                    var Agora_call = Boolean.Parse(dataCall["agora_call"].ToString());
                                    if (Agora_call && CallActionPopupOpened == false)
                                    {
                                        var CallUser = JObject.Parse(data).SelectToken("agora_call_data");

                                        if (CallUser != null)
                                        {
                                            CallActionPopupOpened = true; //Disable any new Popup Screens

                                            JObject User = JObject.FromObject(CallUser);

                                            var user_id = CallUser["user_id"].ToString();
                                            var avatar = User["avatar"].ToString();
                                            var name = User["name"].ToString();
                                            var Videos_data = JObject.Parse(CallUser.ToString()).SelectToken("data");
                                            if (Videos_data != null)
                                            {
                                                JObject Videos = JObject.FromObject(Videos_data);
                                                var id = Videos["id"].ToString(); //call_id
                                                var from_id = Videos["from_id"].ToString();
                                                var type = Videos["type"].ToString();
                                                var time = Videos["time"].ToString();
                                                var Status = Videos["status"].ToString();
                                                var RoomName = Videos["room_name"].ToString();

                                                string AvatarSplit = avatar.Split('/').Last();
                                                var GetImg = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskImage, AvatarSplit);

                                                if (avatar != null)
                                                {
                                                    if (GetImg == "File Dont Exists")
                                                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskImage, avatar);
                                                }

                                                if (type == "video")
                                                {
                                                    if (AppSettings.Enable_Video_Call)
                                                    {
                                                        Intent intent = new Intent(this.Context, typeof(AgoraVideoCallActivity));
                                                        intent.PutExtra("UserID", user_id);
                                                        intent.PutExtra("avatar", avatar);
                                                        intent.PutExtra("name", name);
                                                        intent.PutExtra("from_id", from_id);
                                                        intent.PutExtra("status", Status);
                                                        intent.PutExtra("time", time);
                                                        intent.PutExtra("CallID", id);
                                                        intent.PutExtra("room_name", RoomName);
                                                        intent.PutExtra("type", "Agora_video_call_recieve");
                                                        intent.PutExtra("declined", "0");

                                                        NotificationManagerClass.StartinCommingCall(intent, GetImg, this.GetText(Resource.String.Lbl_Video_call), name + " " + this.GetText(Resource.String.Lbl_is_calling_you), Int32.Parse(id));
                                                    }
                                                }
                                                else if (type == "audio")
                                                {
                                                    if (AppSettings.Enable_Audio_Call)
                                                    {
                                                        Intent intent = new Intent(this.Context, typeof(AgoraAudioCallActivity));
                                                        intent.PutExtra("UserID", user_id);
                                                        intent.PutExtra("avatar", avatar);
                                                        intent.PutExtra("name", name);
                                                        intent.PutExtra("from_id", from_id);
                                                        intent.PutExtra("status", Status);
                                                        intent.PutExtra("time", time);
                                                        intent.PutExtra("CallID", id);
                                                        intent.PutExtra("room_name", RoomName);
                                                        intent.PutExtra("type", "Agora_audio_call_recieve");
                                                        intent.PutExtra("declined", "0");

                                                        NotificationManagerClass.StartinCommingCall(intent, GetImg, this.GetText(Resource.String.Lbl_Voice_call), name + " " + this.GetText(Resource.String.Lbl_is_calling_you), Int32.Parse(id));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (Agora_call == false)
                                    {
                                        if (CallActionPopupOpened)
                                        {
                                            CallActionPopupOpened = false;
                                            var notificationBroadcasterAction = new Intent(Application.Context, typeof(NotificationManagerClass.NotificationBroadcasterCloser));
                                            notificationBroadcasterAction.PutExtra("type", "Agora_video_call_recieve");
                                            notificationBroadcasterAction.PutExtra("action", "dismiss");
                                            this.Context.SendBroadcast(notificationBroadcasterAction);
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        #endregion
                    }

                    if (swipeRefreshLayout.Refreshing)
                        swipeRefreshLayout.Refreshing = false;

                    this.Activity.RunOnUiThread(() =>
                    {
                        try
                        {
                            if (mAdapter.MLastMessagesUser.Count > 0)
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                Classes.UserList = mAdapter.MLastMessagesUser;
                                mAdapter.BindEnd();

                                LastMessageRecyler.Visibility = ViewStates.Visible;
                                lastmessages_Empty.Visibility = ViewStates.Gone;

                                if (CountTimerRefreshDataBase == 25)
                                {
                                    CountTimerRefreshDataBase = 0;
                                    dbDatabase.ClearAll_LastUsersChat();
                                }
                                CountTimerRefreshDataBase++;

                                //Insert All data users to database
                                dbDatabase.Insert_Or_Update_LastUsersChat(mAdapter.MLastMessagesUser);
                                dbDatabase.Dispose();

                            }
                            else
                            {
                                LastMessageRecyler.Visibility = ViewStates.Gone;
                                lastmessages_Empty.Visibility = ViewStates.Visible;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    });

                    TimerWork = "Working";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await LastChat_Users_Api().ConfigureAwait(false);
            }
        }

        public string UserId = "";
        private void MAdapterOnItemClick(object sender, LastMessages_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = mAdapter.GetItem(position);
                    if (item != null)
                    {
                        UserId = item.UserId;

                        // Check if we're running on Android 5.0 or higher
                        if ((int)Build.VERSION.SdkInt < 23)
                        {
                            item.LastMessage.Seen = "1";
                            Intent intent = new Intent(this.Context, typeof(ChatWindow_Activity));
                            intent.PutExtra("UserID", item.UserId);
                            intent.PutExtra("TypeChat", "LastMessenger");
                            intent.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                            StartActivity(intent);
                        }
                        else
                        {
                            //Check to see if any permission in our group is available, if one, then all are
                            if (this.Context.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted && this.Context.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                            {
                                item.LastMessage.Seen = "1";
                                Intent intent = new Intent(this.Context, typeof(ChatWindow_Activity));
                                intent.PutExtra("UserID", item.UserId);
                                intent.PutExtra("TypeChat", "LastMessenger");
                                intent.PutExtra("UserItem", JsonConvert.SerializeObject(item));
                                StartActivity(intent);
                            }
                            else
                            {
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.ReadExternalStorage,
                                    Manifest.Permission.WriteExternalStorage,
                                }, 101);
                            }
                        }
                    }
                }
            }
            catch (Java.Lang.Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MAdapterOnItemLongClick(object sender, LastMessages_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = mAdapter.GetItem(Position);
                    if (item != null)
                    {
                        TimerWork = "Stop";

                        //Pull up dialog
                        Android.Support.V4.App.FragmentTransaction transaction = FragmentManager.BeginTransaction();
                        Dialog_DeleteMessage userDialog = new Dialog_DeleteMessage(item.UserId, item);
                        userDialog.Show(transaction, "dialog fragment");
                        userDialog._OnDeleteMessageUpComplete += UserDialogOnOnDeleteMessageUpComplete;
                    }
                }
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
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(this.Context, this.Context.GetText(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                    if (swipeRefreshLayout.Refreshing)
                        swipeRefreshLayout.Refreshing = false;

                }
                else
                {
                    this.Activity.RunOnUiThread(async () =>
                    {
                        mAdapter.Clear();

                        timer.Enabled = false;
                        timer.Stop();

                        ImageLoader.Instance.ClearMemoryCache();
                        ImageLoader.Instance.ClearDiskCache();

                        await LastChat_Users_Api();

                        if (mAdapter.MLastMessagesUser.Count == 0)
                        {
                            Get_LastChat();
                            return;
                        }

                        TimerWork = "Working";

                        // Run timer 
                        timer = new Timer();
                        timer.Interval = AppSettings.RefreshChatActivitiesSecounds;
                        timer.Elapsed += TimerOnElapsed;
                        timer.Enabled = true;
                        timer.Start();
                    });
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void UserDialogOnOnDeleteMessageUpComplete(object sender, Dialog_DeleteMessage.OnDeleteMessageUp_EventArgs args)
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        var data = mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == this.UserId);
                        if (data != null)
                        {
                            data.LastMessage.Seen = "1";
                            Intent intent = new Intent(this.Context, typeof(ChatWindow_Activity));
                            intent.PutExtra("UserID", data.UserId);
                            intent.PutExtra("TypeChat", "LastMessenger");
                            intent.PutExtra("UserItem", JsonConvert.SerializeObject(data));
                            StartActivity(intent);
                        }
                    }
                    else
                    {
                        Toast.MakeText(this.Context, this.Context.GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        public override void OnDestroy()
        {
            try
            {
                if (timer != null)
                {
                    timer.Enabled = false;
                    timer.Stop();
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
    }
}