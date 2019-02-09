using System;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using WoWonder.Functions;
using WoWonder.Helpers;

namespace WoWonder.Adapters
{
    public class StickerRecylerAdapter
    {
        public class StickerViewHolder : RecyclerView.ViewHolder
        {

            public View MainView { get; private set; }
            public ImageView ImageViewAsync { get; private set; }

            public StickerViewHolder(View itemView, Action<AdapterClickEvents> listener) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    ImageViewAsync = itemView.FindViewById<ImageView>(Resource.Id.stickerImage);
                    itemView.Click += (sender, e) => listener(new AdapterClickEvents
                    {
                        View = itemView,
                        Position = AdapterPosition
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Error Allen");

                }
            }

        }

        public class Sticker_Adapter : RecyclerView.Adapter
        {
            public event EventHandler<AdapterClickEvents> OnItemClick;
            public static RecyclerView Recylercontrol;
            private JavaList<string> StickerRecylerList;

            public Sticker_Adapter(JavaList<string> StickerList)
            {
                StickerRecylerList = StickerList;
            }

            // Create new views (invoked by the layout manager)
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                //Setup your layout here //  First RUN

                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Sticker_view, parent, false);

                var vh = new StickerViewHolder(row, OnClick);
                return vh;
            }

            // Replace the contents of a view (invoked by the layout manager)
            public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
            {
                try
                {
                    // Replace the contents of the view with that element
                    if (viewHolder is StickerViewHolder holder)
                    {
                        var item = StickerRecylerList[position];
                        if (item != null)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                //Set image 
                                ImageCacheLoader.LoadImage(item, holder.ImageViewAsync, false, false);
                                IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskSticker, item);

                              
                            }
                            else
                            {
                                ImageCacheLoader.LoadImage("ImagePlacholder_circel.png", holder.ImageViewAsync, false, false);

                            }
                        }  
                    }
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
                        if (StickerRecylerList == null || StickerRecylerList.Count <= 0)
                            return 0;
                        return StickerRecylerList.Count;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return 0;
                    }
                }
            }

            public string GetItem(int position)
            {
                return StickerRecylerList[position];
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
}