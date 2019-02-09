using System;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View.Animation;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading;
using WoWonder.Activities.DefaultUser;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder_API.Requests;
using Fragment = Android.Support.V4.App.Fragment;

namespace WoWonder.Activities.ChatWindow
{
    public class Chat_Colors_Fragment : Fragment
    {
        private CircleButton closebutton;
        private View Chat_Colors_FragmentView;

        private string _UserId;

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                _UserId = Arguments.GetString("userid");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                Chat_Colors_FragmentView = inflater.Inflate(Resource.Layout.Chat_Colors_Fragment, container, false);
                closebutton = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.closebutton);

                closebutton = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.closebutton);
                closebutton.Click += Closebutton_Click;

                var colorbutton1 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton1);
                var colorbutton2 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton2);
                var colorbutton3 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton3);
                var colorbutton4 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton4);
                var colorbutton5 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton5);
                var colorbutton6 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton6);
                var colorbutton7 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton7);
                var colorbutton8 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton8);
                var colorbutton9 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton9);
                var colorbutton10 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton10);
                var colorbutton11 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton11);
                var colorbutton12 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton12);
                var colorbutton13 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton13);
                var colorbutton14 = Chat_Colors_FragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton14);

                colorbutton1.Click += SetColorbutton_Click;
                colorbutton2.Click += SetColorbutton_Click;
                colorbutton3.Click += SetColorbutton_Click;
                colorbutton4.Click += SetColorbutton_Click;
                colorbutton5.Click += SetColorbutton_Click;
                colorbutton6.Click += SetColorbutton_Click;
                colorbutton7.Click += SetColorbutton_Click;
                colorbutton8.Click += SetColorbutton_Click;
                colorbutton9.Click += SetColorbutton_Click;
                colorbutton10.Click += SetColorbutton_Click;
                colorbutton11.Click += SetColorbutton_Click;
                colorbutton12.Click += SetColorbutton_Click;
                colorbutton13.Click += SetColorbutton_Click;
                colorbutton14.Click += SetColorbutton_Click;

                return Chat_Colors_FragmentView;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private void SetColorbutton_Click(object sender, EventArgs e)
        {
            try
            {
                CircleButton btn = (CircleButton) sender;
                string colorCssode = (string) btn.Tag;

                var MainActivityview = ((ChatWindow_Activity) this.Activity);
                MainActivityview.TopChatToolBar.SetBackgroundColor(Color.ParseColor(colorCssode));
                MainActivityview.ChatSendButton.SetColor(Color.ParseColor(colorCssode));
                ChatWindow_Activity.MainChatColor = colorCssode;

                SetTheme(MainActivityview, colorCssode);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    MainActivityview.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    MainActivityview.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    MainActivityview.Window.SetStatusBarColor(Color.ParseColor(colorCssode));
                }

                MainActivityview.Recreate();
                var ColorFragmentHolder = this.Activity.FindViewById<FrameLayout>(Resource.Id.ButtomFragmentHolder);
                var interplator = new FastOutSlowInInterpolator();
                ColorFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(500);
                Activity.SupportFragmentManager.BeginTransaction().Remove(this).Commit();
                ((ChatWindow_Activity)Activity).ChatColorButton.Tag = "Closed";
                ((ChatWindow_Activity)Activity).ChatColorButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#888888")); 

                if (IMethods.CheckConnectivity())
                {
                    WoWonder_API.Requests.RequestsAsync.Message.Change_Chat_Color(_UserId, colorCssode).ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(this.Context, this.GetText(Resource.String.Lbl_Error_check_internet_connection),ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private System.Drawing.Color DarkerColor(Color color, float correctionfactory = 50f)
        {
            const float hundredpercent = 100f;
            return System.Drawing.Color.FromArgb((int)(((float)color.R / hundredpercent) * correctionfactory),(int)(((float)color.G / hundredpercent) * correctionfactory), (int)(((float)color.B / hundredpercent) * correctionfactory));
        }

        public static System.Drawing.Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return System.Drawing.Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        private void Closebutton_Click(object sender, EventArgs e)
        {
            try
            {
                var ColorFragmentHolder = this.Activity.FindViewById<FrameLayout>(Resource.Id.ButtomFragmentHolder);
                var interplator = new FastOutSlowInInterpolator();
                ColorFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(500);
                Activity.SupportFragmentManager.BeginTransaction().Remove(this).Commit();
                ((ChatWindow_Activity)Activity).ChatColorButton.Tag = "Closed";
                ((ChatWindow_Activity)Activity).ChatColorButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#888888"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void SetTheme(Activity activity, string color)
        {
            try
            {
                if (color.Contains("b582af"))
                {
                    activity.SetTheme(Resource.Style.Chatththemeb582af);
                }
                else if (color.Contains("a84849"))
                {
                    activity.SetTheme(Resource.Style.Chatththemea84849);
                }
                else if (color.Contains("f9c270"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef9c270);
                }
                else if (color.Contains("70a0e0"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme70a0e0);
                }
                else if (color.Contains("56c4c5"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme56c4c5);
                }
                else if (color.Contains("f33d4c"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef33d4c);
                }
                else if (color.Contains("a1ce79"))
                {
                    activity.SetTheme(Resource.Style.Chatththemea1ce79);
                }
                else if (color.Contains("a085e2"))
                {
                    activity.SetTheme(Resource.Style.Chatththemea085e2);
                }
                else if (color.Contains("ed9e6a"))
                {
                    activity.SetTheme(Resource.Style.Chatththemeed9e6a);
                }
                else if (color.Contains("2b87ce"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme2b87ce);
                }
                else if (color.Contains("f2812b"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef2812b);
                }
                else if (color.Contains("0ba05d"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme0ba05d);
                }
                else if (color.Contains("0e71ea"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme0e71ea);
                }
                else if (color.Contains("aa2294"))
                {
                    activity.SetTheme(Resource.Style.Chatththemeaa2294);
                }
                else if (color.Contains("f9a722"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef9a722);
                }
                else if (color.Contains("008484"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme008484);
                }
                else if (color.Contains("5462a5"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme5462a5);
                }
                else if (color.Contains("fc9cde"))
                {
                    activity.SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("fc9cde"))
                {
                    activity.SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("51bcbc"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme51bcbc);
                }
                else if (color.Contains("c9605e"))
                {
                    activity.SetTheme(Resource.Style.Chatththemec9605e);
                }
                else if (color.Contains("01a5a5"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme01a5a5);
                }
                else if (color.Contains("056bba"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme056bba);
                }
                else
                {
                    //Default Color >> AppSettings.MainColor
                    activity.SetTheme(Resource.Style.Chatththemedefault);
                }

                var dataUser = Last_Messages_Fragment.mAdapter.MLastMessagesUser?.FirstOrDefault(a => a.UserId == _UserId);
                if (dataUser != null)
                {        
                    dataUser.ChatColor = color;
                }
                else
                {
                    var dataUserContacts =UserContacts_Activity.ContactAdapter.mMyContactsList?.FirstOrDefault(a => a.UserId == _UserId);
                    if (dataUserContacts != null)
                    {
                        dataUserContacts.St_ChatColor = color;
                    }
                    else
                    {
                        var dataSearchUserList =OnlineSearch_Activity.mAdapter?.mSearchUserList?.FirstOrDefault(a => a.UserId == _UserId);
                        if (dataSearchUserList != null)
                        {
                            dataSearchUserList.ChatColor = color; 
                        }
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