using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using WoWonder.Adapters;
using WoWonder.Frameworks.Agora;
using WoWonder.Frameworks.Twilio;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using Fragment = Android.Support.V4.App.Fragment;


namespace WoWonder.Activities.Tab
{
    public class Last_Calls_Fragment : Fragment
    {

        #region Variables Basic

        public static RecyclerView LastCallsRecyler;
        public static LinearLayout LastCalls_Empty;
        private TextView Icon_lastmesseges;

        public LinearLayoutManager mLayoutManager;
        public static Last_Calls_Adapter mAdapter;

        public string TimeNow = DateTime.Now.ToString("hh:mm");
        public static Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public string Time = Convert.ToString(unixTimestamp);

        #endregion

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {

                Context contextThemeWrapper = new ContextThemeWrapper(Activity, Resource.Style.MyTheme);
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);
                // Set our view from the "Last_Calls_Fragment" layout resource
                View view = localInflater.Inflate(Resource.Layout.Last_Calls_Fragment, container, false);

                //Get values
                LastCallsRecyler = (RecyclerView) view.FindViewById(Resource.Id.lastcallsrecyler);
                LastCalls_Empty = (LinearLayout) view.FindViewById(Resource.Id.lastcall_LinerEmpty);
                Icon_lastmesseges = (AppCompatTextView) view.FindViewById(Resource.Id.calls_icon);

                IMethods.Set_TextViewIcon("1", Icon_lastmesseges, IonIcons_Fonts.AndroidCall);
                Icon_lastmesseges.SetTextColor(Android.Graphics.Color.ParseColor(AppSettings.MainColor));

                LastCallsRecyler.SetItemAnimator(null);

                LastCallsRecyler.Visibility = ViewStates.Visible;
                LastCalls_Empty.Visibility = ViewStates.Gone;

                mLayoutManager = new LinearLayoutManager(this.Context);
                LastCallsRecyler.SetLayoutManager(mLayoutManager);
                mAdapter = new Last_Calls_Adapter(this.Context);
                mAdapter.mCallUser = new ObservableCollection<Classes.Call_User>();
                mAdapter.CallClick += MAdapterOnCallClick;

                LastCallsRecyler.SetAdapter(mAdapter);

                Get_CallUser();

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
 
        public void Get_CallUser()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();

                var LocalList = dbDatabase.Get_CallUserList();
                if (LocalList?.Count > 0)
                {
                    if (mAdapter.mCallUser.Count > 0)
                    {
                        //Bring new users
                        var listnew = LocalList.Where(c => !mAdapter.mCallUser.Select(fc => fc.id).Contains(c.id)).ToList(); // id >> Call_Id
                        if (listnew.Count > 0)
                        {
                            //Results differ
                            Classes.AddRange(mAdapter.mCallUser, listnew);

                            var callUser =
                                new ObservableCollection<Classes.Call_User>(mAdapter.mCallUser.OrderBy(a => a.id));
                            mAdapter.mCallUser = new ObservableCollection<Classes.Call_User>(callUser);
                            mAdapter.BindEnd();
                        }
                        else
                        {
                            mAdapter.mCallUser =
                                new ObservableCollection<Classes.Call_User>(LocalList.OrderBy(a => a.id));
                            mAdapter.BindEnd();
                        }
                    }
                    else
                    {
                        mAdapter.mCallUser = new ObservableCollection<Classes.Call_User>(LocalList.OrderBy(a => a.id));
                        mAdapter.BindEnd();
                    }
                }

                if (mAdapter.mCallUser.Count > 0)
                {
                    LastCallsRecyler.Visibility = ViewStates.Visible;
                    LastCalls_Empty.Visibility = ViewStates.Gone;
                }
                else
                {
                    LastCallsRecyler.Visibility = ViewStates.Gone;
                    LastCalls_Empty.Visibility = ViewStates.Visible;
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MAdapterOnCallClick(object sender, Last_Calls_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                TimeNow = DateTime.Now.ToString("hh:mm");
                Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                Time = Convert.ToString(unixTimestamp);

                if (AppSettings.Enable_Audio_Call && AppSettings.Enable_Video_Call)
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(Activity, Resource.Style.AlertDialogCustom);
                    var dd = this.GetText(Resource.String.Lbl_Call);
                    builder.SetTitle(this.GetText(Resource.String.Lbl_Call));
                    builder.SetMessage(this.GetText(Resource.String.Lbl_Select_Type_Call));

                    builder.SetPositiveButton(this.GetText(Resource.String.Lbl_Voice_call), delegate(object o, DialogClickEventArgs args)
                    {
                        try
                        {
                            var Position = adapterClickEvents.Position;
                            if (Position >= 0)
                            {
                                var item = mAdapter.GetItem(Position);
                                if (item != null)
                                {
                                    Intent IntentOfvideoCall =
                                        new Intent(this.Context, typeof(TwilioVideoCallActivity));
                                    if (AppSettings.Use_Agora_Library && AppSettings.Use_Twilio_Library == false)
                                    {
                                        IntentOfvideoCall = new Intent(this.Context, typeof(AgoraAudioCallActivity));
                                        IntentOfvideoCall.PutExtra("type", "Agora_audio_calling_start");
                                    }
                                    else if (AppSettings.Use_Agora_Library == false && AppSettings.Use_Twilio_Library)
                                    {
                                        IntentOfvideoCall = new Intent(this.Context, typeof(TwilioAudioCallActivity));
                                        IntentOfvideoCall.PutExtra("type", "Twilio_audio_calling_start");
                                    }

                                    IntentOfvideoCall.PutExtra("UserID", item.user_id);
                                    IntentOfvideoCall.PutExtra("avatar", item.avatar);
                                    IntentOfvideoCall.PutExtra("name", item.name);
                                    IntentOfvideoCall.PutExtra("time", TimeNow);
                                    IntentOfvideoCall.PutExtra("CallID", Time);
                                    StartActivity(IntentOfvideoCall);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    });

                    builder.SetNegativeButton(this.GetText(Resource.String.Lbl_Video_call), delegate(object o, DialogClickEventArgs args)
                    {
                        try
                        {
                            var Position = adapterClickEvents.Position;
                            if (Position >= 0)
                            {
                                var item = mAdapter.GetItem(Position);
                                if (item != null)
                                {
                                    Intent IntentOfvideoCall =
                                        new Intent(this.Context, typeof(TwilioVideoCallActivity));
                                    if (AppSettings.Use_Agora_Library && AppSettings.Use_Twilio_Library == false)
                                    {
                                        IntentOfvideoCall = new Intent(this.Context, typeof(AgoraVideoCallActivity));
                                        IntentOfvideoCall.PutExtra("type", "Agora_video_calling_start");
                                    }
                                    else if (AppSettings.Use_Agora_Library == false && AppSettings.Use_Twilio_Library)
                                    {
                                        IntentOfvideoCall = new Intent(this.Context, typeof(TwilioVideoCallActivity));
                                        IntentOfvideoCall.PutExtra("type", "Twilio_video_calling_start");
                                    }

                                    IntentOfvideoCall.PutExtra("UserID", item.user_id);
                                    IntentOfvideoCall.PutExtra("avatar", item.avatar);
                                    IntentOfvideoCall.PutExtra("name", item.name);
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
                    });

                    var alert = builder.Create();
                    alert.Show();
                }
                else if (AppSettings.Enable_Audio_Call == false && AppSettings.Enable_Video_Call) // Video Call On
                {
                    try
                    {
                        var Position = adapterClickEvents.Position;
                        if (Position >= 0)
                        {
                            var item = mAdapter.GetItem(Position);
                            if (item != null)
                            {
                                Intent IntentOfvideoCall = new Intent(this.Context, typeof(TwilioVideoCallActivity));
                                if (AppSettings.Use_Agora_Library && AppSettings.Use_Twilio_Library == false)
                                {
                                    IntentOfvideoCall = new Intent(this.Context, typeof(AgoraVideoCallActivity));
                                    IntentOfvideoCall.PutExtra("type", "Agora_video_calling_start");
                                }
                                else if (AppSettings.Use_Agora_Library == false && AppSettings.Use_Twilio_Library)
                                {
                                    IntentOfvideoCall = new Intent(this.Context, typeof(TwilioVideoCallActivity));
                                    IntentOfvideoCall.PutExtra("type", "Twilio_video_calling_start");
                                }

                                IntentOfvideoCall.PutExtra("UserID", item.user_id);
                                IntentOfvideoCall.PutExtra("avatar", item.avatar);
                                IntentOfvideoCall.PutExtra("name", item.name);
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
                else if (AppSettings.Enable_Audio_Call && AppSettings.Enable_Video_Call == false) // Audio Call On
                {
                    try
                    {
                        var Position = adapterClickEvents.Position;
                        if (Position >= 0)
                        {
                            var item = mAdapter.GetItem(Position);
                            if (item != null)
                            {
                                Intent IntentOfvideoCall = new Intent(this.Context, typeof(TwilioVideoCallActivity));
                                if (AppSettings.Use_Agora_Library && AppSettings.Use_Twilio_Library == false)
                                {
                                    IntentOfvideoCall = new Intent(this.Context, typeof(AgoraAudioCallActivity));
                                    IntentOfvideoCall.PutExtra("type", "Agora_audio_calling_start");
                                }
                                else if (AppSettings.Use_Agora_Library == false && AppSettings.Use_Twilio_Library)
                                {
                                    IntentOfvideoCall = new Intent(this.Context, typeof(TwilioAudioCallActivity));
                                    IntentOfvideoCall.PutExtra("type", "Twilio_audio_calling_start");
                                }

                                IntentOfvideoCall.PutExtra("UserID", item.user_id);
                                IntentOfvideoCall.PutExtra("avatar", item.avatar);
                                IntentOfvideoCall.PutExtra("name", item.name);
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