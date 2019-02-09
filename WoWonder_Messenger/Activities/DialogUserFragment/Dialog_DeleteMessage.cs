using System;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Activities.DialogUserFragment
{
    public class Dialog_DeleteMessage : DialogFragment
    {
        public class OnDeleteMessageUp_EventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView Txt_Username;
        private TextView Txt_Name;

        private Button Btn_DeleteMessage;

        private ImageView Image_Userprofile;

        public event EventHandler<OnDeleteMessageUp_EventArgs> _OnDeleteMessageUpComplete;

        public static string _Userid = "";
        public Classes.Get_Users_List_Object.User _Item;

        #endregion

        public Dialog_DeleteMessage(string userid, Classes.Get_Users_List_Object.User item)
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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                // Set our view from the "Dialog_DeleteMessege_Fragment" layout resource
                var view = inflater.Inflate(Resource.Layout.Dialog_DeleteMessege_Fragment, container, false);

                // Get values
                Txt_Username = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                Txt_Name = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                Btn_DeleteMessage = view.FindViewById<Button>(Resource.Id.DeleteMessage_Button);

                Image_Userprofile = view.FindViewById<ImageView>(Resource.Id.profileAvatar_image);

                //profile_picture
                ImageCacheLoader.LoadImage(_Item.ProfilePicture, Image_Userprofile, false, true);


                Txt_Username.Text = this.GetText(Resource.String.Lbl_Doyouwanttodeletechatwith);
                Txt_Name.Text = "@" + _Item.Username;

                // Event
                Btn_DeleteMessage.Click += BtnDeleteMessageOnClick;

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        private void BtnDeleteMessageOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                var local = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a =>
                    a.UserId == _Userid);
                if (local != null)
                {
                    Last_Messages_Fragment.mAdapter.Remove(local);
                   

                    Task.Run(() =>
                    {
                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.Delete_LastUsersChat(_Userid);
                        dbDatabase.DeleteAllMessagesUser(UserDetails.User_id, _Userid);
                        dbDatabase.Dispose();
                    });
                }

                if (IMethods.CheckConnectivity())
                {
                    var data = Global.Delete_Conversation(_Userid).ConfigureAwait(false);
                }

                Last_Messages_Fragment.TimerWork = "Working";
                this.Dismiss();
                int x = Resource.Animation.slide_right;
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
    }
}