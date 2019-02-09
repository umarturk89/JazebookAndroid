using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;

using WoWonder.Helpers;
using WoWonder_API.Classes.User;
using IMethods = WoWonder.Functions.IMethods;

namespace WoWonder.Adapters
{
    public class OnlineSearch_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<OnlineSearch_AdapterClickEventArgs> ItemClick;
        public event EventHandler<OnlineSearch_AdapterClickEventArgs> ItemLongClick;
        public event EventHandler<OnlineSearch_AdapterClickEventArgs> ImageClick;

        public ObservableCollection<GetSearchObject.User> mSearchUser =
            new ObservableCollection<GetSearchObject.User>();

        private Context Activity_Context;

        public OnlineSearch_Adapter(Context context)
        {
            try
            {
                Activity_Context = context;
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
                //Setup your layout here >> OnlineSearch_view
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.OnlineSearch_view, parent, false);
                var vh = new OnlineSearch_AdapterViewHolder(itemView, OnClick, OnLongClick, ImageOnClick);
                return vh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override int ItemCount
        {
            get
            {
                try
                {
                    if (mSearchUser == null || mSearchUser.Count <= 0)
                        return 0;
                    return mSearchUser.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position) //Add data and Event
        {
            try
            {
                // Replace the contents of the view with that element
                if (viewHolder is OnlineSearch_AdapterViewHolder holder)
                {
                    var item = mSearchUser[position];
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

        public void Initialize(OnlineSearch_AdapterViewHolder holder, GetSearchObject.User item)
        {
            try
            {
                ImageCacheLoader.LoadImage(item.Avatar, holder.ImageAvatar, false, true);
              

                string name = IMethods.Fun_String.DecodeString(item.Name);
                holder.Txt_Username.Text = IMethods.Fun_String.SubStringCutOf(name, 14);

                string lastSeen = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " ";
                var time = IMethods.ITime.TimeAgo(int.Parse(item.LastseenUnixTime));
                holder.Txt_Lastseen.Text = lastSeen + time; 
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
        public void Add(GetSearchObject.User user)
        {
            try
            {
                var check = mSearchUser.FirstOrDefault(a => a.UserId == user.UserId);
                if (check == null)
                {
                    mSearchUser.Add(user);
                    NotifyItemInserted(mSearchUser.IndexOf(mSearchUser.Last()));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Clear()
        {
            try
            {
                mSearchUser.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public GetSearchObject.User GetItem(int position)
        {
            return mSearchUser[position];
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

        void OnClick(OnlineSearch_AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(OnlineSearch_AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void ImageOnClick(OnlineSearch_AdapterClickEventArgs args) => ImageClick?.Invoke(this, args);

    }

    public class OnlineSearch_AdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout RelativeLayout_main { get; private set; }
        public TextView Txt_Username { get; private set; }
        public TextView Txt_Lastseen { get; private set; }
        public ImageView ImageAvatar { get; private set; }

        #endregion

        public OnlineSearch_AdapterViewHolder(View itemView, Action<OnlineSearch_AdapterClickEventArgs> clickListener,
            Action<OnlineSearch_AdapterClickEventArgs> longClickListener,
            Action<OnlineSearch_AdapterClickEventArgs> ImageClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout)MainView.FindViewById(Resource.Id.main);
                Txt_Username = (TextView)MainView.FindViewById(Resource.Id.Txt_Username);
                Txt_Lastseen = (TextView)MainView.FindViewById(Resource.Id.Txt_LastSeen);
                ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.Image_Avatar);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new OnlineSearch_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new OnlineSearch_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                ImageAvatar.Click += (sender, e) => ImageClickListener(new OnlineSearch_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                //Dont Remove this code #####
                FontController.SetFont(Txt_Username, 1);
                FontController.SetFont(Txt_Lastseen, 3);
                //#####

                ImageCacheLoader.LoadImage("no_profile_image.png", ImageAvatar, false, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class OnlineSearch_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}