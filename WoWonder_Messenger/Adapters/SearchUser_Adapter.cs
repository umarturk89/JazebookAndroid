using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;

using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Classes.User;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Adapters
{
    public class SearchUser_Adapter : RecyclerView.Adapter
    {
        public Context Activity_Context;

        public ObservableCollection<GetSearchObject.User> mSearchUserList =
            new ObservableCollection<GetSearchObject.User>();

        public SearchUser_Adapter(Context context)
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

        public override int ItemCount
        {
            get
            {
                try
                {
                    if (mSearchUserList == null || mSearchUserList.Count <= 0)
                        return 0;
                    return mSearchUserList.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        public event EventHandler<SearchUser_AdapterClickEventArgs> ItemClick;
        public event EventHandler<SearchUser_AdapterClickEventArgs> ItemLongClick;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_HContact_view, parent, false);
                var vh = new SearchUser_AdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is SearchUser_AdapterViewHolder holder)
                {
                    var item = mSearchUserList[position];
                    if (item != null)
                    {
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Initialize(SearchUser_AdapterViewHolder holder, GetSearchObject.User item)
        {
            try
            {
                ImageCacheLoader.LoadImage(item.Avatar, holder.Image, false, true);
             

                string name = IMethods.Fun_String.DecodeString(item.Name);
                holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 14);

                string lastSeen = Activity_Context.GetText(Resource.String.Lbl_Last_seen) + " ";
                var time = IMethods.ITime.TimeAgo(int.Parse(item.LastseenUnixTime));
                holder.About.Text = lastSeen + time;

                if (item.is_following == "1") // My Friend
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                    if (AppSettings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);

                    holder.Button.Tag = "true";
                }
                else if (item.is_following == "2") // Request
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    holder.Button.SetTextColor(Color.ParseColor("#444444"));
                    holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Request);
                    holder.Button.Tag = "Request";
                }
                else if (item.is_following == "0") //Not Friend
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    holder.Button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                    if (AppSettings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                    holder.Button.Tag = "false";

                    var dbDatabase = new SqLiteDatabase();
                    dbDatabase.Delete_UsersContact(item.UserId);
                    dbDatabase.Dispose();
                }
                else
                {
                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                    if (AppSettings.ConnectivitySystem == "1") // Following
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                    else // Friend
                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);
                }

                if (!holder.Button.HasOnClickListeners)
                    holder.Button.Click += async (sender, args) => 
                    {
                        try
                        {
                            if (!IMethods.CheckConnectivity())
                            {
                                Toast.MakeText(Activity_Context,Activity_Context.GetString(Resource.String.Lbl_Error_check_internet_connection),ToastLength.Short).Show();
                            }
                            else
                            {
                                if (holder.Button.Tag.ToString() == "false")
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                                    if (AppSettings.ConnectivitySystem == "1") // Following
                                    {
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                                        holder.Button.Tag = "true";
                                    }
                                    else // Request Friend 
                                    {
                                        holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                        holder.Button.SetTextColor(Color.ParseColor("#444444"));
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Request);
                                        holder.Button.Tag = "Request";
                                    }
                                }
                                else
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                    holder.Button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                                    if (AppSettings.ConnectivitySystem == "1") // Following
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                                    else // Friend
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                                    holder.Button.Tag = "false";

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Delete_UsersContact(item.UserId);
                                    dbDatabase.Dispose();
                                }

                                await Task.Run(async () =>
                                {
                                    var (apiStatus, respond) = await Global.Follow_User(item.UserId).ConfigureAwait(false);
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
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
                var check = mSearchUserList.FirstOrDefault(a => a.UserId == user.UserId);
                if (check == null)
                {
                    mSearchUserList.Add(user);
                    NotifyItemInserted(mSearchUserList.IndexOf(mSearchUserList.Last()));
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
                mSearchUserList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public GetSearchObject.User GetItem(int position)
        {
            return mSearchUserList[position];
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


        private void OnClick(SearchUser_AdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(SearchUser_AdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }
    }

    public class SearchUser_AdapterViewHolder : RecyclerView.ViewHolder
    {
        public SearchUser_AdapterViewHolder(View itemView, Action<SearchUser_AdapterClickEventArgs> clickListener,
            Action<SearchUser_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image = MainView.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Button = MainView.FindViewById<Button>(Resource.Id.cont);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new SearchUser_AdapterClickEventArgs{View = itemView, Position = AdapterPosition});
                itemView.LongClick += (sender, e) => longClickListener(new SearchUser_AdapterClickEventArgs{View = itemView, Position = AdapterPosition});

                //Dont Remove this code #####
                FontController.SetFont(Name, 1);
                FontController.SetFont(About, 3);
                FontController.SetFont(Button, 1);
                //#####

                ImageCacheLoader.LoadImage("no_profile_image.png", Image, false, true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public event EventHandler<int> ItemClick;
        public ImageView Image { get; set; }

        public TextView Name { get; set; }
        public TextView About { get; set; }
        public Button Button { get; set; }

        #endregion
    }

    public class SearchUser_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}