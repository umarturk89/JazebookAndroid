using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Controls;

using WoWonder.Functions;
using WoWonder.Helpers;
using Console = System.Console;

namespace WoWonder.Adapters
{
    public class AddNewCall_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<AddNewCall_AdapterClickEventArgs> ItemClick;
        public event EventHandler<AddNewCall_AdapterClickEventArgs> ItemLongClick;
        public event EventHandler<AddNewCall_AdapterClickEventArgs> AudioCallClick;
        public event EventHandler<AddNewCall_AdapterClickEventArgs> VideoCallClick;
        public Context Activity_Context;

        public ObservableCollection<Classes.UserContacts.User> mCallUserContacts = new ObservableCollection<Classes.UserContacts.User>();
        private List<string> listOnline = new List<string>();

        public AddNewCall_Adapter(Context context)
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
                //Setup your layout here >> AddNewCall_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AddNewCall_view, parent, false);
                var holder = new AddNewCall_AdapterViewHolder(itemView, OnClick, OnLongClick, AudioCallOnClick, VideoCallOnClick);
                return holder;
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
                if (viewHolder is AddNewCall_AdapterViewHolder holder)
                {
                    var item = mCallUserContacts[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirection_RightToLeft)
                        {
                            holder.RelativeLayout_main.LayoutDirection = LayoutDirection.Rtl;
                            holder.Txt_Username.TextDirection = TextDirection.Rtl;
                            holder.Txt_platform.TextDirection = TextDirection.Rtl;
                        }

                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Initialize(AddNewCall_AdapterViewHolder holder, Classes.UserContacts.User item)
        {
            try
            {
                ImageCacheLoader.LoadImage(item.Avatar, holder.ImageAvatar, false, true);

               
                   
                string name = IMethods.Fun_String.DecodeString(item.Name);
                holder.Txt_Username.Text = IMethods.Fun_String.SubStringCutOf(name, 25);

                if (item.UserPlatform == null)
                    item.UserPlatform = "web";

                //User platform
                if (item.UserPlatform.Contains("phone"))
                {
                    holder.Txt_platform.Text = this.Activity_Context.GetString(Resource.String.Lbl_Phone);
                }
                else if (item.UserPlatform.Contains("web"))
                {
                    holder.Txt_platform.Text = this.Activity_Context.GetString(Resource.String.Lbl_Web);
                }
                else
                {
                    holder.Txt_platform.Text = this.Activity_Context.GetString(Resource.String.Lbl_Web);
                }

                //Online Or offline
                if (item.Lastseen == "on")
                {
                    holder.Image_Lastseen.SetImageResource(Resource.Drawable.Green_Online);
                    if (AppSettings.Show_Online_Oflline_Message)
                    {
                        var data = listOnline.Contains(item.Name);
                        if (data == false)
                        {
                            listOnline.Add(item.Name);

                            Toast toast = Toast.MakeText(Activity_Context, item.Name + " " + this.Activity_Context.GetString(Resource.String.Lbl_Online), ToastLength.Short);
                            toast.SetGravity(GravityFlags.Center, 0, 0);
                            toast.Show();
                        }
                    }
                }
                else
                {
                    holder.Image_Lastseen.SetImageResource(Resource.Drawable.Grey_Offline);
                } 
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
                    if (mCallUserContacts == null || mCallUserContacts.Count <= 0)
                        return 0;
                    return mCallUserContacts.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }
         
        // Function  
        public void Add(Classes.UserContacts.User userFollower)
        {
            try
            {
                var check = mCallUserContacts.FirstOrDefault(a => a.UserId == userFollower.UserId);
                if (check == null)
                {
                    mCallUserContacts.Add(userFollower);
                    NotifyItemInserted(mCallUserContacts.IndexOf(mCallUserContacts.Last()));
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
                mCallUserContacts?.Clear();
                NotifyDataSetChanged();
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

        public void Remove(Classes.UserContacts.User users)
        {
            try
            {
                var Index = mCallUserContacts.IndexOf(mCallUserContacts.FirstOrDefault(a => a.UserId == users.UserId));
                if (Index != -1)
                {
                    mCallUserContacts.Remove(users);
                    NotifyItemRemoved(Index);
                    NotifyItemRangeRemoved(0, ItemCount);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Classes.UserContacts.User GetItem(int position)
        {
            return mCallUserContacts[position];
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

        void OnClick(AddNewCall_AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AddNewCall_AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void AudioCallOnClick(AddNewCall_AdapterClickEventArgs args) => AudioCallClick?.Invoke(this, args);
        void VideoCallOnClick(AddNewCall_AdapterClickEventArgs args) => VideoCallClick?.Invoke(this, args);

    }

    public class AddNewCall_AdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; set; }

        public RelativeLayout RelativeLayout_main { get; set; }
        public TextView Txt_Username { get; set; }
        public TextView Txt_platform { get; set; }
        public ImageView ImageAvatar { get; set; }
        public CircleImageView Image_Lastseen { get; set; }
        public AppCompatTextView Txt_IconAudioCall { get; set; }
        public AppCompatTextView Txt_IconVideoCall { get; set; }

        #endregion 

        public AddNewCall_AdapterViewHolder(View itemView, Action<AddNewCall_AdapterClickEventArgs> clickListener, Action<AddNewCall_AdapterClickEventArgs> longClickListener
            , Action<AddNewCall_AdapterClickEventArgs> AudioCallclickListener, Action<AddNewCall_AdapterClickEventArgs> VideoCallclickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout)MainView.FindViewById(Resource.Id.main);
                Txt_Username = (TextView)MainView.FindViewById(Resource.Id.Txt_Username);
                Txt_platform = (TextView)MainView.FindViewById(Resource.Id.Txt_Userplatform);
                ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.Img_Avatar);
                Image_Lastseen = (CircleImageView)MainView.FindViewById(Resource.Id.Img_Lastseen);
                Txt_IconAudioCall = (AppCompatTextView)MainView.FindViewById(Resource.Id.IconAudioCall);
                Txt_IconVideoCall = (AppCompatTextView)MainView.FindViewById(Resource.Id.IconVideoCall);

                itemView.Click += (sender, e) => clickListener(new AddNewCall_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new AddNewCall_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                Txt_IconAudioCall.Click += (sender, e) => AudioCallclickListener(new AddNewCall_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                Txt_IconVideoCall.Click += (sender, e) => VideoCallclickListener(new AddNewCall_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });


                FontController.SetFont(Txt_Username, 1);
                FontController.SetFont(Txt_platform, 3);
                 
                if (AppSettings.Enable_Video_Call)
                {
                    Txt_IconVideoCall.Visibility = ViewStates.Visible;
                    IMethods.Set_TextViewIcon("1", Txt_IconVideoCall, IonIcons_Fonts.IosVideocam);
                  
                }
                else
                {
                    Txt_IconVideoCall.Visibility = ViewStates.Gone;
                }

                if (AppSettings.Enable_Audio_Call)
                {
                    Txt_IconAudioCall.Visibility = ViewStates.Visible;
                    IMethods.Set_TextViewIcon("1", Txt_IconAudioCall, IonIcons_Fonts.AndroidCall);
                    Txt_IconAudioCall.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                }
                else
                {
                    Txt_IconAudioCall.Visibility = ViewStates.Gone;
                }

                ImageCacheLoader.LoadImage("no_profile_image.png", ImageAvatar, false, true);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class AddNewCall_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}