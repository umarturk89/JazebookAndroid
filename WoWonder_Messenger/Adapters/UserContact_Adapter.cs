using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Java.Lang;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder.SQLite;
using WoWonder_API.Requests;
using static WoWonder_API.Requests.RequestsAsync;
using Console = System.Console;
using Exception = System.Exception;


namespace WoWonder.Adapters
{
    public class UserContact_Adapter : RecyclerView.Adapter, IFilterable
    {
        public static RecyclerView Recylercontrol;
        public readonly JavaList<Classes.UserContacts.User> CurrentList;
        public Context Activity_Context;
        public JavaList<Classes.UserContacts.User> mMyContactsList = new JavaList<Classes.UserContacts.User>();

        public UserContact_Adapter(Context activity, JavaList<Classes.UserContacts.User> MyContactsList,RecyclerView _Recylercontrol)
        {
            try
            {
                CurrentList = MyContactsList;
                mMyContactsList = MyContactsList;

                Activity_Context = activity;
                Recylercontrol = _Recylercontrol;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public class UserContact_AdapterViewHolder : RecyclerView.ViewHolder
        {
            public View MainView { get; }

            public ImageView Image { get; set; }
            public TextView Name { get; set; }
            public TextView About { get; set; }
            public Button Button { get; set; }

            public UserContact_AdapterViewHolder(View itemView, Action<AdapterClickEvents> listener) : base(itemView)
            {
                try
                {
                    MainView = itemView;

                    Image = MainView.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                    Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                    About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                    Button = MainView.FindViewById<Button>(Resource.Id.cont);

                    itemView.Click += (sender, e) => listener(new AdapterClickEvents
                    {
                        View = itemView,
                        Position = AdapterPosition
                    });

                   
                    //Dont Remove this code #####
                    FontController.SetFont(Name, 1);
                    FontController.SetFont(About, 3);
                    FontController.SetFont(Button, 1);
                    //#####

                    ImageCacheLoader.LoadImage("no_profile_image.png", Image, false, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here // First RUN

            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_HContact_view, parent, false);
            var vh = new UserContact_AdapterViewHolder(row, OnClick);
            return vh;
        }

        public override int ItemCount
        {

            get
            {
                try
                {
                    if (mMyContactsList == null || mMyContactsList.Count <= 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return mMyContactsList.Count;
                    }
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
                if (viewHolder is UserContact_AdapterViewHolder holder)
                {
                    var item = mMyContactsList[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirection_RightToLeft)
                        {
                            holder.About.LayoutDirection = LayoutDirection.Rtl;
                            holder.Name.TextDirection = TextDirection.Rtl;
                            holder.Image.TextDirection = TextDirection.Rtl;
                        }

                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
         
        public Filter Filter => FilterHelper.NewInstance(CurrentList, this);

        public event EventHandler<AdapterClickEvents> ItemClick;
        public event EventHandler<AdapterClickEvents> ItemLongClick;

        public void Initialize(UserContact_AdapterViewHolder holder, Classes.UserContacts.User users)
        {
            try
            {
                if (users.St_ChatColor == null)
                    users.St_ChatColor = AppSettings.MainColor;

                ImageCacheLoader.LoadImage(users.Avatar, holder.Image, false, true);

              
                string name = IMethods.Fun_String.DecodeString(users.Name);
                if (holder.Name.Text != name)
                {
                    holder.Name.Text = IMethods.Fun_String.SubStringCutOf(name, 25);
                } 

                var dataAbout = IMethods.Fun_String.StringNullRemover(users.About);
                if (dataAbout != "Empty")
                {
                    var about = IMethods.Fun_String.DecodeString(dataAbout);
                    holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 25);
                }
                else
                {
                    var about = Activity_Context.GetText(Resource.String.Lbl_DefaultAbout) + " " + AppSettings.Application_Name;
                    holder.About.Text = IMethods.Fun_String.SubStringCutOf(about, 25);
                }

                if (string.IsNullOrEmpty(users.IsFollowing.ToString()))
                    users.IsFollowing = 1;

                //Set All My Contacts  
                holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                if (AppSettings.ConnectivitySystem == "1") // Following
                    holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                else // Friend
                    holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Friends);
                holder.Button.Tag = "true";

                if (!holder.Button.HasOnClickListeners)
                    holder.Button.Click += async (sender, args) =>
                    {
                        try
                        {
                            if (!IMethods.CheckConnectivity())
                            {
                                Toast.MakeText(Activity_Context,
                                    Activity_Context.GetString(Resource.String.Lbl_Error_check_internet_connection),
                                    ToastLength.Short).Show();
                            }
                            else
                            {
                                if (holder.Button.Tag.ToString() == "false")
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable
                                        .follow_button_profile_friends_pressed);
                                    holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                                    if (AppSettings.ConnectivitySystem == "1") // Following
                                    {
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Following);
                                        holder.Button.Tag = "true";
                                    }
                                    else // Request Friend 
                                    {
                                        holder.Button.SetBackgroundResource(Resource.Drawable
                                            .follow_button_profile_friends);
                                        holder.Button.SetTextColor(Color.ParseColor("#444444"));
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Request);
                                        holder.Button.Tag = "Request";
                                    }
                                }
                                else
                                {
                                    holder.Button.SetBackgroundResource(Resource.Drawable
                                        .follow_button_profile_friends);
                                    holder.Button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                                    if (AppSettings.ConnectivitySystem == "1") // Following
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_Follow);
                                    else // Friend
                                        holder.Button.Text = Activity_Context.GetText(Resource.String.Lbl_AddFriends);
                                    holder.Button.Tag = "false";

                                    var dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Delete_UsersContact(users.UserId);
                                    dbDatabase.Dispose();
                                }

                                await Task.Run(async () =>
                                {
                                    var (apiStatus, respond) = await Global.Follow_User(users.UserId).ConfigureAwait(false);
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

        // Function  
        public void Add(Classes.UserContacts.User userFollower)
        {
            try
            {
                var check = mMyContactsList.FirstOrDefault(a => a.UserId == userFollower.UserId);
                if (check == null)
                {
                    mMyContactsList.Add(userFollower);
                    NotifyItemInserted(mMyContactsList.IndexOf(mMyContactsList.Last()));
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
                mMyContactsList?.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Classes.UserContacts.User GetItem(int position)
        {
            return mMyContactsList[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }

        public void SetContacts(JavaList<Classes.UserContacts.User> filtereddata)
        {
            mMyContactsList = filtereddata;
        }

        private void OnClick(AdapterClickEvents args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void OnLongClick(AdapterClickEvents args)
        {
            ItemLongClick?.Invoke(this, args);
        }

    }

    public class AdapterClickEvents : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }

    public class FilterHelper : Filter
    {
        private static JavaList<Classes.UserContacts.User> currentList;

        private static UserContact_Adapter AdapterMain;
        private readonly IFilterable _adapter;

        public static FilterHelper NewInstance(JavaList<Classes.UserContacts.User> CurrentList,UserContact_Adapter MAdapter)
        {
            AdapterMain = MAdapter;
            currentList = CurrentList;

            return new FilterHelper();
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            try
            {
                var filterResults = new FilterResults();
                var foundFilters = new JavaList<Classes.UserContacts.User>();

                if (constraint != null && constraint.Length() > 0)
                {
                    for (var i = 0; i < currentList.Size(); i++)
                    {
                        var Data = currentList[i];
                        var query = constraint.ToString().ToUpper();

                        if (Data.Name.ToUpper().Contains(query)) foundFilters.Add(Data);
                    }

                    filterResults.Count = foundFilters.Count;
                    filterResults.Values = foundFilters;

                    return filterResults;
                }

                filterResults.Count = currentList.Count;
                filterResults.Values = currentList;
                return filterResults;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var filterResults = new FilterResults();
                filterResults.Count = currentList.Count;
                filterResults.Values = currentList;
                return filterResults;
            }
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            try
            {
                AdapterMain.SetContacts((JavaList<Classes.UserContacts.User>) results.Values);
                AdapterMain.NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}