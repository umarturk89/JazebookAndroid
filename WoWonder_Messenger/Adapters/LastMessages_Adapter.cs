using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Widget;
using Refractored.Controls;

using WoWonder.Activities;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using Console = System.Console;

namespace WoWonder.Adapters
{
    public class LastMessages_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<LastMessages_AdapterClickEventArgs> ItemClick;
        public event EventHandler<LastMessages_AdapterClickEventArgs> ItemLongClick;

        public ObservableCollection<Classes.Get_Users_List_Object.User> MLastMessagesUser = new ObservableCollection<Classes.Get_Users_List_Object.User>();

        private Activity Activity_Context;
        private List<string> listOnline = new List<string>();

        public LastMessages_Adapter(Activity context)
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
                //Setup your layout here >> Last_Message_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Last_Message_view, parent, false);
                var holder = new LastMessages_AdapterViewHolder(itemView, OnClick, OnLongClick);
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
                if (viewHolder is LastMessages_AdapterViewHolder holder)
                {
                    var item = MLastMessagesUser[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirection_RightToLeft)
                        {
                            holder.RelativeLayout_main.LayoutDirection = LayoutDirection.Rtl;
                            holder.Txt_Username.TextDirection = TextDirection.Rtl;
                            holder.Txt_LastMessages.TextDirection = TextDirection.Rtl;
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

        public void Initialize(LastMessages_AdapterViewHolder holder, Classes.Get_Users_List_Object.User item)
        {
            try
            {
                ImageCacheLoader.LoadImage(item.ProfilePicture, holder.ImageAvatar, false, true);

                string name = IMethods.Fun_String.DecodeString(item.Name);
                if (holder.Txt_Username.Text != name)
                {
                    holder.Txt_Username.Text = name;
                }

                //If message contains Media files 
                if (item.LastMessage.Media.Contains("image"))
                {
                    if (holder.LastMessagesIcon.Text != IonIcons_Fonts.Images)
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        IMethods.Set_TextViewIcon("1", holder.LastMessagesIcon, IonIcons_Fonts.Images);
                        holder.Txt_LastMessages.Text = this.Activity_Context.GetText(Resource.String.Lbl_SendImageFile);
                    }
                }
                else if (item.LastMessage.Media.Contains("video"))
                {
                    if (holder.LastMessagesIcon.Text != IonIcons_Fonts.Videocamera)
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        IMethods.Set_TextViewIcon("1", holder.LastMessagesIcon, IonIcons_Fonts.Videocamera);
                        holder.Txt_LastMessages.Text = this.Activity_Context.GetText(Resource.String.Lbl_SendVideoFile);
                    }
                }
                else if (item.LastMessage.Media.Contains("sticker"))
                {
                    if (holder.LastMessagesIcon.Text != IonIcons_Fonts.Happy)
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        IMethods.Set_TextViewIcon("1", holder.LastMessagesIcon, IonIcons_Fonts.Happy);
                        holder.Txt_LastMessages.Text = this.Activity_Context.GetText(Resource.String.Lbl_SendStickerFile);
                    }
                }
                else if (item.LastMessage.Media.Contains("sounds"))
                {
                    if (holder.LastMessagesIcon.Text != IonIcons_Fonts.IosMusicalNote)
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        IMethods.Set_TextViewIcon("1", holder.LastMessagesIcon, IonIcons_Fonts.IosMusicalNote);
                        holder.Txt_LastMessages.Text = this.Activity_Context.GetText(Resource.String.Lbl_SendAudioFile);
                    }
                }
                else if (item.LastMessage.Media.Contains("file"))
                {
                    if (holder.LastMessagesIcon.Text != IonIcons_Fonts.IosMusicalNote)
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        IMethods.Set_TextViewIcon("1", holder.LastMessagesIcon, IonIcons_Fonts.Document);
                        holder.Txt_LastMessages.Text = this.Activity_Context.GetText(Resource.String.Lbl_SendFile);
                    }
                }
                else if (item.LastMessage.Stickers.Contains("gif"))
                {
                    if (holder.LastMessagesIcon.Text != "\uf06b")
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        IMethods.Set_TextViewIcon("4", holder.LastMessagesIcon, "\uf06b");
                        holder.Txt_LastMessages.Text = this.Activity_Context.GetText(Resource.String.Lbl_SendGifFile);
                    }
                }
                else
                {
                    holder.LastMessagesIcon.Visibility = ViewStates.Gone;

                    if (item.LastMessage.Text.Contains("http"))
                    {
                        holder.Txt_LastMessages.Text = IMethods.Fun_String.SubStringCutOf(item.LastMessage.Text, 30);
                    }
                    else if (item.LastMessage.Text.Contains("{&quot;Key&quot;") || item.LastMessage.Text.Contains("{key:^qu") || item.LastMessage.Text.Contains("{^key:^qu") || item.LastMessage.Text.Contains("{key:"))
                    {
                        if (holder.LastMessagesIcon.Text != IonIcons_Fonts.IosContact)
                        {
                            holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                            IMethods.Set_TextViewIcon("1", holder.LastMessagesIcon, IonIcons_Fonts.IosContact);
                            holder.Txt_LastMessages.Text = this.Activity_Context.GetText(Resource.String.Lbl_SendContactnumber);
                        }
                    }
                    else
                    {
                        holder.Txt_LastMessages.Text = IMethods.Fun_String.DecodeString(IMethods.Fun_String.SubStringCutOf(item.LastMessage.Text, 30));
                    }
                }

                //last seen time  
                holder.Txt_timestamp.Text = IMethods.ITime.ReplaceTime(item.LastseenTimeText);


                //Online Or offline
                if (item.Lastseen == "on")
                {
                    holder.Txt_timestamp.Text = this.Activity_Context.GetText(Resource.String.Lbl_Online);
                    holder.ImageLastseen.SetImageResource(Resource.Drawable.Green_Online);

                    if (AppSettings.Show_Online_Oflline_Message)
                    {
                        var data = listOnline.Contains(item.Name);
                        if (data == false)
                        {
                            listOnline.Add(item.Name);

                            Toast toast = Toast.MakeText(Activity_Context, item.Name + " " + this.Activity_Context.GetText(Resource.String.Lbl_Online), ToastLength.Short);
                            toast.SetGravity(GravityFlags.Center, 0, 0);
                            toast.Show();
                        }
                    }
                }
                else
                {
                    holder.ImageLastseen.SetImageResource(Resource.Drawable.Grey_Offline);
                }

                //Check read message
                if (item.LastMessage.ToId != UserDetails.User_id && item.LastMessage.FromId == UserDetails.User_id)
                {
                    if (item.LastMessage.Seen == "0" || item.LastMessage.Seen == "")
                    {
                        holder.checkicon.Visibility = ViewStates.Invisible;
                        holder.Txt_Username.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.Txt_LastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                    else
                    {
                        holder.checkicon.Visibility = ViewStates.Visible;
                        holder.Txt_Username.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.Txt_LastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        IMethods.Set_TextViewIcon("1", holder.checkicon, IonIcons_Fonts.AndroidDoneAll);
                    }
                }
                else
                {
                    if (item.LastMessage.Seen == "0" || item.LastMessage.Seen == "")
                    {
                        holder.checkicon.Visibility = ViewStates.Visible;
                        holder.Txt_Username.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
                        holder.Txt_LastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
                        IMethods.Set_TextViewIcon("1", holder.checkicon, IonIcons_Fonts.ChatbubbleWorking);
                    }
                    else
                    {
                        holder.checkicon.Visibility = ViewStates.Invisible;
                        holder.Txt_Username.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.Txt_LastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Function Last_Messages
        public void Add(Classes.Get_Users_List_Object.User users)
        {
            try
            {
                MLastMessagesUser.Add(users);
                NotifyItemInserted(MLastMessagesUser.IndexOf(MLastMessagesUser.Last()));
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
                MLastMessagesUser.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Update(Classes.Get_Users_List_Object.User users)
        {
            try
            {
                var dataUser = MLastMessagesUser.FirstOrDefault(a => a.UserId == users.UserId);
                if (dataUser == null) return;
                {
                    dataUser.UserId = users.UserId;
                    dataUser.Username = users.Username;
                    dataUser.ProfilePicture = users.ProfilePicture;
                    dataUser.CoverPicture = users.CoverPicture;
                    dataUser.LastseenTimeText = users.LastseenTimeText;
                    dataUser.Lastseen = users.Lastseen;
                    dataUser.Url = users.Url;
                    dataUser.Name = users.Name;
                    dataUser.LastseenUnixTime = users.LastseenUnixTime;
                    dataUser.LastseenTimeText = users.LastseenTimeText;
                    dataUser.ChatColor = users.ChatColor;

                    //last_message
                    dataUser.LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                    {
                        Id = users.LastMessage.Id,
                        FromId = users.LastMessage.FromId,
                        GroupId = users.LastMessage.GroupId,
                        ToId = users.LastMessage.ToId,
                        Text = users.LastMessage.Text,
                        Media = users.LastMessage.Media,
                        MediaFileName = users.LastMessage.MediaFileName,
                        MediaFileNames = users.LastMessage.MediaFileNames,
                        Time = users.LastMessage.Time,
                        Seen = users.LastMessage.Seen,
                        DeletedOne = users.LastMessage.DeletedOne,
                        DeletedTwo = users.LastMessage.DeletedTwo,
                        SentPush = users.LastMessage.SentPush,
                        NotificationId = users.LastMessage.NotificationId,
                        TypeTwo = users.LastMessage.TypeTwo,
                        Stickers = users.LastMessage.Stickers,
                        DateTime = users.LastMessage.DateTime,
                    };

                    NotifyItemChanged(MLastMessagesUser.IndexOf(dataUser));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Insert(Classes.Get_Users_List_Object.User users)
        {
            try
            {
                MLastMessagesUser.Insert(0, users);
                NotifyItemInserted(0);

                this.Activity_Context.RunOnUiThread(() =>
                {
                    NotifyItemChanged(MLastMessagesUser.IndexOf(users));
                    Last_Messages_Fragment.LastMessageRecyler.ScrollToPosition(MLastMessagesUser.IndexOf(MLastMessagesUser.Last()));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Move(Classes.Get_Users_List_Object.User users)
        {
            try
            {
                var data = MLastMessagesUser.FirstOrDefault(a => a.UserId == users.UserId);
                if (data != null)
                {
                    var index = MLastMessagesUser.IndexOf(data);
                    if (index > -1 && index != 0)
                    {
                        MLastMessagesUser.Move(index, 0);
                        NotifyItemMoved(index, 0);

                        //Update(data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Remove(Classes.Get_Users_List_Object.User users)
        {
            try
            {
                var index = MLastMessagesUser.IndexOf(MLastMessagesUser.FirstOrDefault(a => a.UserId == users.UserId));
                if (index != -1)
                {
                    MLastMessagesUser.Remove(users);
                    NotifyItemRemoved(index);
                    NotifyItemRangeRemoved(0, ItemCount);
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
                    if (MLastMessagesUser != null)
                    {
                        return MLastMessagesUser.Count;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }


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

        public Classes.Get_Users_List_Object.User GetItem(int position)
        {
            return MLastMessagesUser[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }

        void OnClick(LastMessages_AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(LastMessages_AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class LastMessages_AdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout RelativeLayout_main { get; private set; }
        public AppCompatTextView checkicon { get; private set; }
        public AppCompatTextView LastMessagesIcon { get; private set; }
        public TextView Txt_Username { get; private set; }
        public TextView Txt_LastMessages { get; private set; }
        public TextView Txt_timestamp { get; private set; }
        public ImageView ImageAvatar { get; private set; } //ImageViewAsync
        public CircleImageView ImageLastseen { get; private set; }

        #endregion

        public LastMessages_AdapterViewHolder(View itemView, Action<LastMessages_AdapterClickEventArgs> clickListener, Action<LastMessages_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                RelativeLayout_main = (RelativeLayout)MainView.FindViewById(Resource.Id.main);
                checkicon = (AppCompatTextView)MainView.FindViewById(Resource.Id.IconCheckRead);
                LastMessagesIcon = (AppCompatTextView)MainView.FindViewById(Resource.Id.LastMessages_icon);
                Txt_Username = (TextView)MainView.FindViewById(Resource.Id.Txt_Username);
                Txt_LastMessages = (TextView)MainView.FindViewById(Resource.Id.Txt_LastMessages);
                Txt_timestamp = (TextView)MainView.FindViewById(Resource.Id.Txt_timestamp);
                ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.ImageAvatar);
                ImageLastseen = (CircleImageView)MainView.FindViewById(Resource.Id.ImageLastseen);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new LastMessages_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LastMessages_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                //Dont Remove this code #####
                FontController.SetFont(Txt_Username, 1);
                FontController.SetFont(Txt_LastMessages, 3);
                //#####

                ImageCacheLoader.LoadImage("no_profile_image.png", ImageAvatar, false, true);

            
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Error");
            }
        }

        public Bitmap GetCroppedBitmap(Bitmap bitmap)
        {
            try
            {

                Bitmap output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(output);

                Color color = Android.Graphics.Color.ParseColor("#424242");
                Paint paint = new Paint();
                Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);

                paint.AntiAlias = true;
                canvas.DrawARGB(0, 0, 0, 0);
                paint.Color = color;
                
                canvas.DrawCircle(bitmap.Width / 2, bitmap.Height / 2, bitmap.Width / 2, paint);

                paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                canvas.DrawBitmap(bitmap, rect, rect, paint);
               
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return bitmap;
            }
        }

    }

    public class LastMessages_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}