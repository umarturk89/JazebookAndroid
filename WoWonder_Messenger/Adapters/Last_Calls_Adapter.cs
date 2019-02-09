using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using Console = System.Console;


namespace WoWonder.Adapters
{
    public class Last_Calls_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<Last_Calls_AdapterClickEventArgs> ItemClick;
        public event EventHandler<Last_Calls_AdapterClickEventArgs> ItemLongClick;
        public event EventHandler<Last_Calls_AdapterClickEventArgs> CallClick;

        public ObservableCollection<Classes.Call_User> mCallUser = new ObservableCollection<Classes.Call_User>();

        private Context Activity_Context;

        public Last_Calls_Adapter(Context Context)
        {
            try
            {
                Activity_Context = Context;
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
                //Setup your layout here >> Last_Calls_view
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Last_Calls_view, parent, false);
                var holder = new Last_Calls_AdapterViewHolder(itemView, OnClick, OnLongClick, CallOnClick);
                return holder;
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
                if (viewHolder is Last_Calls_AdapterViewHolder holder)
                {
                    var item = mCallUser[position];
                    if (item != null)
                    {
                        if (AppSettings.FlowDirection_RightToLeft)
                        {
                            holder.RelativeLayout_main.LayoutDirection = LayoutDirection.Rtl;
                            holder.Txt_Username.TextDirection = TextDirection.Rtl;
                            holder.Txt_LastTimecall.TextDirection = TextDirection.Rtl;
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

        public void Initialize(Last_Calls_AdapterViewHolder holder, Classes.Call_User item)
        {
            try
            {
                ImageCacheLoader.LoadImage(item.avatar, holder.Image_Avatar, false, true);
               
                 
                string name = IMethods.Fun_String.DecodeString(item.name);
                holder.Txt_Username.Text = name;

                if (item.typeIcon == "Accept")
                {
                    IMethods.Set_TextViewIcon("2", holder.Txt_LastCall_icon, "\uf095");
                    holder.Txt_LastCall_icon.SetTextColor(Color.ParseColor(item.typeColor));
                }
                else if (item.typeIcon == "Cancel")
                {
                    IMethods.Set_TextViewIcon("2", holder.Txt_LastCall_icon, "\uf3dd");
                    holder.Txt_LastCall_icon.SetTextColor(Color.ParseColor(item.typeColor));
                }
                else if (item.typeIcon == "Declined")
                {
                    IMethods.Set_TextViewIcon("2", holder.Txt_LastCall_icon, "\uf05e");
                    holder.Txt_LastCall_icon.SetTextColor(Color.ParseColor(item.typeColor));
                }

                if (item.time == "Declined call")
                {
                    holder.Txt_LastTimecall.Text = this.Activity_Context.GetText(Resource.String.Lbl_Declined_call);
                }
                else if (item.time == "Missed call")
                {
                    holder.Txt_LastTimecall.Text = this.Activity_Context.GetText(Resource.String.Lbl_Missed_call);
                }
                else if (item.time == "Answered call")
                {
                    holder.Txt_LastTimecall.Text = this.Activity_Context.GetText(Resource.String.Lbl_Answered_call);
                }
                else
                {
                    holder.Txt_LastTimecall.Text = item.time;
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Function Call
        public void Insert(Classes.Call_User Call)
        {
            try
            {
                var check = mCallUser.FirstOrDefault(a => a.id == Call.id);
                if (check == null)
                {
                    mCallUser.Insert(0, Call);
                    NotifyItemInserted(0);
                    Last_Calls_Fragment.LastCallsRecyler?.ScrollToPosition(0);
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
                mCallUser.Clear();
                NotifyDataSetChanged();
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
                    if (mCallUser == null || mCallUser.Count <= 0)
                        return 0;
                    return mCallUser.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        public Classes.Call_User GetItem(int position)
        {
            return mCallUser[position];
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

        void OnClick(Last_Calls_AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(Last_Calls_AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void CallOnClick(Last_Calls_AdapterClickEventArgs args) => CallClick?.Invoke(this, args);

    }

    public class Last_Calls_AdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout RelativeLayout_main { get; set; }
        public TextView Txt_Username { get; private set; }
        public TextView Txt_LastTimecall { get; private set; }
        public AppCompatTextView Txt_LastCall_icon { get; private set; }
        public AppCompatTextView Txt_IconCall { get; private set; }
        public ImageView Image_Avatar { get; private set; }

        #endregion


        public Last_Calls_AdapterViewHolder(View itemView, Action<Last_Calls_AdapterClickEventArgs> clickListener, Action<Last_Calls_AdapterClickEventArgs> longClickListener
            , Action<Last_Calls_AdapterClickEventArgs> CallclickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values

                RelativeLayout_main = (RelativeLayout)MainView.FindViewById(Resource.Id.main);

                Txt_Username = (TextView)MainView.FindViewById(Resource.Id.Txt_name);
                Txt_LastCall_icon = (AppCompatTextView)MainView.FindViewById(Resource.Id.LastCall_icon);
                Txt_LastTimecall = (TextView)MainView.FindViewById(Resource.Id.Txt_Lasttimecalls);
                Image_Avatar = (ImageView)MainView.FindViewById(Resource.Id.ImageAvatar);
                Txt_IconCall = (AppCompatTextView)MainView.FindViewById(Resource.Id.IconCall);

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new Last_Calls_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new Last_Calls_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                Txt_IconCall.Click += (sender, e) => CallclickListener(new Last_Calls_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                IMethods.Set_TextViewIcon("1", Txt_IconCall, IonIcons_Fonts.AndroidCall);
                Txt_IconCall.SetTextColor(Color.ParseColor(AppSettings.MainColor));

                //Dont Remove this code #####
                FontController.SetFont(Txt_Username, 1);
                FontController.SetFont(Txt_LastTimecall, 3);
                //#####

                ImageCacheLoader.LoadImage("no_profile_image.png", Image_Avatar, false, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class Last_Calls_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}