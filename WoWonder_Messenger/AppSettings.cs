//Use DoughouzChecker last version 3.0 to 
//build your own certifcate 
//For Full Documention 
//https://paper.dropbox.com/doc/WoWonder-Messenger-2.0--ARqO4OoLf_KXGWT63gNm0pOuAQ-M6qrJYGQ0C0NZlhZ3PqI7
//CopyRight DoughouzLight

namespace WoWonder
{
    public class AppSettings
    {

        public string TripleDesAppServiceProvider;

        //*********************************************************

        public AppSettings()
        {
            TripleDesAppServiceProvider = "IbxeixrTQTIbjj2d58d1xf0LkQgNNrGF/wnJMjvzCVo1cAyHrbuXxEgUBP4gi2z34hVrr/XZJ3qmhjImH+uOVe581fKpa+3+yf49NMomwVsqdfnBVEDTJIGa4GeEvKzgvKNY4AnNjB3XSOZkekN3sxsZ/d1TOjoGIv6ErNipmjF3aCxp0P40WwyHdM1l5Wy0mFNV+x2r47QAS1zP6zHG/9TF3YuIowAXo24B0qeYIaADAeWXyq8aMtm1PDABXX1A";
        }

        //Main Settings >>>>>
        //*********************************************************

        public static string Version = "2.2";

        public static string Application_Name = "Jazebook Messenger";

        // Friend system = 0 , follow system = 1
        public static string ConnectivitySystem = "1";

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#a84849";

        public static string Story_Default_Color = "#b96c6d";
        public static string Story_Read_Color = "#808080";

        //Language Settings >>
        //*********************************************************
        public static bool FlowDirection_RightToLeft = false;
        public static string Lang = ""; //Default language

        //Type connection >>
        //*********************************************************
        public static bool Use_SocketClient = false; //For next update versions  

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        //ADMOB >> Please add the code ad in the Here and Strings.xml 
        //*********************************************************
        public static bool Show_ADMOB_Banner = false;
        public static bool Show_ADMOB_Interstitial = false;
        public static bool Show_ADMOB_RewardVideo = false;

        public static string Ad_App_ID = "ca-app-pub-5135691635931982~4077881706";
        public static string Ad_Banner_Key = "ca-app-pub-5135691635931982/6776323500";
        public static string Ad_Interstitial_Key = "ca-app-pub-5135691635931982/4466434529";
        public static string Ad_RewardVideo_Key = "ca-app-pub-5135691635931982/7731317149";

        //Three times after entering the ad is displayed
        public static int Show_ADMOB_Interstitial_Count = 3;
        public static int Show_ADMOB_RewardedVideo_Count = 3;

        //Social Logins >> 
        //*********************************************************
        public static bool Show_Facebook_Login = false;
        public static bool Show_Google_Login = false;

        //ChatWindow_Activity >>
        //*********************************************************
        public static bool Show_Button_image = true;
        public static bool Show_Button_video = true;
        public static bool Show_Button_contact = true;
        public static bool Show_Button_attachfile = true;
        public static bool Show_Button_color = true;
        public static bool Show_Button_stickers = true;
        public static bool Show_Button_Music = true;
        public static bool Show_Button_Gif = false; //For next update versions 

        //Recourd Sound Style & Text
        ///*********************************************************
        public static bool ShowButton_RecourdSound = true;

        public static string MW_Recourd_ImageButton = "RecourdImage.png";
        public static string MW_Recourd_StopImageButton = "stop.png";

        //Tabbed_Main_Page >>
        //*********************************************************
        public static bool Show_Title_Username = false;

        // Video/Audio Call Settings >>
        //*********************************************************
        public static bool Enable_Audio_Video_Call = true;

        public static bool Enable_Audio_Call = true;
        public static bool Enable_Video_Call = true;

        public static bool Use_Agora_Library = true;
        public static bool Use_Twilio_Library = false;

        // Walkthrough Settings >>
        //*********************************************************
        public static bool Show_WalkTroutPage = true;

        public static bool Walkthrough_SetFlowAnimation = true;
        public static bool Walkthrough_SetZoomAnimation = false;
        public static bool Walkthrough_SetSlideOverAnimation = false;
        public static bool Walkthrough_SetDepthAnimation = false;
        public static bool Walkthrough_SetFadeAnimation = false;

        //Last_Messages Page >>
        ///*********************************************************
        public static bool Show_Online_Oflline_Message = true;

        public static bool Invitation_System = false;   //Invite friends section

        public static int RefreshChatActivitiesSecounds = 6000; // 6 Secounds
        public static int MessageRequestSpeed = 3000; // 3 Secounds

        public static bool RenderPriorityFastPostLoad = true;

        //After this time set update API to get the stories.
        //*********************************************************
        public static int UpdateStory = 10; // 10 * 6 == 60 Secounds

        //Bypass Web Erros 
        ///*********************************************************
        public static bool TurnTrustFailureOn_WebException = false;
        public static bool TurnSecurityProtocolType3072On = false;

        // Stickers Packs Settings >>
        //*********************************************************
        public static int StickersOnEachRow = 3;
        public static string StickersBarColor = "#efefef";

        public static bool ShowStickerspack0 = true;
        public static bool ShowStickerspack1 = true;
        public static bool ShowStickerspack2 = true;
        public static bool ShowStickerspack3 = true;
        public static bool ShowStickerspack4 = true;
        public static bool ShowStickerspack5 = true;
        public static bool ShowStickerspack6 = false;
    }
}
