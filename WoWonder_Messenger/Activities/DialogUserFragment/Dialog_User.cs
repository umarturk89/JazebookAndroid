using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using FFImageLoading;
using FFImageLoading.Views;
using WoWonder.Activities.ChatWindow;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;
using Console = System.Console;


namespace WoWonder.Activities.DialogUserFragment
{
    public class Dialog_User : DialogFragment
    {
        public class OnUserUp_EventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView Txt_Username;
        private TextView Txt_Name;

        private CircleButton Btn_SendMesseges;
        private CircleButton Btn_Add;

        private ImageView Image_Userprofile;

        public event EventHandler<OnUserUp_EventArgs> _OnUserUpComplete;

        public static string _Userid = "";
        public GetSearchObject.User _Item;

        #endregion

        public Dialog_User(string userid, GetSearchObject.User item)
        {
            try
            {
                _Userid = userid;
                _Item = item;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //open Layout as a message
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                // Set our view from the "Dialog_User_Fragment" layout resource
                var view = inflater.Inflate(Resource.Layout.Dialog_User_Fragment, container, false);

                // Get values
                Txt_Username = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                Txt_Name = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                Btn_SendMesseges = view.FindViewById<CircleButton>(Resource.Id.SendMesseges_button);

                Btn_Add = view.FindViewById<CircleButton>(Resource.Id.Add_button);
                Btn_Add.Tag = "Add";

                Image_Userprofile = view.FindViewById<ImageView>(Resource.Id.profileAvatar_image);

                //profile_picture
                ImageCacheLoader.LoadImage(_Item.Avatar, Image_Userprofile, false, true);

               
                Txt_Username.Text = _Item.Name;
                Txt_Name.Text = "@" + _Item.Username;

                if (_Item.is_following == "1") // My Friend
                {
                    Btn_Add.Visibility = ViewStates.Visible;
                    Btn_Add.SetColor(Color.ParseColor("#efefef"));
                    Btn_Add.SetImageResource(Resource.Drawable.ic_tick);
                    Btn_Add.Drawable.SetTint(Color.ParseColor("#444444"));
                    Btn_Add.Tag = "friends";
                }
                else if (_Item.is_following == "2") // Request
                {
                    Btn_Add.SetColor(Color.ParseColor("#efefef"));
                    Btn_Add.SetImageResource(Resource.Drawable.ic_tick);
                    Btn_Add.Drawable.SetTint(Color.ParseColor("#444444"));
                    Btn_Add.Tag = "Request";
                }
                else if (_Item.is_following == "0") //Not Friend
                {
                    Btn_Add.Visibility = ViewStates.Visible;

                    Btn_Add.SetColor(Color.ParseColor("#444444"));
                    Btn_Add.SetImageResource(Resource.Drawable.ic_add);
                    Btn_Add.Drawable.SetTint(Color.ParseColor("#ffffff"));
                    Btn_Add.Tag = "Add";
                }

                // Event
                Btn_SendMesseges.Click += BtnSendMessegesOnClick;
                Btn_Add.Click += BtnAddOnClick;

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //animations
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        private void BtnSendMessegesOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                this.Dismiss();
                int x = Resource.Animation.slide_right;

                Intent intent = new Intent(this.Context, typeof(ChatWindow_Activity));
                intent.PutExtra("UserID", _Userid);
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnAddOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (Btn_Add.Tag.ToString() == "Add") //(is_following == "0") >> Not Friend
                    {
                        Btn_Add.SetColor(Color.ParseColor("#efefef"));
                        Btn_Add.SetImageResource(Resource.Drawable.ic_tick);
                        Btn_Add.Drawable.SetTint(Color.ParseColor("#444444"));
                        Btn_Add.Tag = "friends";

                        _Item.is_following = "1";
                    }
                    else if (Btn_Add.Tag.ToString() == "request") //(is_following == "2") >> Request
                    {
                        Btn_Add.SetColor(Color.ParseColor("#efefef"));
                        Btn_Add.SetImageResource(Resource.Drawable.ic_tick);
                        Btn_Add.Drawable.SetTint(Color.ParseColor("#444444"));
                        Btn_Add.Tag = "Add";

                        _Item.is_following = "2";
                    }
                    else //(is_following == "1") >> Friend
                    {
                        Btn_Add.SetColor(Color.ParseColor("#444444"));
                        Btn_Add.SetImageResource(Resource.Drawable.ic_add);
                        Btn_Add.Drawable.SetTint(Color.ParseColor("#ffffff"));

                        Btn_Add.Tag = "Add";

                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.Delete_UsersContact(_Userid);
                        dbDatabase.Dispose();

                        _Item.is_following = "0";
                    }

                    
                 
                    var response = Global.Follow_User(_Userid).ConfigureAwait(false);

                    
                    if (_Item.is_following == "1")
                    {
                        if (AppSettings.ConnectivitySystem == "1")
                        {
                            Toast.MakeText(Application.Context,
                                this.GetText(Resource.String.Lbl_Sent_successfully_followed), ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(Application.Context,
                                    this.GetText(Resource.String.Lbl_Sent_successfully_FriendRequest),
                                    ToastLength.Short)
                                .Show();
                        }
                    }
                    else
                    {
                        if (AppSettings.ConnectivitySystem == "1")
                        {
                            Toast.MakeText(Application.Context,
                                    this.GetText(Resource.String.Lbl_Sent_successfully_Unfollowed), ToastLength.Short)
                                .Show();
                        }
                        else
                        {
                            Toast.MakeText(Application.Context,
                                this.GetText(Resource.String.Lbl_Sent_successfully_FriendRequestCancelled),
                                ToastLength.Short).Show();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(Application.Context,
                        this.GetText(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
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
    }
}