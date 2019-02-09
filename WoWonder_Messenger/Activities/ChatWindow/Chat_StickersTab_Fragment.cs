using System;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using FFImageLoading;
using WoWonder.Activities.StickersFragments;
using WoWonder.Adapters;
using Fragment = Android.Support.V4.App.Fragment;


namespace WoWonder.Activities.ChatWindow
{
    public class Chat_StickersTab_Fragment : Fragment
    {
        private View MainTabPage;
        private TabLayout tabs;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                MainTabPage = inflater.Inflate(Resource.Layout.Chat_StickersTab_Fragment, container, false);
                tabs = MainTabPage.FindViewById<TabLayout>(Resource.Id.tabsSticker);
                ViewPager viewPager = MainTabPage.FindViewById<ViewPager>(Resource.Id.viewpagerSticker);
                AppBarLayout AppBarLayoutview = MainTabPage.FindViewById<AppBarLayout>(Resource.Id.appbarSticker);

                SetUpViewPager(viewPager);

                return MainTabPage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                StickersTab_Adapter adapter = new StickersTab_Adapter(ChildFragmentManager);
                if (AppSettings.ShowStickerspack0)
                    adapter.AddFragment(new StickerFragment1(), "0");

                if (AppSettings.ShowStickerspack1)
                    adapter.AddFragment(new StickerFragment2(), "1");

                if (AppSettings.ShowStickerspack2)
                    adapter.AddFragment(new StickerFragment3(), "2");

                if (AppSettings.ShowStickerspack3)
                    adapter.AddFragment(new StickerFragment4(), "3");

                if (AppSettings.ShowStickerspack4)
                    adapter.AddFragment(new StickerFragment5(), "4");

                if (AppSettings.ShowStickerspack5)
                    adapter.AddFragment(new StickerFragment6(), "5");

                if (AppSettings.ShowStickerspack6)
                    adapter.AddFragment(new StickerFragment7(), "6");

                viewPager.Adapter = adapter;
                tabs.SetupWithViewPager(viewPager);
                tabs.SetBackgroundColor(Color.ParseColor(AppSettings.StickersBarColor));

                if (tabs.TabCount > 0)
                {
                    for (int i = 0; i <= tabs.TabCount; i++)
                    {
                        var StickerReplacer = tabs.GetTabAt(i);
                        if (StickerReplacer != null)
                        {
                            if (StickerReplacer.Text == "0")
                                StickerReplacer.SetIcon(Resource.Drawable.Sticker1).SetText("");

                            if (StickerReplacer.Text == "1")
                                StickerReplacer.SetIcon(Resource.Drawable.sticker2).SetText("");

                            if (StickerReplacer.Text == "2")
                                StickerReplacer.SetIcon(Resource.Drawable.Sticker3).SetText("");

                            if (StickerReplacer.Text == "3")
                                StickerReplacer.SetIcon(Resource.Drawable.Sticker4).SetText("");

                            if (StickerReplacer.Text == "4")
                                StickerReplacer.SetIcon(Resource.Drawable.Sticker5).SetText("");

                            if (StickerReplacer.Text == "5")
                                StickerReplacer.SetIcon(Resource.Drawable.Sticker6).SetText("");

                            if (StickerReplacer.Text == "6")
                                StickerReplacer.SetIcon(Resource.Drawable.Sticker7).SetText("");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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