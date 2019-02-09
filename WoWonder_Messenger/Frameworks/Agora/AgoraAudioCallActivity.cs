using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using DT.Xamarin.Agora;
using FFImageLoading.Views;
using ME.Alexrs.Wavedrawable;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;

namespace WoWonder.Frameworks.Agora
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation, ResizeableActivity = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AgoraAudioCallActivity : AppCompatActivity
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
        protected AgoraRtcAudioCallHandler AgoraHandler;

        private bool _isVideoEnabled = true;
        private SurfaceView _localVideoView;

        private CircleButton End_call_button;
        private CircleButton Speaker_audio_button;
        private CircleButton Mute_audio_button;
        private RelativeLayout UserInfoview_container;
        private ImageView UserImageViewAsync;
        private TextView UserNameTextView;
        private TextView DurationTextView;
        private Timer TimerSound;
        private WaveDrawable WaveDrawbleAnimination;

        private int CountSecoundsOfOutgoingCall = 0;
        private Timer TimerRequestWaiter = new Timer();

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.AgoraAudioCallActivityLayout);

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

                Intent intentCloser = new Intent(this, typeof(NotificationManagerClass.NotificationBroadcasterCloser));
                intentCloser.PutExtra("action", "answer");
                intentCloser.PutExtra("type", Intent.GetStringExtra("type"));
                SendBroadcast(intentCloser);

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

            
                LoadUserInfo(UserID);

                if (CALL_TYPE == "Agora_audio_calling_start")
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
                    DurationTextView.Text = this.GetText(Resource.String.Lbl_Calling);
                    var ApiStartCall = await API_Request.Create_Agora_Call_Event_Async(UserID, "audio");
                    if (ApiStartCall != null && !string.IsNullOrEmpty(ApiStartCall.room_name) && !string.IsNullOrEmpty(ApiStartCall.id))
                    {
                        if (!string.IsNullOrEmpty(ApiStartCall.room_name))
                            ROOM_NAME = ApiStartCall.room_name;
                        if (!string.IsNullOrEmpty(ApiStartCall.id))
                            CALL_ID = ApiStartCall.id;
                        IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3");

                        TimerRequestWaiter = new Timer();
                        TimerRequestWaiter.Interval = 5000;
                        TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                        TimerRequestWaiter.Start();
                        InitAgoraEngineAndJoinChannel(ROOM_NAME); //the caller Is Joining agora Server
                    }
                }
                else
                {
                    ROOM_NAME = Intent.GetStringExtra("room_name");
                    CALL_ID = Intent.GetStringExtra("CallID");
                    DurationTextView.Text = this.GetText(Resource.String.Lbl_Waiting_to_connect);

                    var ApiStartCall = await API_Request.Send_Agora_Call_Action_Async("answer", CALL_ID);
                    if (ApiStartCall == "200")
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
                        InitAgoraEngineAndJoinChannel(ROOM_NAME); //the caller Is Joining agora Server
                    }
                    else
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

                        DurationTextView.Text = this.GetText(Resource.String.Lbl_Faild_to_connect);
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
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                DurationTextView.Text = this.GetText(Resource.String.Lbl_No_respond_from_the_user);
                await Task.Delay(3000);
                AgoraEngine.LeaveChannel();
                AgoraEngine.Dispose();
                AgoraEngine = null;
                Finish();
            }
            catch (Exception e)
            {
                Finish();
                Console.WriteLine(e);
            }
        }

        public async void OnCall_Declined_From_User()
        {
            try
            {
                IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                DurationTextView.Text = this.GetText(Resource.String.Lbl_The_user_declinde_your_call);
                await Task.Delay(3000);
                AgoraEngine.LeaveChannel();
                AgoraEngine.Dispose();
                AgoraEngine = null;
                Finish();
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
                var Call_Result_Generation = await API_Request.Check_Agora_Call_Answer_Async(CALL_ID, "audio");

                if (string.IsNullOrEmpty(Call_Result_Generation))
                    return;

                if (Call_Result_Generation == "answered")
                {

                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();

                    RunOnUiThread(() =>
                    {
                        IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                        InitAgoraEngineAndJoinChannel(ROOM_NAME);
                    });

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

        private void Speaker_audio_button_Click(object sender, EventArgs e)
        {
            try
            {
                //Speaker
                if (Speaker_audio_button.Selected)
                {
                    Speaker_audio_button.Selected = false;
                    Speaker_audio_button.SetImageResource(Resource.Drawable.ic_speaker_close);
                    AgoraEngine.SetEnableSpeakerphone(false);
                }
                else
                {
                    Speaker_audio_button.Selected = true;

                    Speaker_audio_button.SetImageResource(Resource.Drawable.ic_speaker_up);
                    AgoraEngine.SetEnableSpeakerphone(true);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnBackPressed()
        {
            //Close Api Starts here >>
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
            }
            catch (Exception exception)
            {
                base.OnBackPressed();
                Console.WriteLine(exception);
            }
            base.OnBackPressed();
        }

        public void InitAgoraEngineAndJoinChannel(string room_name)
        {
            try
            {
                AgoraHandler = new AgoraRtcAudioCallHandler(this);
                AgoraEngine = RtcEngine.Create(BaseContext, AgoraSettings.AgoraAPI, AgoraHandler);

                AgoraEngine.DisableVideo();
                AgoraEngine.EnableAudio();
                AgoraEngine.SetEncryptionMode("aes-128-xts");
                AgoraEngine.SetEncryptionSecret("");
                AgoraEngine.JoinChannel(null, room_name, string.Empty, 0);
                AgoraEngine.SetEnableSpeakerphone(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnUserOffline(int uid, int reason)
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                    DurationTextView.Text = this.GetText(Resource.String.Lbl_Lost_his_connection);
                    await Task.Delay(3000);
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                    Finish();
                }
                catch (Exception e)
                {
                    Finish();
                    Console.WriteLine(e);
                }
            });
        }

        public void OnUserJoined(int uid, int reason)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    DurationTextView.Text = this.GetText(Resource.String.Lbl_Please_wait);
                    IMethods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    WaveDrawbleAnimination.StopAnimation();

                    TimerSound = new Timer();
                    TimerSound.Interval = 1000;
                    TimerSound.Elapsed += TimerSound_Elapsed;
                    TimerSound.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            try
            {
                //Close Api Starts here >>
                API_Request.Send_Agora_Call_Action_Async("close", CALL_ID).ConfigureAwait(false);

                if (AgoraEngine != null)
                {
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TimerSound_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                //Write your own duration function here 
                string s = TimeSpan.FromSeconds(e.SignalTime.Second).ToString(@"mm\:ss");
                DurationTextView.Text = s;
            });
        }

        public void OnConnectionLost()
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, this.GetText(Resource.String.Lbl_Lost_Connection), ToastLength.Short).Show();

                Finish();
            });
        }

        public void OnNetworkQuality(int p0, int p1, int p2)
        {
            RunOnUiThread(() =>
            {
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
    }
}