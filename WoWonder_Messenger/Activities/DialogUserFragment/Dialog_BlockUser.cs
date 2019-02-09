using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using WoWonder.Activities.DefaultUser;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Activities.DialogUserFragment {
    public class Dialog_BlockUser : DialogFragment {
        public Dialog_BlockUser (string userid, GetBlockedUsersObject.BlockedUsers item) {
            try {
                _Userid = userid;
                _Item = item;
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            try {
                base.OnCreateView (inflater, container, savedInstanceState);

                // Set our view from the "Dialog_BlockUser_Fragment" layout resource
                var view = inflater.Inflate (Resource.Layout.Dialog_BlockUser_Fragment, container, false);

                // Get values
                Txt_Username = view.FindViewById<TextView> (Resource.Id.Txt_Username);
                Txt_Name = view.FindViewById<TextView> (Resource.Id.Txt_SecendreName);

                Btn_UnBlockUser = view.FindViewById<Button> (Resource.Id.UnBlockUser_Button);

                Image_Userprofile = view.FindViewById<ImageView> (Resource.Id.profileAvatar_image);

                ImageCacheLoader.LoadImage(_Item.avatar, Image_Userprofile, false, true);

                Txt_Username.Text = _Item.name;
                Txt_Name.Text = "@" + _Item.username;

                return view;
            } catch (Exception e) {
                Console.WriteLine (e);
                return null;
            }
        }

        public override void OnResume () {
            try {
                base.OnResume ();

                //Add Event
                Btn_UnBlockUser.Click += BtnUnBlockUserOnClick;
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }

        public override void OnPause () {
            try {
                base.OnPause ();

                // Event
                Btn_UnBlockUser.Click -= BtnUnBlockUserOnClick;
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }

        public override void OnActivityCreated (Bundle savedInstanceState) {
            Dialog.Window.RequestFeature (WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated (savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        private void BtnUnBlockUserOnClick (object sender, EventArgs eventArgs) {
            try {
                if (IMethods.CheckConnectivity ()) {
                    var local = BlockedUsers_Activity.mAdapter.mBlockedUsers.FirstOrDefault (a =>
                        a.user_id == _Userid);
                    if (local != null) BlockedUsers_Activity.mAdapter.Remove (local);

                    Toast.MakeText (Application.Context, GetString (Resource.String.Lbl_Unblock_successfully),
                        ToastLength.Short).Show ();

                    var data = Global.Block_User (_Userid, false).ConfigureAwait (false); //false >> "un-block"
                } else {
                    Toast.MakeText (Context, GetString (Resource.String.Lbl_Error_check_internet_connection),
                        ToastLength.Short).Show ();
                }

                Dismiss ();
                var x = Resource.Animation.slide_right;
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }

        public override void OnTrimMemory (TrimMemory level) {
            try {
                ImageService.Instance.InvalidateMemoryCache ();
                GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory (level);
            } catch (Exception exception) {
                Console.WriteLine (exception);
            }
        }

        public override void OnLowMemory () {
            try {
                GC.Collect (GC.MaxGeneration);
                base.OnLowMemory ();
            } catch (Exception exception) {
                Console.WriteLine (exception);
            }
        }

        public class OnBlockUserUp_EventArgs : EventArgs {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView Txt_Username;
        private TextView Txt_Name;

        private Button Btn_UnBlockUser;

        private ImageView Image_Userprofile;

        public event EventHandler<OnBlockUserUp_EventArgs> _OnBlockUserUpComplete;

        public static string _Userid = "";
        public GetBlockedUsersObject.BlockedUsers _Item;

        #endregion
    }
}