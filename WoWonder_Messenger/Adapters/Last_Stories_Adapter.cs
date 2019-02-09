using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Refractored.Controls;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Story;
using Color = Android.Graphics.Color;
using Console = System.Console;


namespace WoWonder.Adapters
{
    public class StoriesViewHolder : RecyclerView.ViewHolder
    {
        #region  Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout RelativeLayout_main { get; private set; }
        public TextView Txt_Username { get; private set; }
        public TextView Txt_description { get; private set; }
        public ImageView ProfileImageViewAsync { get; private set; }
        public CircleImageView Profile_indicator { get; private set; }

        #endregion

        //View itemView, string type, Action<int> listener
        public StoriesViewHolder(View itemView, Action<AdapterClickEvents> listener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout) MainView.FindViewById(Resource.Id.main);
                Txt_Username = (TextView) MainView.FindViewById(Resource.Id.Txt_Username);
                Txt_description = (TextView) MainView.FindViewById(Resource.Id.Txt_description);

                Profile_indicator = itemView.FindViewById<CircleImageView>(Resource.Id.profile_indicator);
                ProfileImageViewAsync = itemView.FindViewById<ImageView>(Resource.Id.userProfileImage);

                //Create an Event
                itemView.Click += (sender, e) => listener(new AdapterClickEvents
                {
                    View = itemView,
                    Position = AdapterPosition
                });

                //Dont Remove this code #####
                FontController.SetFont(Txt_Username, 1);
                FontController.SetFont(Txt_description, 3);
                //#####

                ImageCacheLoader.LoadImage("no_profile_image.png", ProfileImageViewAsync, false, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Error");
            }
        }
    }

    public class Last_Stories_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<AdapterClickEvents> OnItemClick;
        public Context Activity_Context;

        public ObservableCollection<GetStoriesObject.Story> mStoryList =
            new ObservableCollection<GetStoriesObject.Story>();

        public Last_Stories_Adapter(Context context)
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

        //Setup your layout here //  First RUN
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Last_Stories_view, parent, false);
                var vh = new StoriesViewHolder(row,OnClick);
                return vh;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        // Replace the contents of the view with that element
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position) //Add data and Event
        {
            try
            {
                if (viewHolder is StoriesViewHolder holder)
                {
                    var item = mStoryList[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirection_RightToLeft)
                        {
                            holder.RelativeLayout_main.LayoutDirection = LayoutDirection.Rtl;
                            holder.Txt_Username.TextDirection = TextDirection.Rtl;
                            holder.Txt_description.TextDirection = TextDirection.Rtl;
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

        public void Initialize(StoriesViewHolder holder, GetStoriesObject.Story item)
        {
            try
            {
                //profile_picture
                ImageCacheLoader.LoadImage(item.user_data.avatar, holder.ProfileImageViewAsync, false, true);

              

                string name = IMethods.Fun_String.DecodeString(item.user_data.name);
                holder.Txt_Username.Text = name;

                if (!string.IsNullOrEmpty(item.description))
                {
                    holder.Txt_description.Text = item.description;
                }
                else if (!string.IsNullOrEmpty(item.title))
                {
                    holder.Txt_description.Text = item.title;
                }
                else
                {
                    holder.Txt_description.Text = this.Activity_Context.GetText(Resource.String.Lbl_Empty);
                }

                if (item.Profile_indicator == null)
                    item.Profile_indicator = AppSettings.Story_Default_Color;

                holder.Profile_indicator.BorderColor = Color.ParseColor(item.Profile_indicator); // Default_Color 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private void OnClick(AdapterClickEvents args)
        {
            OnItemClick?.Invoke(this, args);
        }

        public override int ItemCount
        {
            get
            {
                try
                {
                    if (mStoryList == null || mStoryList.Count <= 0)
                        return 0;
                    return mStoryList.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        public GetStoriesObject.Story GetItem(int position)
        {
            return mStoryList[position];
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

        // Function Video
        public void Add(GetStoriesObject.Story Story)
        {
            try
            {
                var check = mStoryList.FirstOrDefault(a => a.user_id == Story.user_id);
                if (check == null)
                {
                    mStoryList.Add(Story);
                    NotifyItemInserted(mStoryList.IndexOf(mStoryList.Last()));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Remove(GetStoriesObject.Story Story)
        {
            try
            {
                var Index = mStoryList.IndexOf(mStoryList.FirstOrDefault(a => a.id == Story.id));
                if (Index != -1)
                {
                    mStoryList.Remove(Story);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
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
                mStoryList.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Update()
        {
            try
            {
                var check = mStoryList.Where(a => a.Profile_indicator == AppSettings.Story_Read_Color).ToList();
                if (check.Count > 0)
                    foreach (var all in check)
                        all.Profile_indicator = AppSettings.Story_Read_Color;

                NotifyDataSetChanged();
                NotifyItemChanged(0); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
    }
}