using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using DT.Xamarin.Agora;
using DT.Xamarin.Agora.Video;
using FFImageLoading.Views;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;

namespace WoWonder.Frameworks.Agora {
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation, ResizeableActivity = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AgoraVideoCallActivity : AppCompatActivity
    {
        #region Variables Basic

        public static string ROOM_NAME = "TestRoom";
        public static string CALL_ID = "0";
        public static string CALL_TYPE = "0";
        public static string UserID = "";
        public static string avatar = "0";
        public static string name = "0";
        public static string from_id = "0";
        public static string active = "0";
        public static string time = "0";
        public static string status = "0";

        protected const int MaxLocalVideoDimension = 150;
        protected RtcEngine AgoraEngine;
        protected AgoraRtcHandler AgoraHandler;
        private bool _isVideoEnabled = true;
        private SurfaceView _localVideoView;

        //Controls
        private Button Switch_cam_button;

        private CircleButton End_call_button;
        private CircleButton Mute_video_button;
        private CircleButton Mute_audio_button;
        private RelativeLayout UserInfoview_container;
        private ImageView UserImageViewAsync;
        private TextView UserNameTextView;
        private TextView NoteTextView;

        private int CountSecoundsOfOutgoingCall = 0;
        private Timer TimerRequestWaiter = new Timer();

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.AgoraVideoCallActivityLayout);

                Intent intentCloser = new Intent(this, typeof(NotificationManagerClass.NotificationBroadcasterCloser));
                intentCloser.PutExtra("action", "answer");
                intentCloser.PutExtra("type", Intent.GetStringExtra("type"));
                SendBroadcast(intentCloser);

                UserID = Intent.GetStringExtra("UserID");
                avatar = Intent.GetStringExtra("avatar");
                name = Intent.GetStringExtra("name");

                var data_CALL_ID = Intent.GetStringExtra("CallID") ?? "Data not available";
                if (data_CALL_ID != "Data not available" && !String.IsNullOrEmpty(data_CALL_ID))
                {
                    CALL_ID = data_CALL_ID;

                    from_id = Intent.GetStringExtra("from_id");
                    active = Intent.GetStringExtra("active");
                    time = Intent.GetStringExtra("time");
                    status = Intent.GetStringExtra("status");
                    ROOM_NAME = Intent.GetStringExtra("room_name");
                    CALL_TYPE = Intent.GetStringExtra("type");
                }

                Switch_cam_button = FindViewById<Button>(Resource.Id.switch_cam_button);
                Mute_video_button = FindViewById<CircleButton>(Resource.Id.mute_video_button);
                End_call_button = FindViewById<CircleButton>(Resource.Id.end_call_button);
                Mute_audio_button = FindViewById<CircleButton>(Resource.Id.mute_audio_button);

                UserInfoview_container = FindViewById<RelativeLayout>(Resource.Id.userInfoview_container);
                UserImageViewAsync = FindViewById<ImageView>(Resource.Id.userImageViewAsync);
                UserNameTextView = FindViewById<TextView>(Resource.Id.userNameTextView);
                NoteTextView = FindViewById<TextView>(Resource.Id.noteTextView);

                Switch_cam_button.Click += Switch_cam_button_Click;
                Mute_video_button.Click += Mute_video_button_Click;
                End_call_button.Click += End_call_button_Click;
                Mute_audio_button.Click += Mute_audio_button_Click;

                LoadUserInfo(UserID);

                if (CALL_TYPE == "Agora_video_calling_start")
                {
                    Start_Call_Action("call");
                }
                else
                {
                    Start_Call_Action("recieve_call");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void Start_Call_Action(string Type)
        {
            try
            {
                if (Type == "call")
                {
                    NoteTextView.Text = this.GetText(Resource.String.Lbl_Calling);
                    var ApiStartCall = await API_Request.Create_Agora_Call_Event_Async(UserID, "video");
                    if (ApiStartCall != null)
                    {
                        ROOM_NAME = ApiStartCall.room_name;
                        CALL_ID = ApiStartCall.id;
                        IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3");

                        TimerRequestWaiter = new Timer();
                        TimerRequestWaiter.Interval = 5000;
                        TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                        TimerRequestWaiter.Start();
                    }
                }
                else
                {
                    ROOM_NAME = Intent.GetStringExtra("room_name");
                    CALL_ID = Intent.GetStringExtra("CallID");
                    name = Intent.GetStringExtra("name");
                    avatar = Intent.GetStringExtra("avatar");

                    NoteTextView.Text = this.GetText(Resource.String.Lbl_Waiting_to_connect);

                    var ApiStartCall = await API_Request.Send_Agora_Call_Action_Async("answer", CALL_ID);
                    if (ApiStartCall == "200")
                    {
                        var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a => a.id == CALL_ID); // id >> Call_Id
                        if (ckd == null)
                        {
                            Classes.Call_User cv = new Classes.Call_User();
                            cv.id = CALL_ID;
                            cv.user_id = UserID;
                            cv.avatar = avatar;
                            cv.name = name;
                            cv.from_id = from_id;
                            cv.active = active;
                            cv.time = "Answered call";
                            cv.status = status;
                            cv.room_name = ROOM_NAME;
                            cv.type = CALL_TYPE;
                            cv.typeIcon = "Accept";
                            cv.typeColor = "#008000";

                            Last_Calls_Fragment.mAdapter?.Insert(cv);

                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.Insert_CallUser(cv);
                            dbDatabase.Dispose();
                        }
                        InitAgoraEngineAndJoinChannel(ROOM_NAME); //the caller Is Joining agora Server
                    }
                    else
                    {
                        var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a => a.id == CALL_ID); // id >> Call_Id
                        if (ckd == null)
                        {
                            Classes.Call_User cv = new Classes.Call_User();
                            cv.id = CALL_ID;
                            cv.user_id = UserID;
                            cv.avatar = avatar;
                            cv.name = name;
                            cv.from_id = from_id;
                            cv.active = active;
                            cv.time = "Missed call";
                            cv.status = status;
                            cv.room_name = ROOM_NAME;
                            cv.type = CALL_TYPE;
                            cv.typeIcon = "Cancel";
                            cv.typeColor = "#FF0000";

                            Last_Calls_Fragment.mAdapter?.Insert(cv);

                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.Insert_CallUser(cv);
                            dbDatabase.Dispose();
                        }
                        NoteTextView.Text = this.GetText(Resource.String.Lbl_Faild_to_connect);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadUserInfo(string userid)
        {
            try
            {
                UserNameTextView.Text = name;

                //profile_picture
                ImageCacheLoader.LoadImage(avatar, UserImageViewAsync, false, true);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void OnCallTime_Running_Out()
        {
            try
            {
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                NoteTextView.Text = this.GetText(Resource.String.Lbl_No_respond_from_the_user);
                await Task.Delay(3000);
                if (AgoraEngine != null)
                {
                    try
                    {
                        AgoraEngine.StopPreview();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                    try
                    {
                        AgoraEngine.SetupLocalVideo(null);
                        AgoraEngine.LeaveChannel();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    AgoraEngine.Dispose();
                    AgoraEngine = null;

                }
                Finish();
            }
            catch (Exception exception)
            {
                Finish();
                Console.WriteLine(exception);
            }
        }

        public async void OnCall_Declined_From_User()
        {
            try
            {
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");

                NoteTextView.Text = this.GetText(Resource.String.Lbl_The_user_declinde_your_call);
                await Task.Delay(3000);
                if (AgoraEngine != null)
                {
                    try
                    {
                        AgoraEngine.StopPreview();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                    try
                    {
                        AgoraEngine.SetupLocalVideo(null);
                        AgoraEngine.LeaveChannel();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    AgoraEngine.Dispose();
                    AgoraEngine = null;

                }
                Finish();
            }
            catch (Exception exception)
            {
                Finish();
                Console.WriteLine(exception);
            }
        }

        private async void TimerCallRequestAnswer_Waiter_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var Call_Result_Generation = await API_Request.Check_Agora_Call_Answer_Async(CALL_ID, "video");

                if (string.IsNullOrEmpty(Call_Result_Generation))
                    return;

                TimerRequestWaiter.Enabled = false;
                TimerRequestWaiter.Stop();
                TimerRequestWaiter.Close();

                RunOnUiThread(() => {
                    IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    InitAgoraEngineAndJoinChannel(ROOM_NAME);
                });

                if (Call_Result_Generation == "answered")
                {
                    var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a => a.id == CALL_ID); // id >> Call_Id
                    if (ckd == null)
                    {
                        Classes.Call_User cv = new Classes.Call_User
                        {
                            id = CALL_ID,
                            user_id = UserID,
                            avatar = avatar,
                            name = name,
                            from_id = from_id,
                            active = active,
                            time = "Answered call",
                            status = status,
                            room_name = ROOM_NAME,
                            type = CALL_TYPE,
                            typeIcon = "Accept",
                            typeColor = "#008000"
                        };

                        Last_Calls_Fragment.mAdapter?.Insert(cv);

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_CallUser(cv);
                        dbDatabase.Dispose();
                    }
                }
                else if (Call_Result_Generation == "calling")
                {
                    if (CountSecoundsOfOutgoingCall < 80)
                    {
                        CountSecoundsOfOutgoingCall += 10;
                    }
                    else
                    {
                        //Call Is inactive 
                        TimerRequestWaiter.Enabled = false;
                        TimerRequestWaiter.Stop();
                        TimerRequestWaiter.Close();

                        RunOnUiThread(OnCallTime_Running_Out);
                    }
                    var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a => a.id == CALL_ID); // id >> Call_Id
                    if (ckd == null)
                    {
                        Classes.Call_User cv = new Classes.Call_User
                        {
                            id = CALL_ID,
                            user_id = UserID,
                            avatar = avatar,
                            name = name,
                            from_id = from_id,
                            active = active,
                            time = "Answered call",
                            status = status,
                            room_name = ROOM_NAME,
                            type = CALL_TYPE,
                            typeIcon = "Accept",
                            typeColor = "#008000"
                        };

                        Last_Calls_Fragment.mAdapter?.Insert(cv);

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_CallUser(cv);
                        dbDatabase.Dispose();
                    }
                }
                else if (Call_Result_Generation == "declined")
                {
                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();

                    RunOnUiThread(OnCall_Declined_From_User);
                    var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a => a.id == CALL_ID); // id >> Call_Id
                    if (ckd == null)
                    {
                        Classes.Call_User cv = new Classes.Call_User
                        {
                            id = CALL_ID,
                            user_id = UserID,
                            avatar = avatar,
                            name = name,
                            from_id = from_id,
                            active = active,
                            time = "Declined call",
                            status = status,
                            room_name = ROOM_NAME,
                            type = CALL_TYPE,
                            typeIcon = "Declined",
                            typeColor = "#FF8000"
                        };

                        Last_Calls_Fragment.mAdapter?.Insert(cv);

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_CallUser(cv);
                        dbDatabase.Dispose();
                    }
                }
                else if (Call_Result_Generation == "no_answer")
                {
                    //Call Is inactive 
                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();

                    RunOnUiThread(OnCallTime_Running_Out);

                    var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a => a.id == CALL_ID); // id >> Call_Id
                    if (ckd == null)
                    {
                        Classes.Call_User cv = new Classes.Call_User();
                        cv.id = CALL_ID;
                        cv.user_id = UserID;
                        cv.avatar = avatar;
                        cv.name = name;
                        cv.from_id = from_id;
                        cv.active = active;
                        cv.time = "Missed call";
                        cv.status = status;
                        cv.room_name = ROOM_NAME;
                        cv.type = CALL_TYPE;
                        cv.typeIcon = "Cancel";
                        cv.typeColor = "#FF0000";

                        Last_Calls_Fragment.mAdapter?.Insert(cv);

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_CallUser(cv);
                        dbDatabase.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Mute_audio_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (Mute_audio_button.Selected)
                {
                    Mute_audio_button.Selected = false;
                    Mute_audio_button.SetImageResource(Resource.Drawable.ic_camera_mic_open);
                }
                else
                {
                    Mute_audio_button.Selected = true;
                    Mute_audio_button.SetImageResource(Resource.Drawable.ic_camera_mic_mute);
                }
                AgoraEngine.MuteLocalAudioStream(Mute_audio_button.Selected);
                var visibleMutedLayers = Mute_audio_button.Selected ? ViewStates.Visible : ViewStates.Invisible;
                FindViewById(Resource.Id.local_video_overlay).Visibility = visibleMutedLayers;
                FindViewById(Resource.Id.local_video_muted).Visibility = visibleMutedLayers;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }

        private void End_call_button_Click(object sender, EventArgs e)
        {
            try
            {
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                if (AgoraEngine != null)
                {
                    try
                    {
                        AgoraEngine.StopPreview();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                    try
                    {
                        AgoraEngine.SetupLocalVideo(null);
                        AgoraEngine.LeaveChannel();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                }
                Finish();
            }
            catch (Exception exception)
            {
                Finish();
                Console.WriteLine(exception);
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                AgoraEngine.StopPreview();
                AgoraEngine.SetupLocalVideo(null);
                AgoraEngine.LeaveChannel();
                AgoraEngine.Dispose();
                AgoraEngine = null;
                Finish();

                base.OnBackPressed();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                base.OnBackPressed();
            }
        }

        private void Mute_video_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (Mute_video_button.Selected)
                {
                    Mute_video_button.Selected = false;
                    Mute_video_button.SetImageResource(Resource.Drawable.ic_camera_video_open);
                }
                else
                {
                    Mute_video_button.Selected = true;
                    Mute_video_button.SetImageResource(Resource.Drawable.ic_camera_video_mute);
                }

                AgoraEngine.MuteLocalVideoStream(Mute_video_button.Selected);
                _isVideoEnabled = !Mute_video_button.Selected;
                FindViewById(Resource.Id.local_video_container).Visibility =
                    _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
                _localVideoView.Visibility = _isVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Switch_cam_button_Click(object sender, EventArgs e)
        {
            AgoraEngine.SwitchCamera();
        }

        public void OnFirstRemoteVideoDecoded(int uid, int width, int height, int elapsed)
        {
            RunOnUiThread(() => {
                SetupRemoteVideo(uid);
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
            });
        }

        protected override void OnDestroy()
        {
            try
            {
                //Close Api Starts here >>
                API_Request.Send_Agora_Call_Action_Async("close", CALL_ID).ConfigureAwait(false);

                if (AgoraEngine != null)
                {
                    AgoraEngine.StopPreview();
                    AgoraEngine.SetupLocalVideo(null);
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                }
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                base.OnDestroy();
            }
        }

        public void OnUserOffline(int uid, int reason)
        {
            RunOnUiThread(async () => {

                FrameLayout container = (FrameLayout)FindViewById(Resource.Id.remote_video_view_container);
                IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                container.RemoveAllViews();
                UserInfoview_container.Visibility = ViewStates.Visible;
                NoteTextView.Text = this.GetText(Resource.String.Lbl_Lost_his_connection);
                await Task.Delay(3000);
                try
                {
                    AgoraEngine.StopPreview();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                try
                {
                    AgoraEngine.SetupLocalVideo(null);
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                AgoraEngine = null;

                Finish();
            });
        }

        public void OnUserJoined(int uid, int reason)
        {
            RunOnUiThread(() => {
                UserInfoview_container.Visibility = ViewStates.Gone;
                NoteTextView.Text = "";
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
            });
        }

        public void OnUserMuteVideo(int uid, bool muted)
        {
            RunOnUiThread(() => {
                UserInfoview_container.Visibility = ViewStates.Visible;
                OnRemoteUserVideoMuted(uid, muted);
            });
        }

        public void OnConnectionLost()
        {
            RunOnUiThread(() => {
                Toast.MakeText(this, this.GetText(Resource.String.Lbl_Lost_Connection), ToastLength.Short).Show();
            });
        }

        public void OnNetworkQuality(int p0, int p1, int p2)
        {
            RunOnUiThread(() => {
                if (p1 == 3 || p2 == 3)
                {
                    //QUALITY_POOR(3)
                }
                else if (p1 == 4 || p2 == 4)
                {
                    //QUALITY_VBAD(5)
                }
                else if (p1 == 5 || p2 == 5)
                {
                    //QUALITY_DOWN(6)
                }
                else if (p1 == 6 || p2 == 6)
                {
                    //QUALITY_DOWN(6)
                }
            });
        }

        public void OnFirstLocalVideoFrame(float height, float width, int p2)
        {
            try
            {
                var ratio = height / width;
                var ratioHeight = ratio * MaxLocalVideoDimension;
                var ratioWidth = MaxLocalVideoDimension / ratio;
                var containerHeight = height > width ? MaxLocalVideoDimension : ratioHeight;
                var containerWidth = height > width ? ratioWidth : MaxLocalVideoDimension;
                RunOnUiThread(() => {
                    var videoContainer = FindViewById<RelativeLayout>(Resource.Id.local_video_container);
                    var parameters = videoContainer.LayoutParameters;
                    parameters.Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, containerHeight,
                        Resources.DisplayMetrics);
                    parameters.Width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, containerWidth,
                        Resources.DisplayMetrics);
                    videoContainer.LayoutParameters = parameters;
                    FindViewById(Resource.Id.local_video_container).Visibility =
                        _isVideoEnabled ? ViewStates.Visible : ViewStates.Invisible;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetupRemoteVideo(int uid)
        {
            try
            {
                FrameLayout container = (FrameLayout)FindViewById(Resource.Id.remote_video_view_container);
                if (container.ChildCount >= 1)
                {
                    return;
                }
                SurfaceView surfaceView = RtcEngine.CreateRendererView(BaseContext);
                container.AddView(surfaceView);
                AgoraEngine.SetupRemoteVideo(new VideoCanvas(surfaceView, VideoCanvas.RenderModeAdaptive, uid));
                surfaceView.Tag = uid; // for mark purpose
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitAgoraEngineAndJoinChannel(string Room_name)
        {
            try
            {
                InitializeAgoraEngine();
                AgoraEngine.EnableVideo();
                AgoraEngine.SetVideoProfile(AgoraSettings.VideoQuality, false);
                SetupLocalVideo();
                AgoraEngine.JoinChannel(null, Room_name, string.Empty, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetupLocalVideo()
        {
            try
            {
                FrameLayout container = (FrameLayout)FindViewById(Resource.Id.local_video_view_container);
                _localVideoView = RtcEngine.CreateRendererView(BaseContext);
                _localVideoView.SetZOrderMediaOverlay(true);
                container.AddView(_localVideoView);
                AgoraEngine.SetupLocalVideo(new VideoCanvas(_localVideoView, VideoCanvas.RenderModeAdaptive, 0));
                if (!string.IsNullOrEmpty(""))
                {
                    AgoraEngine.SetEncryptionMode("aes-128-xts");
                    AgoraEngine.SetEncryptionSecret("");
                }
                AgoraEngine.StartPreview();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitializeAgoraEngine()
        {
            try
            {
                AgoraHandler = new AgoraRtcHandler(this);
                AgoraEngine = RtcEngine.Create(BaseContext, AgoraSettings.AgoraAPI, AgoraHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnRemoteUserVideoMuted(int uid, bool muted)
        {

            FrameLayout container = (FrameLayout)FindViewById(Resource.Id.remote_video_view_container);
            SurfaceView surfaceView = (SurfaceView)container.GetChildAt(0);
            var tag = surfaceView.Tag;
            if (tag != null && (int)tag == uid)
            {
                NoteTextView.Text = this.GetText(Resource.String.Lbl_Muted_his_video);
                surfaceView.Visibility = muted ? ViewStates.Gone : ViewStates.Visible;
                if (muted)
                {
                    UserInfoview_container.Visibility = ViewStates.Visible;
                    NoteTextView.Text = this.GetText(Resource.String.Lbl_Muted_his_video);
                }
                else
                {
                    UserInfoview_container.Visibility = ViewStates.Gone;
                    NoteTextView.Text = "";
                }
            }
        }
    }
}