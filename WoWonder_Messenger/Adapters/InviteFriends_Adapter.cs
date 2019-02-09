using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using WoWonder.Activities.DefaultUser;
using WoWonder.Functions;
using WoWonder.Helpers;


namespace WoWonder.Adapters
{
    public class InviteFriends_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<InviteFriends_AdapterClickEventArgs> ItemClick;
        public event EventHandler<InviteFriends_AdapterClickEventArgs> ItemLongClick;

        public ObservableCollection<IMethods.PhoneContactManager.UserContact> mUsersPhoneContacts =
            new ObservableCollection<IMethods.PhoneContactManager.UserContact>();

        private Context Activity_Context;

        public InviteFriends_Adapter(Context activity)
        {
            try
            {
                Activity_Context = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here >> Invite_Friends_view
            View itemView = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.Invite_Friends_view, parent, false);

            var vh = new InviteFriends_AdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override int ItemCount
        {
            get
            {
                try
                {
                    if (mUsersPhoneContacts == null || mUsersPhoneContacts.Count <= 0)
                        return 0;
                    return mUsersPhoneContacts.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is InviteFriends_AdapterViewHolder holder)
                {
                    var item = mUsersPhoneContacts[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirection_RightToLeft)
                        {
                            holder.RelativeLayout_main.LayoutDirection = LayoutDirection.Rtl;
                            holder.Txt_name.TextDirection = TextDirection.Rtl;
                            holder.Txt_number.TextDirection = TextDirection.Rtl;
                        }

                        string name = IMethods.Fun_String.DecodeString(item.UserDisplayName);
                        holder.Txt_name.Text = name;
                        holder.Txt_number.Text = item.PhoneNumber;

                        ImageCacheLoader.LoadImage("no_profile_image.png", holder.ImageAvatar, false, true);

          
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public IMethods.PhoneContactManager.UserContact GetItem(int position)
        {
            return mUsersPhoneContacts[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        void OnClick(InviteFriends_AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(InviteFriends_AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class InviteFriends_AdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        private BlockedUsers_Activity Activity_Context;

        public event EventHandler<int> ItemClick;
        public event EventHandler<int> ImageClick;

        public RelativeLayout RelativeLayout_main { get; private set; }
        public TextView Txt_name { get; private set; }
        public TextView Txt_number { get; private set; }
        public ImageView ImageAvatar { get; private set; }

        #endregion

        public InviteFriends_AdapterViewHolder(View itemView, Action<InviteFriends_AdapterClickEventArgs> clickListener,
            Action<InviteFriends_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout) MainView.FindViewById(Resource.Id.main);
                Txt_name = (TextView) MainView.FindViewById(Resource.Id.name_Text);
                Txt_number = (TextView) MainView.FindViewById(Resource.Id.numberphone_Text);
                ImageAvatar = (ImageView) MainView.FindViewById(Resource.Id.Image_Avatar);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new InviteFriends_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new InviteFriends_AdapterClickEventArgs
                    {View = itemView, Position = AdapterPosition});

                //Dont Remove this code #####
                FontController.SetFont(Txt_name, 1);
                FontController.SetFont(Txt_number, 3);
                //#####

                ImageCacheLoader.LoadImage("no_profile_image.png", ImageAvatar, false, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class InviteFriends_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}