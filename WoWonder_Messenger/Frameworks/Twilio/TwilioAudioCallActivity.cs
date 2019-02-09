using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading.Views;
using ME.Alexrs.Wavedrawable;
using TwilioVideo;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;

namespace WoWonder.Frameworks.Twilio
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation, ResizeableActivity = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class TwilioAudioCallActivity : AppCompatActivity, TwilioVideoHelper.IListener
    {
        public TwilioVideoHelper TwilioVideo { get; private set; }

        public static string TWILIO_ACCESS_TOKEN = "YOUR_TOKEN";
        public static string TWILIO_ACCESS_TOKEN_USER2 = "YOUR_TOKEN";
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

        private CircleButton End_call_button;
        private CircleButton Speaker_audio_button;
        private CircleButton Mute_audio_button;
        private RelativeLayout UserInfoview_container;
        private ImageView UserImageViewAsync;
        private TextView UserNameTextView;
        private TextView DurationTextView;
        private Timer TimerRequestWaiter = new Timer();
        private WaveDrawable WaveDrawbleAnimination;

        TwilioVideo.VideoView _UserprimaryVideo;
        TwilioVideo.VideoView _thumbnailVideo;
        private LocalVideoTrack LocalvideoTrack;
        private VideoTrack UserVideoTrack;

        const int PermissionsRequestCode = 1;
        bool _dataUpdated;
        private int CountSecoundsOfOutgoingCall = 0;
        string _localVideoTrackId;
        string _remoteVideoTrackId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.TwilioAudioCallActivityLayout);
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                // Create your application here
                Intent intentCloser = new Intent(this, typeof(NotificationManagerClass.NotificationBroadcasterCloser));
                intentCloser.PutExtra("action", "answer");
                intentCloser.PutExtra("type", Intent.GetStringExtra("type"));
                SendBroadcast(intentCloser);

                UserID = Intent.GetStringExtra("UserID");
                avatar = Intent.GetStringExtra("avatar");
                name = Intent.GetStringExtra("name");

                var data_CALL_ID = Intent.GetStringExtra("CallID") ?? "Data not available";
                if (data_CALL_ID != "Data not available" && !string.IsNullOrEmpty(data_CALL_ID))
                {
                    CALL_ID = data_CALL_ID;

                    TWILIO_ACCESS_TOKEN = Intent.GetStringExtra("access_token");
                    TWILIO_ACCESS_TOKEN_USER2 = Intent.GetStringExtra("access_token_2");
                    from_id = Intent.GetStringExtra("from_id");
                    active = Intent.GetStringExtra("active");
                    time = Intent.GetStringExtra("time");
                    status = Intent.GetStringExtra("status");
                    ROOM_NAME = Intent.GetStringExtra("room_name");
                    CALL_TYPE = Intent.GetStringExtra("type");
                }

                Speaker_audio_button = FindViewById<CircleButton>(Resource.Id.speaker_audio_button);
                End_call_button = FindViewById<CircleButton>(Resource.Id.end_audio_call_button);
                Mute_audio_button = FindViewById<CircleButton>(Resource.Id.mute_audio_call_button);

                UserImageViewAsync = FindViewById<ImageView>(Resource.Id.audiouserImageViewAsync);
                UserNameTextView = FindViewById<TextView>(Resource.Id.audiouserNameTextView);
                DurationTextView = FindViewById<TextView>(Resource.Id.audiodurationTextView);

                Speaker_audio_button.Click += Speaker_audio_button_Click;
                End_call_button.Click += End_call_button_Click;
                Mute_audio_button.Click += Mute_audio_button_Click;

                Speaker_audio_button.SetImageResource(Resource.Drawable.ic_speaker_close);

              

                bool granted =
                    ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) ==
                    Permission.Granted &&
                    ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.RecordAudio) ==
                    Permission.Granted;

                CheckVideoCallPermissions(granted);

                if (CALL_TYPE == "Twilio_audio_call")
                {
                    if (!string.IsNullOrEmpty(TWILIO_ACCESS_TOKEN))
                    {
                        if (!string.IsNullOrEmpty(UserID))
                            Load_userWhenCall(UserID);

                        TwilioVideo = TwilioVideoHelper.GetOrCreate(ApplicationContext);
                        UpdateState();
                        DurationTextView.Text = this.GetText(Resource.String.Lbl_Waiting_for_answer);
                        var Check_for_ResponseDate = API_Request.Send_Twilio_Audio_Call_Answer_Async("answer", CALL_ID);
                        ConnectToRoom();

                        var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a =>
                            a.id == CALL_ID); // id >> Call_Id
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
                }
                else if (CALL_TYPE == "Twilio_audio_calling_start")
                {
                    if (!string.IsNullOrEmpty(UserID))
                    {
                        LoadProfileFromUserID(UserID);
                        Load_userWhenCall(UserID);
                    }

                    DurationTextView.Text = this.GetText(Resource.String.Lbl_Calling);
                    TwilioVideo = TwilioVideoHelper.GetOrCreate(ApplicationContext);

                    IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3");

                    UpdateState();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public void Load_userWhenCall(string User_ID)
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

        public async void LoadProfileFromUserID(string User_ID)
        {
            try
            {
                Load_userWhenCall(User_ID);
                var Call_Result_Generation = await API_Request.Create_Twilio_Audio_Call_Answer_Async(User_ID);
                if (Call_Result_Generation != null)
                {
                    CALL_ID = Call_Result_Generation.id;
                    TWILIO_ACCESS_TOKEN = Call_Result_Generation.access_token;
                    TWILIO_ACCESS_TOKEN_USER2 = Call_Result_Generation.access_token_2;
                    ROOM_NAME = Call_Result_Generation.room_name;
                  

                    TimerRequestWaiter = new Timer();
                    TimerRequestWaiter.Interval = 5000;
                    TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                    TimerRequestWaiter.Start();
                }
                else
                {
                    FinishCall(true);
                 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void TimerCallRequestAnswer_Waiter_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var Call_Result_Generation = await API_Request.Check_Twilio_Call_Answer_Async(CALL_ID, "audio");

                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();

                if (string.IsNullOrEmpty(Call_Result_Generation))
                    return;

                if (Call_Result_Generation == "200")
                {
                    if (!string.IsNullOrEmpty(TWILIO_ACCESS_TOKEN))
                    {
                        TimerRequestWaiter.Enabled = false;
                        TimerRequestWaiter.Stop();
                        TimerRequestWaiter.Close();

                        RunOnUiThread(async () =>
                        {
                            await Task.Delay(1000);

                            TwilioVideo.UpdateToken(TWILIO_ACCESS_TOKEN_USER2);
                            TwilioVideo.JoinRoom(ApplicationContext, ROOM_NAME);

                            var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a =>
                                a.id == CALL_ID); // id >> Call_Id
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
                        });
                    }
                }
                else if (Call_Result_Generation == "300")
                {
                    if (CountSecoundsOfOutgoingCall < 70)
                    {
                        CountSecoundsOfOutgoingCall += 10;
                    }
                    else
                    {
                        //Call Is inactive 
                        TimerRequestWaiter.Enabled = false;
                        TimerRequestWaiter.Stop();
                        TimerRequestWaiter.Close();

                        var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a =>
                            a.id == CALL_ID); // id >> Call_Id
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
                                time = "Missed call",
                                status = status,
                                room_name = ROOM_NAME,
                                type = CALL_TYPE,
                                typeIcon = "Cancel",
                                typeColor = "#FF0000"
                            };

                            Last_Calls_Fragment.mAdapter?.Insert(cv);

                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.Insert_CallUser(cv);
                            dbDatabase.Dispose();
                        }

                        FinishCall(true);
                    }
                }
                else
                {
                    //Call Is inactive 
                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();

                    var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a =>
                        a.id == CALL_ID); // id >> Call_Id
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

                    FinishCall(true);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Speaker_audio_button_Click(object sender, EventArgs e)
        {
            try
            {
                //Speaker
                if (Speaker_audio_button.Selected)
                {
                    Speaker_audio_button.Selected = false;
                    Speaker_audio_button.SetImageResource(Resource.Drawable.ic_speaker_close);
                
                }
                else
                {
                    Speaker_audio_button.Selected = true;

                    Speaker_audio_button.SetImageResource(Resource.Drawable.ic_speaker_up);
           
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

                TwilioVideo.Mute(Mute_audio_button.Selected);
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
                FinishCall(true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public async void ConnectToRoom()
        {
            TwilioVideo.UpdateToken(TWILIO_ACCESS_TOKEN);
            TwilioVideo.JoinRoom(ApplicationContext, ROOM_NAME);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            TwilioVideo = TwilioVideoHelper.GetOrCreate(ApplicationContext);
            UpdateState();
        }

        public bool OnSupportNavigateUp()
        {
            TryCancelCall();
            return true;
        }

        public override void OnBackPressed()
        {
            FinishCall(true);
          
        }

        protected override void OnStart()
        {
            base.OnStart();
            UpdateState();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateState();
          
        }

        protected override void OnPause()
        {
           
            _dataUpdated = false;
            base.OnPause();
        }

        void UpdateState()
        {
            try
            {
                if (_dataUpdated)
                    return;
                _dataUpdated = true;
                TwilioVideo.Bind(this);
                UpdatingState();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected virtual void UpdatingState()
        {
        }

        protected void TryCancelCall()
        {
            CloseScreen();
        }

        void OnCallCanceled()
        {
            CloseScreen();
        }

        protected void SoftCloseScreen()
        {
            CloseScreen();
        }

        protected void CloseScreen()
        {
            Finish();
        }

        protected virtual void FinishCall(bool hangup)
        {
            try
            {
                if (TwilioVideo.ClientIsReady)
                {
                    TwilioVideo.Unbind(this);
                    TwilioVideo.FinishCall();
                }

                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void RequestCameraAndMicrophonePermissions()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera) ||
                ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.RecordAudio))
                Toast.MakeText(this, this.GetText(Resource.String.Lbl_Need_Camera), ToastLength.Long).Show();
            else
                ActivityCompat.RequestPermissions(this,
                    new string[] {Manifest.Permission.Camera, Manifest.Permission.RecordAudio}, PermissionsRequestCode);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == PermissionsRequestCode)
                CheckVideoCallPermissions(grantResults.Any(p => p == Permission.Denied));
        }

        void CheckVideoCallPermissions(bool granted)
        {
            if (!granted)
                RequestCameraAndMicrophonePermissions();
        }

        #region TwilioVideo.IListener

        public void SetLocalVideoTrack(LocalVideoTrack videoTracklocal)
        {
            try
            {
                if (LocalvideoTrack == null)
                {
                    LocalvideoTrack = videoTracklocal;
                    var trackId = videoTracklocal?.TrackId;
                    if (_localVideoTrackId == trackId)
                    {
                        LocalvideoTrack.Enable(false);
                        return;
                    }
                    else
                    {
                        _localVideoTrackId = trackId;
                        LocalvideoTrack.Enable(false);

                     
                    }
                }
                else
                {
                    if (LocalvideoTrack.IsEnabled)
                    {
                        LocalvideoTrack.Enable(false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetRemoteVideoTrack(TwilioVideo.VideoTrack videoTrack)
        {
            try
            {
                var trackId = videoTrack?.TrackId;

                if (_remoteVideoTrackId == trackId)
                    return;

                _remoteVideoTrackId = trackId;
                if (UserVideoTrack == null)
                {
                    UserVideoTrack = videoTrack;
                  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveLocalVideoTrack(LocalVideoTrack track)
        {
            SetLocalVideoTrack(null);
        }

        public void RemoveRemoteVideoTrack(TwilioVideo.VideoTrack track)
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public virtual void OnRoomConnected(string roomId)
        {

        }

        public virtual void OnRoomDisconnected(TwilioVideoHelper.StopReason reason)
        {
         
            Toast.MakeText(this, this.GetText(Resource.String.Lbl_Room_Disconnected), ToastLength.Short).Show();
      
        }

        public virtual void OnParticipantConnected(string participantId)
        {
            try
            {
                DurationTextView.Text = this.GetText(Resource.String.Lbl_Connected);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public virtual void OnParticipantDisconnected(string participantId)
        {
            RunOnUiThread(async () =>
            {
                DurationTextView.Text = this.GetText(Resource.String.Lbl_User_Lost_Connection);
                await Task.Delay(2000);
                FinishCall(true);
            });
        }

        #endregion

        public void SetCallTime(int seconds)
        {
            DurationTextView.Text = seconds.ToString();
        }
    }
}