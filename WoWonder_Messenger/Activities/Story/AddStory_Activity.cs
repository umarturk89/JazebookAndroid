using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.InputMethodServices;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AT.Markushi.UI;
using Com.Cenkgun.Chatbar;
using FFImageLoading;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Story;
using WoWonder_API.Requests;
using File = Java.IO.File;

namespace WoWonder.Activities.Story
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AddStory_Activity : AppCompatActivity, View.IOnClickListener
    {
        //Event Click Add Story
        public void OnClick(View v)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    if (Type == "image")
                    {
                        //Send image function
                        var file = Android.Net.Uri.FromFile(new File(Uri));
                        PikedImage = IMethods.Fun_String.TrimTo(file.Path.Split('/').Last(), 30);
                        var stream = IMethods.MultiMedia.GetMedia_as_Stream(file.Path);

                        var check = ActivityListItems.FirstOrDefault(a => a.Label == PikedImage);
                        if (check == null)
                            ActivityListItems.Add(new Classes.Storyitems
                            {
                                Label = PikedImage,
                                ImageFullPath = file.Path,
                                ImageStream = stream
                            });

                        var attach = ActivityListItems.FirstOrDefault(a => a.Label == PikedImage);
                        if (attach != null)
                        {
                            List<Stream> streams = new List<Stream>();

                            foreach (var Atta in ActivityListItems)
                                if (Atta.ImageStream != null)
                                    streams.Add(Atta.ImageStream);

                            unixTimestamp = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                            Time = Convert.ToString(unixTimestamp);

                            List<GetStoriesObject.Story> listOfStories = new List<GetStoriesObject.Story>();
                            var checkUser =
                                Last_Stroies_Fragment.StoryAdapter?.mStoryList?.FirstOrDefault(a =>
                                    a.user_id == UserDetails.User_id);
                            if (checkUser != null)
                            {
                                var checkUserExits =
                                    Classes.StoryList.FirstOrDefault(a => a.Value == checkUser.user_id);
                                if (checkUserExits.Value == null)
                                {
                                    var ch = checkUserExits.Key?.FirstOrDefault(a => a.id == checkUser.id);
                                    if (ch == null)
                                    {
                                        GetStoriesObject.Story story = new GetStoriesObject.Story
                                        {
                                            user_data = new GetStoriesObject.UserData()
                                            {
                                                name = !string.IsNullOrEmpty(UserDetails.Full_name)
                                                    ? UserDetails.Full_name
                                                    : UserDetails.Username,
                                                avatar = UserDetails.avatar,
                                            },
                                            thumbnail = file.Path,
                                            id = Time,
                                            user_id = UserDetails.User_id,
                                            title = Txt_AboutStory.MessageText,
                                            description = Txt_AboutStory.MessageText,
                                            is_owner = true,
                                            type = "image",
                                        };

                                        listOfStories.Add(story);

                                        Classes.StoryList.Add(listOfStories, story.user_id);
                                    }
                                }
                                else
                                {
                                    foreach (var item in Classes.StoryList.Keys.ToList())
                                    {
                                        string userId = item.FirstOrDefault(a => a.user_id == checkUser.user_id)
                                            ?.user_id;  

                                        if (checkUserExits.Value == userId)
                                        {
                                            GetStoriesObject.Story story = new GetStoriesObject.Story
                                            {
                                                user_data = new GetStoriesObject.UserData()
                                                {
                                                    name = !string.IsNullOrEmpty(UserDetails.Full_name)
                                                        ? UserDetails.Full_name
                                                        : UserDetails.Username,
                                                    avatar = UserDetails.avatar,
                                                },
                                                thumbnail = file.Path,
                                                id = Time,
                                                user_id = UserDetails.User_id,
                                                title = Txt_AboutStory.MessageText,
                                                description = Txt_AboutStory.MessageText,
                                                is_owner = true,
                                                type = "image",
                                            };

                                            var ch = item.FirstOrDefault(a => a.id == story.id);
                                            if (ch == null)
                                            {
                                                item.Add(story);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                GetStoriesObject.Story story = new GetStoriesObject.Story
                                {
                                    user_data = new GetStoriesObject.UserData()
                                    {
                                        name = !string.IsNullOrEmpty(UserDetails.Full_name)
                                            ? UserDetails.Full_name
                                            : UserDetails.Username,
                                        avatar = UserDetails.avatar,
                                    },
                                    thumbnail = file.Path,
                                    id = Time,
                                    user_id = UserDetails.User_id,
                                    title = Txt_AboutStory.MessageText,
                                    description = Txt_AboutStory.MessageText,
                                    is_owner = true,
                                    type = "image",
                                };

                                Last_Stroies_Fragment.StoryAdapter?.Add(story);

                                listOfStories.Clear();

                                var checkUserExits = Classes.StoryList.FirstOrDefault(a => a.Value == story.user_id);
                                if (checkUserExits.Value == null)
                                {
                                    listOfStories.Add(story);
                                    Classes.StoryList.Add(listOfStories, story.user_id);
                                }
                            }

                            Task.Factory.StartNew(async () =>
                            {
                                //just pass file_path and type video or image

                                var (Api_status, Respond) = await WoWonder_API.Requests.RequestsAsync.Story.Create_Story(Txt_AboutStory.MessageText,
                                    "", file.Path, "image");
                                if (Api_status == 200)
                                {
                                    if (Respond is CreateStoryObject result)
                                    {
                                        Console.WriteLine(result.story_id);

                                        Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short)
                                            .Show();
                                    }
                                }
                                else
                                {
                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong),
                                        ToastLength.Short).Show();
                                }
                            });
                            Finish();
                        }
                    }
                    else
                    {
                        VideoTimer.Enabled = false;
                        VideoTimer.Stop();

                        //Send image function
                        var file = Android.Net.Uri.FromFile(new File(Uri));

                        List<GetStoriesObject.Story> listOfStories = new List<GetStoriesObject.Story>();

                        var checkUser =
                            Last_Stroies_Fragment.StoryAdapter?.mStoryList?.FirstOrDefault(a =>
                                a.user_id == UserDetails.User_id);
                        if (checkUser != null)
                        {
                            if (Classes.StoryList == null)
                                return;

                            var checkUserExits = Classes.StoryList.FirstOrDefault(a => a.Value == checkUser.user_id);
                            if (checkUserExits.Value == null)
                            {
                                var ch = checkUserExits.Key.FirstOrDefault(a => a.id == checkUser.id);
                                if (ch == null)
                                {
                                    GetStoriesObject.Story story = new GetStoriesObject.Story
                                    {
                                        user_data = new GetStoriesObject.UserData()
                                        {
                                            name = !string.IsNullOrEmpty(UserDetails.Full_name)
                                                ? UserDetails.Full_name
                                                : UserDetails.Username,
                                            avatar = UserDetails.avatar,
                                        },
                                        thumbnail = file.Path,
                                        id = Time,
                                        user_id = UserDetails.User_id,
                                        title = Txt_AboutStory.MessageText,
                                        description = Txt_AboutStory.MessageText,
                                        is_owner = true,
                                        type = "video",
                                    };

                                    listOfStories.Add(story);

                                    Classes.StoryList.Add(listOfStories, story.user_id);
                                }
                            }
                            else
                            {
                                if (Classes.StoryList == null)
                                    return;

                                foreach (var item in Classes.StoryList?.Keys.ToList())
                                {
                                    string userId = item.FirstOrDefault(a => a.user_id == checkUser.user_id)?.user_id;
                                    if (checkUserExits.Value == userId)
                                    {
                                        GetStoriesObject.Story story = new GetStoriesObject.Story
                                        {
                                            user_data = new GetStoriesObject.UserData()
                                            {
                                                name = !string.IsNullOrEmpty(UserDetails.Full_name)
                                                    ? UserDetails.Full_name
                                                    : UserDetails.Username,
                                                avatar = UserDetails.avatar,
                                            },
                                            thumbnail = file.Path,
                                            id = Time,
                                            user_id = UserDetails.User_id,
                                            title = Txt_AboutStory.MessageText,
                                            description = Txt_AboutStory.MessageText,
                                            is_owner = true,
                                            type = "video",
                                        };

                                        var ch = item.FirstOrDefault(a => a.id == story.id);
                                        if (ch == null)
                                        {
                                            item.Add(story);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            GetStoriesObject.Story story = new GetStoriesObject.Story
                            {
                                user_data = new GetStoriesObject.UserData()
                                {
                                    name = !string.IsNullOrEmpty(UserDetails.Full_name)
                                        ? UserDetails.Full_name
                                        : UserDetails.Username,
                                    avatar = UserDetails.avatar,
                                },
                                thumbnail = file.Path,
                                id = Time,
                                user_id = UserDetails.User_id,
                                title = Txt_AboutStory.MessageText,
                                description = Txt_AboutStory.MessageText,
                                is_owner = true,
                                type = "video",
                            };

                            Last_Stroies_Fragment.StoryAdapter?.Add(story);

                            listOfStories.Clear();

                            var checkUserExits = Classes.StoryList.FirstOrDefault(a => a.Value == story.user_id);
                            if (checkUserExits.Value == null)
                            {
                                listOfStories.Add(story);
                                Classes.StoryList.Add(listOfStories, story.user_id);
                            }
                        }

                        Task.Factory.StartNew(async () =>
                        {
                            //just pass file_path and type video or image
                            var (Api_status, Respond) = await WoWonder_API.Requests.RequestsAsync.Story.Create_Story(Txt_AboutStory.MessageText,
                                "", file.Path, "video");
                            if (Api_status == 200)
                            {
                                if (Respond is CreateStoryObject result)
                                {
                                    Console.WriteLine(result.story_id);

                                    var getStory =
                                        Classes.StoryList.FirstOrDefault(a => a.Value == UserDetails.User_id);
                                    if (getStory.Value != null)
                                    {
                                        var dataStory = getStory.Key.FirstOrDefault(a => a.id == Time);
                                        if (dataStory != null)
                                        {
                                            dataStory.id = result.story_id.ToString();
                                        }
                                    }

                                    Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();

                                }
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong),
                                    ToastLength.Short).Show();
                            }
                        });

                        Finish();
                    }
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Error_check_internet_connection),
                            ToastLength.Short)
                        .Show();
                }

                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                SetContentView(Resource.Layout.AddStory_layout);
                 
                var data_Uri = Intent.GetStringExtra("Uri") ?? "Data not available";
                if (data_Uri != "Data not available" && !string.IsNullOrEmpty(data_Uri)) Uri = data_Uri; // Uri file 
                var data_Type = Intent.GetStringExtra("Type") ?? "Data not available";
                if (data_Type != "Data not available" && !string.IsNullOrEmpty(data_Type))
                    Type = data_Type; // Type file 

                //Get values
                StoriesProgressViewDisplay = FindViewById<ProgressBar>(Resource.Id.storiesview);
                MainLayout = FindViewById<RelativeLayout>(Resource.Id.storyDisplay);
                imagstoryDisplay = FindViewById<ImageView>(Resource.Id.imagstoryDisplay);
                videoView = FindViewById<VideoView>(Resource.Id.VideoView);
                UserProfileImage = FindViewById<ImageView>(Resource.Id.userImage);
                usernameText = FindViewById<TextView>(Resource.Id.usernameText);
                Txt_AboutStory = FindViewById<ChatBarView>(Resource.Id.Txt_StoryAbout);

                Btn_Video_icon = FindViewById<CircleButton>(Resource.Id.Videoicon_button);
                BackIcon = FindViewById<TextView>(Resource.Id.backicon);
                LoadingProgressBarview = FindViewById<ProgressBar>(Resource.Id.loadingProgressBarview);
                LoadingProgressBarview.Visibility = ViewStates.Gone;

                Btn_Video_icon.Visibility = ViewStates.Gone;
                Btn_Video_icon.Tag = "Play";

                Txt_AboutStory.SetMessageBoxHint(this.GetString(Resource.String.Lbl_Add_caption));
                Txt_AboutStory.SetSendClickListener(this);

                imagstoryDisplay.Click += ImagstoryDisplay_Click;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ImagstoryDisplay_Click(object sender, EventArgs e)
        {
            //Dismiss Keybaord
            InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                Btn_Video_icon.Click += VideoIconOnClick;
                MainLayout.Click += MainLayoutDisplay_Click;
                videoView.Completion += VideoView_Completion;
                videoView.Prepared += VideoView_Prepared;
                BackIcon.Click += BackIcon_Click;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                Btn_Video_icon.Click -= VideoIconOnClick;
                MainLayout.Click -= MainLayoutDisplay_Click;
                videoView.Completion -= VideoView_Completion;
                videoView.Prepared -= VideoView_Prepared;
                BackIcon.Click -= BackIcon_Click;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void VideoIconOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Btn_Video_icon.Tag.ToString() == "Play")
                {
                    videoView.Start();

                    if (Uri.Contains("http"))
                    {
                        videoView.SetVideoURI(Android.Net.Uri.Parse(Uri));
                    }
                    else
                    {
                        var file = Android.Net.Uri.FromFile(new File(Uri));
                        videoView.SetVideoPath(file.Path);
                    }

                    VideoTimer = new Timer();
                    VideoTimer.Interval = 100;
                    VideoTimer.Enabled = true;
                    VideoTimer.Elapsed += Timervideo_Elapsed;
                    VideoTimer.Start();

                    Btn_Video_icon.Tag = "Stop";
                    Btn_Video_icon.SetImageResource(Resource.Drawable.ic_stop_white_24dp);
                }
                else
                {
                    VideoTimer.Enabled = false;
                    VideoTimer.Stop();

                    videoView.Suspend();

                    Btn_Video_icon.Tag = "Play";
                    Btn_Video_icon.SetImageResource(Resource.Drawable.ic_play_arrow);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnStart()
        {
            try
            {
                base.OnStart();

                IMethods.Set_TextViewIcon("1", BackIcon, IonIcons_Fonts.AndroidArrowBack);

                //profile_picture
                ImageCacheLoader.LoadImage(UserDetails.avatar, UserProfileImage, false, true);

                if (!string.IsNullOrEmpty(UserDetails.Full_name))
                    usernameText.Text = UserDetails.Full_name;
                else
                    usernameText.Text = UserDetails.Username;
                 
                ActivityListItems.Clear();

                if (Type == "image")
                    SetImageStory(Uri);
                else
                    SetvideoStory(Uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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

        protected override void OnDestroy()
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

        #region Variables Basic

        public List<string> ListOfStories = new List<string>();
        public List<string> ListOfStoriesText = new List<string>();
        private RelativeLayout MainLayout;
        private ImageView imagstoryDisplay;
        private ImageView UserProfileImage;

        private Timer VideoTimer = new Timer();

        private TextView usernameText;
        private TextView BackIcon;
        private CircleButton Btn_Video_icon;

        private ChatBarView Txt_AboutStory;

        private int countstory, progrescount;

        private ProgressBar StoriesProgressViewDisplay;
        private VideoView videoView;
        private ProgressBar LoadingProgressBarview;

        private static int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        private static string Time = Convert.ToString(unixTimestamp);
        private string PikedImage = "";

        private static readonly ObservableCollection<Classes.Storyitems> ActivityListItems = new ObservableCollection<Classes.Storyitems>();

        private string Uri = "";
        private string Type = "";

        #endregion

        #region Method Add

        public void SetImageStory(string url)
        {
            try
            {
                if (imagstoryDisplay.Visibility == ViewStates.Gone)
                    imagstoryDisplay.Visibility = ViewStates.Visible;

                var file = Android.Net.Uri.FromFile(new File(url));

                ImageCacheLoader.LoadImage(file.Path, imagstoryDisplay, false, false);


                if (videoView.Visibility == ViewStates.Visible)
                    videoView.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetvideoStory(string url)
        {
            try
            {
                if (imagstoryDisplay.Visibility == ViewStates.Visible)
                    imagstoryDisplay.Visibility = ViewStates.Gone;

                if (videoView.Visibility == ViewStates.Gone) videoView.Visibility = ViewStates.Visible;

                Btn_Video_icon.Visibility = ViewStates.Visible;

                Btn_Video_icon.Tag = "Play";
                Btn_Video_icon.SetImageResource(Resource.Drawable.ic_play_arrow);

                LoadingProgressBarview.Visibility = ViewStates.Visible;

                if (videoView.IsPlaying)
                    videoView.Suspend();

                if (url.Contains("http"))
                {
                    videoView.SetVideoURI(Android.Net.Uri.Parse(url));
                }
                else
                {
                    var file = Android.Net.Uri.FromFile(new File(url));
                    videoView.SetVideoPath(file.Path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Timervideo_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                var ff = videoView.CurrentPosition * 100 / videoView.Duration;
                StoriesProgressViewDisplay.SetProgress(ff, true);
            });
        }

        private void MainLayoutDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                if (VideoTimer != null && VideoTimer.Enabled)
                {
                    LoadingProgressBarview.Visibility = ViewStates.Gone;
                    VideoTimer.Stop();
                }

                if (countstory < ListOfStories.Count)
                {
                    // ChangeStoryView();
                }
                else
                {
                    Finish();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BackIcon_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void VideoView_Prepared(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                LoadingProgressBarview.Visibility = ViewStates.Gone;
                var Max = 100;

                StoriesProgressViewDisplay.Max = Max;
            });
        }

        private void VideoView_Completion(object sender, EventArgs e)
        {
            try
            {
                VideoTimer.Enabled = false;
                VideoTimer.Stop();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
    }
}