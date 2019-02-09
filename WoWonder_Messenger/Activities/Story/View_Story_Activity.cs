using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Views;
using FFImageLoading.Work;
using Newtonsoft.Json;
using UniversalImageLoader.Core;
using WoWonder.Activities.Tab;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Story;
using WoWonder_API.Requests;
using Console = System.Console;
using Uri = Android.Net.Uri;


namespace WoWonder.Activities.Story
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class View_Story_Activity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int) Build.VERSION.SdkInt < 23)
                {
                }
                else
                {
                    Window.AddFlags(WindowManagerFlags.Fullscreen);
                    Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
                }

                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);


                SetContentView(Resource.Layout.View_Story_Layout);
                 
                var datalist = JsonConvert.DeserializeObject<GetStoriesObject.Story>(Intent.GetStringExtra("Story"));
                if (datalist != null) _Story_Data = datalist;

                //Get values
                StoriesProgressViewDisplay = FindViewById<ProgressBar>(Resource.Id.storiesview);
                MainLayout = FindViewById<RelativeLayout>(Resource.Id.storyDisplay);
                imagstoryDisplay = FindViewById<ImageViewAsync>(Resource.Id.imagstoryDisplay);

                storyaboutText = FindViewById<TextView>(Resource.Id.storyaboutText);
                videoView = FindViewById<VideoView>(Resource.Id.VideoView);
                UserProfileImage = FindViewById<ImageView>(Resource.Id.userImage);
                usernameText = FindViewById<TextView>(Resource.Id.usernameText);
                Txt_LastSeen = FindViewById<TextView>(Resource.Id.LastSeenText);
                CountStoryText = FindViewById<TextView>(Resource.Id.CountStoryText);

                BackIcon = FindViewById<TextView>(Resource.Id.backicon);

                DeleteStory_Icon = FindViewById<TextView>(Resource.Id.DeleteIcon);
                IMethods.Set_TextViewIcon("1", DeleteStory_Icon, IonIcons_Fonts.TrashA);

                LoadingProgressBarview = FindViewById<ProgressBar>(Resource.Id.loadingProgressBarview);
                LoadingProgressBarview.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                MainLayout.Click += MainLayoutDisplay_Click;
                MainLayout.LongClick += MainLayoutDisplay_LongClick;
                videoView.Completion += VideoView_Completion;
                videoView.Prepared += VideoView_Prepared;
                BackIcon.Click += BackIcon_Click;
                DeleteStory_Icon.Click += DeleteStoryIconOnClick;

                AddDataStory();
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
                MainLayout.Click -= MainLayoutDisplay_Click;
                MainLayout.LongClick -= MainLayoutDisplay_LongClick;
                videoView.Completion -= VideoView_Completion;
                videoView.Prepared -= VideoView_Prepared;
                BackIcon.Click -= BackIcon_Click;
                DeleteStory_Icon.Click -= DeleteStoryIconOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Function add data story 
        public void AddDataStory()
        {
            try
            {
                ListOfStories.Clear();

                if (_Story_Data != null)
                {
                    IMethods.Set_TextViewIcon("1", BackIcon, IonIcons_Fonts.AndroidArrowBack);

                    //ImageServiceLoader.Load_Image(UserProfileImage, "no_profile_image.png", _Story_Data.user_data.avatar, 1);
                    ImageCacheLoader.LoadImage(_Story_Data.user_data.avatar, UserProfileImage, false, true);

                    User_ID = _Story_Data.user_id;
                    Story_ID = _Story_Data.id;

                    string name = IMethods.Fun_String.DecodeString(_Story_Data.user_data.name);
                    usernameText.Text = name;
                    Txt_LastSeen.Text = _Story_Data.expire;

                    //Successfully read
                    _Story_Data.Profile_indicator = AppSettings.Story_Read_Color;

                    //Set show or not icon delete 
                    DeleteStory_Icon.Visibility = _Story_Data.is_owner ? ViewStates.Visible : ViewStates.Invisible;

                    var getStory = Classes.StoryList.FirstOrDefault(a => a.Value == User_ID);
                    if (getStory.Value != null)
                    {
                        if (Classes.StoryList?.FirstOrDefault(a => a.Value == User_ID).Key?.Count == 0)
                        {
                            var ckd = Last_Stroies_Fragment.StoryAdapter?.mStoryList?.FirstOrDefault(a => a.user_id == User_ID);
                            if (ckd != null)
                            {
                                ckd.Profile_indicator = AppSettings.Story_Read_Color;
                            } 
                        }

                        count = getStory.Key.Count;

                        foreach (var story in getStory.Key)
                        {
                            string storyAbout = "";

                            if (!string.IsNullOrEmpty(_Story_Data.description))
                                storyAbout = story.description;
                            else if (!string.IsNullOrEmpty(_Story_Data.title))
                                storyAbout = story.title;

                            //image and video
                            if (!story.thumbnail.Contains("avatar"))
                            {
                                string[] data = new string[] {story.id, storyAbout};
                                ListOfStories?.Add(story.thumbnail, data);
                            }

                            if (story.images?.Count > 0)
                            {
                                for (int i = 0; i < story.images?.Count; i++)
                                {
                                    string[] data = new string[] {story.id, storyAbout};
                                    ListOfStories.Add(story.images[i].filename, data);
                                }
                            }

                            if (story.videos?.Count > 0)
                            {
                                for (int i = 0; i < story.videos?.Count; i++)
                                {
                                    string[] data = new string[] {story.id, storyAbout};
                                    ListOfStories.Add(story.videos[i].filename, data);
                                }
                            }
                        }
                    }

                    count = ListOfStories.Count;
                    CountStoryText.Text = count.ToString();
                }

                Timerstory = new Timer();
                Timerstory.Interval = 6000;
                Timerstory.Elapsed += Timerstory_Elapsed;
                Timerstory.Start();

                Timerprogress = new Timer();
                Timerprogress.Interval = 60;
                Timerprogress.Elapsed += Timerprogress_Elapsed;
                Timerprogress.Start();
                progrescount = 0;

                countstory = -1; // starts the list from the first url

                RunOnUiThread(ChangeStoryView);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Back 
        private void BackIcon_Click(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Event Delete Story
        private void DeleteStoryIconOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                //just pass file_path and type video or image
                WoWonder_API.Requests.RequestsAsync.Story.Delete_Story(Story_ID).ConfigureAwait(false);

                var getStory = Classes.StoryList.FirstOrDefault(a => a.Value == User_ID);
                if (getStory.Value != null)
                {
                    var dataStory = getStory.Key.FirstOrDefault(a => a.id == Story_ID);
                    if (dataStory != null)
                    {
                        getStory.Key.Remove(dataStory);
                    } 
                }

                if (Classes.StoryList?.FirstOrDefault(a => a.Value == User_ID).Key?.Count == 0)
                {
                    var ckd = Last_Stroies_Fragment.StoryAdapter?.mStoryList?.FirstOrDefault(a => a.user_id == User_ID);
                    Last_Stroies_Fragment.StoryAdapter?.Remove(ckd);
                }

                Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();

                Finish();
                Timerstory.Enabled = false;
                Timerstory.Stop();

                Timerprogress.Enabled = false;
                Timerprogress.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void VideoView_Prepared(object sender, EventArgs e)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    LoadingProgressBarview.Visibility = ViewStates.Gone;
                    var Max = 100;

                    StoriesProgressViewDisplay.Max = Max;
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void VideoView_Completion(object sender, EventArgs e)
        {
            try
            {
                VideoTimer.Enabled = false;
                VideoTimer.Stop();
                RunOnUiThread(ChangeStoryView);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MainLayoutDisplay_LongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                Timerstory.Enabled = false;
                Timerprogress.Enabled = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Timerprogress_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    progrescount += 1;

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        if (progrescount != 100)
                            StoriesProgressViewDisplay.SetProgress(progrescount, true);
                        else
                            progrescount = 1;
                    }
                    else
                    {
                        try
                        {
                            // For API < 24 
                            if (progrescount != 100)
                            {
                                StoriesProgressViewDisplay.Progress = progrescount;
                            }
                            else
                                progrescount = 1;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Timerstory_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (countstory == count)
                {
                    Timerstory.Stop();
                    Timerprogress.Stop();
                    Finish();
                }
                else if (countstory > count)
                {
                    Timerstory.Stop();
                    Timerprogress.Stop();
                    Finish();
                }
                else
                {
                    RunOnUiThread(ChangeStoryView);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void ChangeStoryView()
        {
            try
            {
                if (!Timerstory.Enabled)
                {
                    Timerstory.Enabled = true;
                    Timerprogress.Enabled = true;
                    StoriesProgressViewDisplay.Max = 100;
                }

                progrescount = 0;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    StoriesProgressViewDisplay.SetProgress(progrescount, true);
                }
                else
                {
                    try
                    {
                        // For API < 24 
                        StoriesProgressViewDisplay.Progress = progrescount;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }

                countstory++;
                CountStoryText.Text = (count - countstory).ToString();

                if (countstory <= count - 1)
                {
                    var dataStory = ListOfStories.FirstOrDefault();
                    if (dataStory.Key != null)
                    {
                        var type = IMethods.AttachmentFiles.Check_FileExtension(dataStory.Key);
                        if (type == "Video")
                            SetVideoStory(dataStory.Key, dataStory.Value);
                        else if (type == "Image")
                            SetImageStory(dataStory.Key, dataStory.Value);


                        ListOfStories.Remove(dataStory.Key);
                    }
                }
                else
                {
                    Finish();

                    Timerstory.Enabled = false;
                    Timerstory.Stop();

                    Timerprogress.Enabled = false;
                    Timerprogress.Stop();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MainLayoutDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Timerstory.Enabled)
                {
                    Timerstory.Enabled = true;
                    Timerprogress.Enabled = true;
                    StoriesProgressViewDisplay.Max = 100;
                }

                if (VideoTimer != null && VideoTimer.Enabled)
                {
                    LoadingProgressBarview.Visibility = ViewStates.Gone;
                    VideoTimer.Stop();
                }

                if (countstory < count)
                    ChangeStoryView();
                else
                    Finish();
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
                Timerstory.Stop();
                Timerprogress.Stop();
                Timerprogress.Close();
                Timerstory.Close();

                base.OnDestroy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetImageStory(string url, string[] data)
        {
            try
            {
                GC.Collect();

                if (imagstoryDisplay.Visibility == ViewStates.Gone)
                    imagstoryDisplay.Visibility = ViewStates.Visible;

                //Set description this story
                if (data?.Length > 0)
                {
                    Story_ID = data[0];
                    storyaboutText.Text = data[1];
                }

                if (videoView.Visibility == ViewStates.Visible)
                    videoView.Visibility = ViewStates.Gone;
                 
               

                Timerstory.Enabled = false;
                Timerprogress.Enabled = false;

                LoadingProgressBarview.Visibility = ViewStates.Visible;

                if (url.Contains("http"))
                {
                    var Filename = url.Split('/').Last();
                    var filePath = Path.Combine(IMethods.IPath.FolderDcimStory);
                    var MediaFile = filePath + "/" + Filename;

                    if (File.Exists(MediaFile))
                    {
                        Timerstory.Enabled = true;
                        Timerprogress.Enabled = true;

                        LoadingProgressBarview.Visibility = ViewStates.Gone;

                        var file = Uri.FromFile(new Java.IO.File(MediaFile));
                     

                        var ImageTrancform = ImageService.Instance.LoadFile(file.Path);
                        ImageTrancform.DownSampleMode(InterpolationMode.Medium);
                        ImageTrancform.FadeAnimation(false);
                        ImageTrancform.Retry(3, 3000);
                        ImageTrancform.WithCache(CacheType.All);
                        ImageTrancform.Into(imagstoryDisplay);
                    }
                    else
                    {
                        var WebClient = new WebClient();

                        WebClient.DownloadDataAsync(new System.Uri(url));
                        WebClient.DownloadDataCompleted += (s, e) =>
                        {
                            try
                            {
                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                File.WriteAllBytes(MediaFile, e.Result);


                              
                                var file = Uri.FromFile(new Java.IO.File(MediaFile));
                                var ImageTrancform = ImageService.Instance.LoadFile(file.Path);
                                ImageTrancform.DownSampleMode(InterpolationMode.Medium);
                                ImageTrancform.FadeAnimation(false);
                                ImageTrancform.Retry(3, 3000);
                                ImageTrancform.WithCache(CacheType.All);
                                ImageTrancform.Into(imagstoryDisplay);

                                Timerstory.Enabled = true;
                                Timerprogress.Enabled = true;

                                LoadingProgressBarview.Visibility = ViewStates.Gone;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                            var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                            mediaScanIntent.SetData(Uri.FromFile(new Java.IO.File(MediaFile)));
                            Application.Context.SendBroadcast(mediaScanIntent);
                        };
                    }
                }
                else
                {
                    Timerstory.Enabled = true;
                    Timerprogress.Enabled = true;

                    LoadingProgressBarview.Visibility = ViewStates.Gone;

                    var file = Uri.FromFile(new Java.IO.File(url));
                   

                    var ImageTrancform = ImageService.Instance.LoadFile(file.Path);
                    ImageTrancform.DownSampleMode(InterpolationMode.Medium);
                    ImageTrancform.FadeAnimation(false);
                    ImageTrancform.Retry(3, 3000);
                    ImageTrancform.WithCache(CacheType.All);
                    ImageTrancform.Into(imagstoryDisplay);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetVideoStory(string url, string[] data)
        {
            try
            {
                if (imagstoryDisplay.Visibility == ViewStates.Visible)
                    imagstoryDisplay.Visibility = ViewStates.Gone;

                if (videoView.Visibility == ViewStates.Gone)
                    videoView.Visibility = ViewStates.Visible;

                Timerstory.Enabled = false;
                Timerprogress.Enabled = false;

                LoadingProgressBarview.Visibility = ViewStates.Visible;

                if (videoView.IsPlaying)
                    videoView.Suspend();

                //Set description this story
                if (data?.Length > 0)
                {
                    Story_ID = data[0];
                    storyaboutText.Text = data[1];
                }

                if (url.Contains("http"))
                {
                    videoView.SetVideoURI(Uri.Parse(url));
                }
                else
                {
                    var file = Uri.FromFile(new Java.IO.File(url));
                    videoView.SetVideoPath(file.Path);
                }

                videoView.Start();


                VideoTimer = new Timer();
                VideoTimer.Interval = 100;
                VideoTimer.Elapsed += TimerVideo_Elapsed;
                VideoTimer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TimerVideo_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RunOnUiThread(() =>
                {
                    var ff = videoView.CurrentPosition * 100 / videoView.Duration;
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        StoriesProgressViewDisplay.SetProgress(ff, true);
                    }
                    else
                    {
                        try
                        {
                            // For API < 24 
                            StoriesProgressViewDisplay.Progress = ff;
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                ImageLoader.Instance.ClearMemoryCache();
                ImageLoader.Instance.ClearDiskCache();


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

        #region Variables Basic

        public Dictionary<string, string[]> ListOfStories = new Dictionary<string, string[]>();

        private RelativeLayout MainLayout;
        private ImageViewAsync imagstoryDisplay;
        private ImageView UserProfileImage;

        private Timer Timerstory;
        private Timer Timerprogress;
        private Timer VideoTimer = new Timer();

        private TextView storyaboutText;
        private TextView usernameText;
        private TextView Txt_LastSeen;
        private TextView BackIcon;
        private TextView DeleteStory_Icon;
        private TextView CountStoryText;
        private int countstory, progrescount;

        private ProgressBar StoriesProgressViewDisplay;
        private VideoView videoView;
        private ProgressBar LoadingProgressBarview;

        private static string User_ID;
        private static string Story_ID;

        private GetStoriesObject.Story _Story_Data;

        private int count = 1;

        #endregion
    }
}