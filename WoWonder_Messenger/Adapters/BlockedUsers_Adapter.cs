using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;

using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.User;

namespace WoWonder.Adapters
{
    public class BlockedUsers_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<BlockedUsers_AdapterClickEventArgs> ItemClick;
        public event EventHandler<BlockedUsers_AdapterClickEventArgs> ItemLongClick;

        public ObservableCollection<GetBlockedUsersObject.BlockedUsers> mBlockedUsers =
            new ObservableCollection<GetBlockedUsersObject.BlockedUsers>();

        private Context Activity_Context;

        public BlockedUsers_Adapter(Context activity)
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
            try
            {
                //Setup your layout here >> BlockedUsers_view
                View itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.BlockedUsers_view, parent, false);
                var vh = new BlockedUsers_AdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is BlockedUsers_AdapterViewHolder holder)
                {
                    var item = mBlockedUsers[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirection_RightToLeft)
                        {
                            holder.RelativeLayout_main.LayoutDirection = LayoutDirection.Rtl;
                            holder.Txt_Username.TextDirection = TextDirection.Rtl;
                            holder.Txt_Lastseen.TextDirection = TextDirection.Rtl;
                        }

                        Initialize(holder, item);
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Initialize(BlockedUsers_AdapterViewHolder holder, GetBlockedUsersObject.BlockedUsers users)
        {
            try
            {
                ImageCacheLoader.LoadImage(users.avatar, holder.ImageAvatar, false, true);
              

                string name = IMethods.Fun_String.DecodeString(users.name);
                holder.Txt_Username.Text = name;

                string lastSeen = Activity_Context.GetText(Resource.String.Lbl_Last_seen);
                var time = IMethods.ITime.TimeAgo(int.Parse(users.lastseen_unix_time));
                holder.Txt_Lastseen.Text = lastSeen + " " + time; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void BindEnd()
        {
            try
            {
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Function Users
        public void Add(GetBlockedUsersObject.BlockedUsers user)
        {
            try
            {
                var check = mBlockedUsers.FirstOrDefault(a => a.user_id == user.user_id);
                if (check == null)
                {
                    mBlockedUsers.Add(user);
                    NotifyItemInserted(mBlockedUsers.IndexOf(mBlockedUsers.Last()));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Insert(GetBlockedUsersObject.BlockedUsers users)
        {
            try
            {
                mBlockedUsers.Insert(0, users);
                NotifyItemInserted(mBlockedUsers.IndexOf(mBlockedUsers.First()));
                NotifyItemRangeInserted(0, mBlockedUsers.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Move(GetBlockedUsersObject.BlockedUsers users)
        {
            try
            {
                var data = mBlockedUsers.FirstOrDefault(a => a.user_id == users.user_id);
                var index = mBlockedUsers.IndexOf(data);
                if (index > -1)
                {
                    mBlockedUsers.Move(index, 0);
                    NotifyItemMoved(index, 0);
                    NotifyItemChanged(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Clear()
        {
            try
            {
                mBlockedUsers.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Remove(GetBlockedUsersObject.BlockedUsers users)
        {
            try
            {
                var Index = mBlockedUsers.IndexOf(
                    mBlockedUsers.FirstOrDefault(a => a.user_id == users.user_id));
                if (Index != -1)
                {
                    mBlockedUsers.Remove(users);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public GetBlockedUsersObject.BlockedUsers GetItem(int position)
        {
            return mBlockedUsers[position];
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

        public override int ItemCount
        {
            get
            {
                try
                {
                    if (mBlockedUsers == null || mBlockedUsers.Count <= 0)
                        return 0;
                    return mBlockedUsers.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        void OnClick(BlockedUsers_AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(BlockedUsers_AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class BlockedUsers_AdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout RelativeLayout_main { get; set; }
        public TextView Txt_Username { get; private set; }
        public TextView Txt_Lastseen { get; private set; }
        public ImageView ImageAvatar { get; private set; }

        #endregion

        public BlockedUsers_AdapterViewHolder(View itemView, Action<BlockedUsers_AdapterClickEventArgs> clickListener,Action<BlockedUsers_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout) MainView.FindViewById(Resource.Id.main);

                Txt_Username = (TextView) MainView.FindViewById(Resource.Id.Txt_Username);
                Txt_Lastseen = (TextView) MainView.FindViewById(Resource.Id.Txt_LastSeen);
                ImageAvatar = (ImageView) MainView.FindViewById(Resource.Id.Image_Avatar);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new BlockedUsers_AdapterClickEventArgs{View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new BlockedUsers_AdapterClickEventArgs{View = itemView, Position = AdapterPosition});

                FontController.SetFont(Txt_Username, 1);
                FontController.SetFont(Txt_Lastseen, 3);

                ImageCacheLoader.LoadImage("no_profile_image.png", ImageAvatar, false, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class BlockedUsers_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}