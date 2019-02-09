using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using WoWonder.Activities.Tab;
using WoWonder.Functions;


namespace WoWonder.Activities
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class AppIntroWalkTroutPage : AppIntro.AppIntro
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                AddSlide(AppIntro.AppIntroFragment.NewInstance(this.GetText(Resource.String.Lbl_Title_page1), this.GetText(Resource.String.Lbl_Description_page1),Resource.Drawable.rocket, Color.ParseColor("#2d4155")));
                AddSlide(AppIntro.AppIntroFragment.NewInstance(this.GetText(Resource.String.Lbl_Title_page2), this.GetText(Resource.String.Lbl_Description_page2),Resource.Drawable.Search_WA, Color.ParseColor("#fcb840")));
                AddSlide(AppIntro.AppIntroFragment.NewInstance(this.GetText(Resource.String.Lbl_Title_page3), this.GetText(Resource.String.Lbl_Description_page3),Resource.Drawable.paper_plane, Color.ParseColor("#2485c3")));
                AddSlide(AppIntro.AppIntroFragment.NewInstance(this.GetText(Resource.String.Lbl_Title_page4), this.GetText(Resource.String.Lbl_Description_page4),Resource.Drawable.speech_bubble, Color.ParseColor("#9244b1")));
                this.SetSkipText(this.GetText(Resource.String.Lbl_Skip));
                this.SetDoneText(this.GetText(Resource.String.Lbl_Done));

                if (AppSettings.Walkthrough_SetFlowAnimation)
                {
                    SetFlowAnimation();
                }
                else if (AppSettings.Walkthrough_SetZoomAnimation)
                {
                    SetZoomAnimation();
                }
                else if (AppSettings.Walkthrough_SetSlideOverAnimation)
                {
                    SetSlideOverAnimation();
                }
                else if (AppSettings.Walkthrough_SetDepthAnimation)
                {
                    SetDepthAnimation();
                }
                else if (AppSettings.Walkthrough_SetFadeAnimation)
                {
                    SetFadeAnimation();
                }

                
                ShowStatusBar(false);
                SetBarColor(Color.ParseColor("#333639"));
                SetSeparatorColor(Color.ParseColor("#2196f3"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        int count = 1;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                if (grantResults[0] == Permission.Granted)
                {

                }
                else
                {
                    //Permission Denied :(
                    Toast.MakeText(this, this.GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnNextPressed()
        {
            try
            {
                base.OnNextPressed();

                var index = this.slidesNumber;
                if (count == 1)
                {
                    //Get data profile
                    var data = API_Request.Get_MyProfileData_Api(this).ConfigureAwait(false);

                    if ((int) Build.VERSION.SdkInt < 23)
                    {

                    }
                    else
                    {
                        RequestPermissions(new String[]
                        {
                            Manifest.Permission.ReadContacts,
                            Manifest.Permission.ReadPhoneNumbers,

                            Manifest.Permission.Camera,
                        }, 208);
                    }

                    count++;
                }
                else if (count == 2)
                {
                    count++;
                }
                else if (count == 3)
                {
                    if ((int) Build.VERSION.SdkInt < 23)
                    {

                    }
                    else
                    {
                        if (AppSettings.ShowButton_RecourdSound)
                        {
                            RequestPermissions(new String[]
                            {
                                Manifest.Permission.RecordAudio,
                                Manifest.Permission.ModifyAudioSettings,
                            }, 103);
                        }

                        API_Request.is_Friend = true;
                        var data = API_Request.Get_users_friends_Async("").ConfigureAwait(false);
                    }

                    count++;
                }
                else if (count == 4)
                {
                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Do something when users tap on Done button.
        public override void OnDonePressed()
        {
            StartActivity(new Intent(Application.Context, typeof(Tabbed_Main_Page)));
            this.Finish();
        }

        // Do something when users tap on Skip button.
        public override void OnSkipPressed()
        {

            RequestPermissions(new String[]
            {
                Manifest.Permission.Camera,
                Manifest.Permission.ReadContacts,
                Manifest.Permission.ReadPhoneNumbers,
            }, 208);

            StartActivity(new Intent(Application.Context, typeof(Tabbed_Main_Page)));
            this.Finish();
        }

    }
}