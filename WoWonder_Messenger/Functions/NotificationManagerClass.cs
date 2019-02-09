using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using WoWonder.Activities.Tab;
using WoWonder.Frameworks.Agora;
using WoWonder.Frameworks.Twilio;
using WoWonder.SQLite;
using Exception = System.Exception;
using String = System.String;


namespace WoWonder.Functions
{
    public class NotificationManagerClass
    {
        public static string NOTIFICATION_ID = "NOTIFICATION_ID";
        public static string CHANNEL_ID = "Channel_2018";

        public static string CallID = "";
        public static string UserID = "";
        public static string avatar = "";
        public static string name = "";
        public static string access_token = "";
        public static string access_token_2 = "";
        public static string from_id = "";
        public static string active = "";
        public static string time = "";
        public static string status = "";
        public static string room_name = "";
        public static string type = "";
        public static string Declined = "";

        public class NotificationActivity : AppCompatActivity
        {
            protected override void OnCreate(Bundle savedInstanceState)
            {
                try
                {
                    base.OnCreate(savedInstanceState);
                    var cc = Intent.GetIntExtra(NOTIFICATION_ID, 0);
                    notifyMgr.Cancel(cc);
                    Finish();
                    SendBroadcast(new Intent(Intent.ActionCloseSystemDialogs));

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public static PendingIntent getDismissIntent(int notificationId, Context context)
            {
                try
                {
                    Intent intent = new Intent(context, typeof(NotificationActivity));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.PutExtra(NOTIFICATION_ID, notificationId);
                    PendingIntent dismissIntent =
                        PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
                    NotificationManagerCompat.From(Application.Context).Cancel(notificationId);
                    return dismissIntent;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }

            public static PendingIntent GetAnswerIntent(int notificationId, Context context, Intent intentdata)
            {
                try
                {
                    PendingIntent dismissIntent;
                    if (intentdata.GetStringExtra("type") == "Twilio_video_call")
                    {
                        Intent intent = new Intent(context, typeof(TwilioVideoCallActivity));
                        intent.SetFlags(ActivityFlags.TaskOnHome | ActivityFlags.BroughtToFront);
                        intent.PutExtra("UserID", intentdata.GetStringExtra("UserID"));
                        intent.PutExtra("avatar", intentdata.GetStringExtra("avatar"));
                        intent.PutExtra("name", intentdata.GetStringExtra("name"));
                        intent.PutExtra("access_token", intentdata.GetStringExtra("access_token"));
                        intent.PutExtra("access_token_2", intentdata.GetStringExtra("access_token_2"));
                        intent.PutExtra("from_id", intentdata.GetStringExtra("from_id"));
                        intent.PutExtra("active", intentdata.GetStringExtra("active"));
                        intent.PutExtra("time", intentdata.GetStringExtra("time"));
                        intent.PutExtra("CallID", intentdata.GetStringExtra("CallID"));
                        intent.PutExtra("room_name", intentdata.GetStringExtra("room_name"));
                        intent.PutExtra("type", intentdata.GetStringExtra("type"));
                        intent.PutExtra("NOTIFICATION_ID", notificationId);

                        intent.PutExtra(NOTIFICATION_ID, notificationId);
                        dismissIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
                    }
                    else if (intentdata.GetStringExtra("type") == "Twilio_audio_call")
                    {
                        Intent intent = new Intent(context, typeof(TwilioAudioCallActivity));
                        intent.SetFlags(ActivityFlags.TaskOnHome | ActivityFlags.BroughtToFront);
                        intent.PutExtra("UserID", intentdata.GetStringExtra("UserID"));
                        intent.PutExtra("avatar", intentdata.GetStringExtra("avatar"));
                        intent.PutExtra("name", intentdata.GetStringExtra("name"));
                        intent.PutExtra("access_token", intentdata.GetStringExtra("access_token"));
                        intent.PutExtra("access_token_2", intentdata.GetStringExtra("access_token_2"));
                        intent.PutExtra("from_id", intentdata.GetStringExtra("from_id"));
                        intent.PutExtra("active", intentdata.GetStringExtra("active"));
                        intent.PutExtra("time", intentdata.GetStringExtra("time"));
                        intent.PutExtra("CallID", intentdata.GetStringExtra("CallID"));
                        intent.PutExtra("room_name", intentdata.GetStringExtra("room_name"));
                        intent.PutExtra("type", intentdata.GetStringExtra("type"));
                        intent.PutExtra("NOTIFICATION_ID", notificationId);

                        intent.PutExtra(NOTIFICATION_ID, notificationId);
                        dismissIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
                    }
                    else if (intentdata.GetStringExtra("type") == "Agora_audio_call_recieve")
                    {
                        Intent intent = new Intent(context, typeof(AgoraAudioCallActivity));
                        intent.SetFlags(ActivityFlags.TaskOnHome | ActivityFlags.BroughtToFront);
                        intent.PutExtra("UserID", intentdata.GetStringExtra("UserID"));
                        intent.PutExtra("avatar", intentdata.GetStringExtra("avatar"));
                        intent.PutExtra("name", intentdata.GetStringExtra("name"));
                        intent.PutExtra("time", intentdata.GetStringExtra("time"));
                        intent.PutExtra("CallID", intentdata.GetStringExtra("CallID"));
                        intent.PutExtra("room_name", intentdata.GetStringExtra("room_name"));
                        intent.PutExtra("type", intentdata.GetStringExtra("type"));

                        intent.PutExtra(NOTIFICATION_ID, notificationId);
                        dismissIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
                    }
                    else if (intentdata.GetStringExtra("type") == "Agora_video_call_recieve")
                    {
                        Intent intent = new Intent(context, typeof(AgoraVideoCallActivity));
                        intent.SetFlags(ActivityFlags.TaskOnHome | ActivityFlags.BroughtToFront);
                        intent.PutExtra("UserID", intentdata.GetStringExtra("UserID"));
                        intent.PutExtra("avatar", intentdata.GetStringExtra("avatar"));
                        intent.PutExtra("name", intentdata.GetStringExtra("name"));
                        intent.PutExtra("time", intentdata.GetStringExtra("time"));
                        intent.PutExtra("CallID", intentdata.GetStringExtra("CallID"));
                        intent.PutExtra("room_name", intentdata.GetStringExtra("room_name"));
                        intent.PutExtra("type", intentdata.GetStringExtra("type"));
                        intent.PutExtra(NOTIFICATION_ID, notificationId);
                        dismissIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.CancelCurrent);
                    }
                    else
                    {
                        dismissIntent = PendingIntent.GetActivity(context, 0, null, PendingIntentFlags.CancelCurrent);
                    }

                    //Close Notification Call
                    Intent intentCloser = new Intent(context, typeof(NotificationBroadcasterCloser));
                    intentCloser.PutExtra(NOTIFICATION_ID, notificationId);
                    context.SendBroadcast(intentCloser);
                    return dismissIntent;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        public static NotificationManager notifyMgr;

        public static void StartinCommingCall(Intent Intent, string ImageProfile, string Title, string Long_Description,
            int Notification_id)
        {
            try
            {
                notifyMgr = (NotificationManager) Application.Context.GetSystemService(Context.NotificationService);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    // Create the NotificationChannel, but only on API 26+ because
                    // the NotificationChannel class is new and not in the support library
                    var channel = new NotificationChannel(CHANNEL_ID, "Message_Notifciation_Channel_1",NotificationImportance.Default); //high
                    var sound = Android.Net.Uri.Parse("android.resource://" + Application.Context.PackageName + "/raw/mystic_call");

                    AudioAttributes attributes = new AudioAttributes.Builder()
                        .SetUsage(AudioUsageKind.Notification)
                        .Build();
                    // Configure the notification channel.
                    channel.EnableLights(true);
                    channel.SetSound(sound, attributes); // This is IMPORTANT

                    channel.Description = Application.Context.GetText(Resource.String.Lbl_Notification_of_Messages);
                    channel.EnableVibration(true);
                    channel.LockscreenVisibility = NotificationVisibility.Public;

                    notifyMgr?.CreateNotificationChannel(channel);
                }
               
                var data_CALL_ID = Intent.GetStringExtra("CallID") ?? "Data not available";
                if (data_CALL_ID != "Data not available" && !String.IsNullOrEmpty(data_CALL_ID))
                {
                    CallID = data_CALL_ID;

                    UserID = Intent.GetStringExtra("UserID");
                    avatar = Intent.GetStringExtra("avatar");
                    name = Intent.GetStringExtra("name");
                    access_token = Intent.GetStringExtra("access_token");
                    access_token_2 = Intent.GetStringExtra("access_token_2");
                    from_id = Intent.GetStringExtra("from_id");
                    active = Intent.GetStringExtra("active");
                    time = Intent.GetStringExtra("time");
                    status = Intent.GetStringExtra("status");
                    room_name = Intent.GetStringExtra("room_name");
                    type = Intent.GetStringExtra("type");
                    Declined = Intent.GetStringExtra("declined");
                }

                var NotificationBroadcasterAction =
                    new Intent(Application.Context, typeof(NotificationBroadcasterCloser));
                NotificationBroadcasterAction.PutExtra(NOTIFICATION_ID, Notification_id);
                NotificationBroadcasterAction.PutExtra("type", type);
                NotificationBroadcasterAction.PutExtra("CallID", CallID);
                NotificationBroadcasterAction.PutExtra("action", "dismiss");

                PendingIntent GetAnswerIntent =
                    NotificationManagerClass.NotificationActivity.GetAnswerIntent(Notification_id, Application.Context,
                        Intent);
                PendingIntent MainIntent = PendingIntent.GetActivity(Application.Context, Notification_id,
                    new Intent(Application.Context, typeof(Tabbed_Main_Page)), 0);
                PendingIntent cancelIntent = PendingIntent.GetBroadcast(Application.Context, Notification_id,
                    NotificationBroadcasterAction, 0);

                NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context);

                if (!String.IsNullOrEmpty(ImageProfile))
                {
                    var Image = GetBitmapFromPath(ImageProfile);
                    builder.SetLargeIcon(Image);
                }

                if ((int) Android.OS.Build.VERSION.SdkInt >= 21)
                    builder.SetCategory(Notification.CategoryCall).SetVisibility(NotificationCompat.VisibilityPublic);

                var uri = Android.Net.Uri.Parse("android.resource://" + Application.Context.PackageName +"/raw/mystic_call");
                
                var Vibrate = new long[]
                {
                    1000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000,
                    2000, 1000, 2000, 1000, 2000, 1000, 2000, 1000, 2000
                };
                builder.SetPriority(NotificationCompat.PriorityMax);
                builder.SetColorized(true);
                builder.SetColor(Color.ParseColor(AppSettings.MainColor)).SetDeleteIntent(cancelIntent);
                builder.SetSmallIcon(Resource.Drawable.icon)
                    .SetContentTitle(Title).SetAutoCancel(true)
                    .SetContentText(Long_Description)
                    .SetChannelId(CHANNEL_ID)
                    .SetContentIntent(cancelIntent).SetLights(Color.Red, 3000, 3000)
                    .SetSound(uri)
                    .SetOngoing(true).SetVibrate(Vibrate)
                    .SetFullScreenIntent(MainIntent, true)
                    .AddAction(Resource.Drawable.ic_close_black_24dp, Application.Context.GetText(Resource.String.Lbl_Dismiss), cancelIntent)
                    .AddAction(Resource.Drawable.ic_action_notifcation_phone, Application.Context.GetText(Resource.String.Lbl_Answer), GetAnswerIntent);

                Notification NotificationBuild = builder.Build();
                NotificationBuild.Color = Color.ParseColor(AppSettings.MainColor);
                NotificationBuild.Flags |= NotificationFlags.Insistent;
                NotificationBuild.Sound = uri;
                NotificationBuild.Vibrate = Vibrate;
                builder.Notification.Flags |= NotificationFlags.AutoCancel;

            
                if (Declined == "0")
                {
                    notifyMgr.Notify(0, NotificationBuild);
                }
                else
                {
                    Application.Context.SendBroadcast(NotificationBroadcasterAction); //Close all incomming calls
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static Bitmap GetBitmapFromPath(String photoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            return BitmapFactory.DecodeFile(photoPath, options);
        }

        public static void CloseinCommingCallNotification(string Id)
        {
            try
            {
                int useridNumber = Int32.Parse(Id);
                notifyMgr.Cancel(useridNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [BroadcastReceiver()]
        [IntentFilter(new[] {"select.notif.id"})]
        public class NotificationBroadcasterCloser : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    int notificationId = intent.GetIntExtra("NOTIFICATION_ID", 0);

                    NotificationManager notifyMgr =
                        (NotificationManager) Application.Context.GetSystemService(Context.NotificationService);
                    notifyMgr.Cancel(notificationId);

                    var Call_id = intent.GetStringExtra("CallID");
                    var isDismiss = intent.GetStringExtra("action");
                    if (isDismiss == "dismiss")
                    {
                        if (intent.GetStringExtra("type") == "Twilio_video_call")
                        {
                            API_Request.Send_Twilio_Video_Call_Answer_Async("decline", Call_id).ConfigureAwait(false);
                        }
                        else if (intent.GetStringExtra("type") == "Twilio_audio_call")
                        {
                            API_Request.Send_Twilio_Audio_Call_Answer_Async("decline", Call_id).ConfigureAwait(false);
                        }
                        else if (intent.GetStringExtra("type") == "Agora_audio_call_recieve")
                        {
                            API_Request.Send_Agora_Call_Action_Async("decline", Call_id).ConfigureAwait(false);
                        }
                        else if (intent.GetStringExtra("type") == "Agora_video_call_recieve")
                        {
                            API_Request.Send_Agora_Call_Action_Async("decline", Call_id).ConfigureAwait(false);
                        }

                        var ckd = Last_Calls_Fragment.mAdapter?.mCallUser?.FirstOrDefault(a =>
                            a.id == CallID); // id >> Call_Id
                        if (ckd == null)
                        {
                            Classes.Call_User cv = new Classes.Call_User();
                            cv.id = CallID;
                            cv.user_id = UserID;
                            cv.avatar = avatar;
                            cv.name = name;
                            cv.access_token = access_token;
                            cv.access_token_2 = access_token_2;
                            cv.from_id = from_id;
                            cv.active = active;
                            cv.time = "Missed call";
                            cv.status = status;
                            cv.room_name = room_name;
                            cv.type = type;
                            cv.typeIcon = "Cancel";
                            cv.typeColor = "#FF0000";

                            Last_Calls_Fragment.mAdapter?.Insert(cv);

                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.Insert_CallUser(cv);
                            dbDatabase.Dispose();
                        }

                       
                        notifyMgr.CancelAll();
                    }
                    else
                    {
                       
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        [Service]
        public class BackgroundRunner : IntentService
        {
            protected override void OnHandleIntent(Intent intent)
            {
                try
                {
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}