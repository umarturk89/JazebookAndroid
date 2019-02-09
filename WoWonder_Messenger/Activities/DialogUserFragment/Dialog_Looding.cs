using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Com.Airbnb.Lottie;
using FFImageLoading;

namespace WoWonder.Activities.DialogUserFragment {
    public class Dialog_Looding : DialogFragment {
        public class OnLooding_EventArgs : EventArgs {
            public View View { get; set; }
            public int Position { get; set; }
        }

        public event EventHandler<OnLooding_EventArgs> _OnLoodingComplete;

        public Dialog_Looding () {
            try {

            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            try {
                base.OnCreateView (inflater, container, savedInstanceState);

                // Set our view from the "Dialog_Looding_Fragment" layout resource
                var view = inflater.Inflate (Resource.Layout.Dialog_Looding_Fragment, container, false);

                LottieAnimationView animationView = view.FindViewById<LottieAnimationView> (Resource.Id.animation_view);
                animationView.SetAnimation ("material_wave_loading.json");

                return view;
            } catch (Exception e) {
                Console.WriteLine (e);
                return null;
            }
        }

        //animations
        public override void OnActivityCreated (Bundle savedInstanceState) {
            Dialog.Window.RequestFeature (WindowFeatures.NoTitle); //Sets the title bar to invisible

            base.OnActivityCreated (savedInstanceState);

            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
    }
}