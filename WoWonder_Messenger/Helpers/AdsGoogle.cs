using System;
using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;
using Android.Widget;


namespace WoWonder.Helpers
{
    public class AdsGoogle
    {
        //Interstitial >> 
        //==================================================

        #region Interstitial

        public class AdmobInterstitial
        {
            public InterstitialAd _ad;
            public void ShowAd(Context context)
            {
                try
                {
                    _ad = new InterstitialAd(context);
                    _ad.AdUnitId = AppSettings.Ad_Interstitial_Key;

                    var intlistener = new InterstitialAdListener(_ad);
                    intlistener.OnAdLoaded();
                    _ad.AdListener = intlistener;
                    string android_id = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                    var requestbuilder = new AdRequest.Builder();
                    requestbuilder.AddTestDevice(android_id);
                    _ad.LoadAd(requestbuilder.Build());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public class InterstitialAdListener : AdListener
        {
            public InterstitialAd _ad;

            public InterstitialAdListener(InterstitialAd ad)
            {
                _ad = ad;
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();

                if (_ad.IsLoaded)
                    _ad.Show();
            }
        }

        private static int Count_Interstitial = 0;
        public static void Ad_Interstitial(Context context)
        {
            try
            {
                if (AppSettings.Show_ADMOB_Interstitial)
                {
                    if (Count_Interstitial == AppSettings.Show_ADMOB_Interstitial_Count)
                    {
                        Count_Interstitial = 0;
                        AdmobInterstitial ads = new AdmobInterstitial();
                        ads.ShowAd(context);
                    }
                    Count_Interstitial++;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion

        //Rewarded Video >>
        //===================================================

        #region Rewarded

        public class AdmobRewardedVideo : AdListener, IRewardedVideoAdListener
        {
            public IRewardedVideoAd Rad;
            public void ShowAd(Context context)
            {
                try
                { 
                    // Use an activity context to get the rewarded video instance.
                    Rad = MobileAds.GetRewardedVideoAdInstance(context);
                    Rad.RewardedVideoAdListener = this;

                    OnRewardedVideoAdLoaded();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    base.OnAdLoaded();

                    OnRewardedVideoAdLoaded();

                    if (Rad.IsLoaded)
                        Rad.Show();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public void OnRewarded(IRewardItem reward)
            {
              
                if (Rad.IsLoaded)
                    Rad.Show();
            }

            public void OnRewardedVideoAdClosed()
            {
                OnRewardedVideoAdLoaded();
            }

            public void OnRewardedVideoAdFailedToLoad(int errorCode)
            {
                Toast.MakeText(Application.Context, "No ads currently available", ToastLength.Short).Show();
            }

            public void OnRewardedVideoAdLeftApplication()
            {

            }

            public void OnRewardedVideoAdLoaded( )
            {
                string android_id = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                var requestbuilder = new AdRequest.Builder();
                requestbuilder.AddTestDevice(android_id); 
                Rad.LoadAd(AppSettings.Ad_RewardVideo_Key, requestbuilder.Build());
                Rad.Show();
            }

            public void OnRewardedVideoAdOpened()
            {

            }

            public void OnRewardedVideoStarted()
            {

            }

            void IRewardedVideoAdListener.OnRewardedVideoAdClosed()
            {
                OnRewardedVideoAdClosed();
            }
        }


        private static int Count_RewardedVideo = 0;
        public static void Ad_RewardedVideo(Context context)
        {
            try
            {
                if (AppSettings.Show_ADMOB_RewardVideo)
                {
                    if (Count_RewardedVideo == AppSettings.Show_ADMOB_RewardedVideo_Count)
                    {
                        Count_RewardedVideo = 0;
                        AdmobRewardedVideo ads = new AdmobRewardedVideo();
                        ads.ShowAd(context);
                    }
                    Count_RewardedVideo++;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}