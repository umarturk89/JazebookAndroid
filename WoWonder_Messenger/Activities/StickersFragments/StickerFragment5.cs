using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View.Animation;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using WoWonder.Activities.ChatWindow;
using WoWonder.Adapters;
using WoWonder.Functions;
using WoWonder.Helpers;


namespace WoWonder.Activities.StickersFragments {
    public class StickerFragment5 : Fragment {
        private View TabPage;
        public RecyclerView StickerRecylerview;
        private GridLayoutManager mLayoutManager;
        public StickerRecylerAdapter.Sticker_Adapter StickerAdapter;
        public string TimeNow = DateTime.Now.ToString ("hh:mm");

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            try {
                TabPage = inflater.Inflate (Resource.Layout.StickerFragment1, container, false);
                StickerRecylerview = (RecyclerView) TabPage.FindViewById (Resource.Id.stickerRecyler1);
                // Stickers.StickerList1

                mLayoutManager = new GridLayoutManager (Activity.ApplicationContext, AppSettings.StickersOnEachRow, GridLayoutManager.Vertical, false);
                StickerRecylerview.SetLayoutManager (mLayoutManager);
                StickerAdapter = new StickerRecylerAdapter.Sticker_Adapter (Stickers.StickerList5);

                StickerRecylerview.SetAdapter (StickerAdapter);

                StickerAdapter.OnItemClick += StickerAdapterOnOnItemClick;

                return TabPage;
            } catch (Exception e) {
                Console.WriteLine (e);
                return null;
            }

        }

        private void StickerAdapterOnOnItemClick (object sender, AdapterClickEvents adapterClickEvents) {
            try {
                var StcikerUrl = StickerAdapter.GetItem (adapterClickEvents.Position);

                var unixTimestamp = (Int32) (DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds;

                Classes.Message m1 = new Classes.Message
                {
                    M_id = unixTimestamp.ToString(),
                    from_id = UserDetails.User_id,
                    to_id = ChatWindow_Activity.Userid,
                    media = StcikerUrl,
                    time_text = TimeNow,
                    position = "right",
                    type = "right_sticker"
                };

                ChatWindow_Activity.MAdapter.Add (m1);

                //Sticker Send Function
                if (IMethods.CheckConnectivity ()) {
                    MessageController.SendMessageTask(ChatWindow_Activity.Userid, unixTimestamp.ToString(), "", "", "", StcikerUrl, "sticker" + adapterClickEvents.Position).ConfigureAwait(false);
                } else {
                    Toast.MakeText (this.Context, this.GetText(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show ();
                }

                try {
                    var ChatWindow = ((ChatWindow_Activity) Activity);

                    var interplator = new FastOutSlowInInterpolator ();
                    ChatWindow.ChatStickerButton.Tag = "Closed";

                    ChatWindow.ResetButtonTags ();
                    ChatWindow.ChatStickerButton.Drawable.SetTint (Android.Graphics.Color.ParseColor ("#888888"));
                    ChatWindow.TopFragmentHolder.Animate ().SetInterpolator (interplator).TranslationY (1200).SetDuration (300);
                    ChatWindow.SupportFragmentManager.BeginTransaction ().Remove (ChatWindow.Chat_StickersTab_BoxFragment).Commit ();
                } catch (Exception exception) {
                    Console.WriteLine (exception);
                }
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }


        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnDestroy()
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}