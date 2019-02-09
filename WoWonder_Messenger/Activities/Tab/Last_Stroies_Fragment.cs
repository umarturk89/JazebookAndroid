using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Newtonsoft.Json;
using WoWonder.Activities.Story;
using WoWonder.Adapters;
using WoWonder.Functions;
using WoWonder.Helpers;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Story;
using WoWonder_API.Requests;
using Fragment = Android.Support.V4.App.Fragment;
using IMethods = WoWonder.Functions.IMethods;


namespace WoWonder.Activities.Tab
{
    public class Last_Stroies_Fragment : Fragment
    {
        #region Variables Basic

        public RecyclerView LastStoriessRecyler;
        private LinearLayoutManager mLayoutManager;
        public static Last_Stories_Adapter StoryAdapter;
        private LinearLayout LastStoriess_Empty;
        private AppCompatTextView Icon_lastStories;

        public static bool Show_Snackbar = false;
        private View view;

        #endregion

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                //Set our view from the "Last_Stories_Fragment" layout resource
                view = inflater.Inflate(Resource.Layout.Last_Stories_Fragment, container, false);

                //Get values
                LastStoriessRecyler = view.FindViewById<RecyclerView>(Resource.Id.lastStoriessRecyler);
                LastStoriess_Empty = (LinearLayout) view.FindViewById(Resource.Id.lastStoriess_LinerEmpty);
                Icon_lastStories = view.FindViewById<AppCompatTextView>(Resource.Id.lastStories_icon);

                IMethods.Set_TextViewIcon("1", Icon_lastStories, IonIcons_Fonts.IosCameraOutline);
                Icon_lastStories.SetTextColor(Android.Graphics.Color.ParseColor(AppSettings.MainColor));

                LastStoriessRecyler.Visibility = ViewStates.Visible;
                LastStoriess_Empty.Visibility = ViewStates.Gone;
                LastStoriessRecyler.SetItemAnimator(null);

                mLayoutManager = new LinearLayoutManager(this.Context);
                LastStoriessRecyler.SetLayoutManager(mLayoutManager);
                StoryAdapter = new Last_Stories_Adapter(this.Context);
                StoryAdapter.OnItemClick += StoryAdapterOnOnItemClick;
                LastStoriessRecyler.SetAdapter(StoryAdapter);

                GetStory_Api();

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();


                if (StoryAdapter.mStoryList.Count > 0)
                {
                    LastStoriessRecyler.Visibility = ViewStates.Visible;
                    LastStoriess_Empty.Visibility = ViewStates.Gone;

                    StoryAdapter.Update();
                }
                else
                {
                    LastStoriessRecyler.Visibility = ViewStates.Gone;
                    LastStoriess_Empty.Visibility = ViewStates.Visible;
                }

                if (Show_Snackbar)
                {
                    try
                    {
                        Show_Snackbar = false;
                        Toast.MakeText(this.Context, this.GetText(Resource.String.Lbl_Uploads_Story), ToastLength.Short).Show();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

       
        #region Get Story

        public async void GetStory_Api()
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                }
                else
                {
                    var (apiStatus, respond) = await WoWonder_API.Requests.RequestsAsync.Story.Get_Stories();
                    if (apiStatus == 200)
                    {
                        if (respond is GetStoriesObject result)
                        {
                            if (result.stories.Length > 0)
                            {
                                Classes.StoryList = new Dictionary<List<GetStoriesObject.Story>, string>(); // Key ListData , Value : user_id 
                                Classes.StoryList.Clear();
                                 
                                foreach (var story in result.stories)
                                {
                                    List<GetStoriesObject.Story> listOfStories = new List<GetStoriesObject.Story>();
                                    var checkUser = StoryAdapter.mStoryList.FirstOrDefault(a => a.user_id == story.user_id);
                                    if (checkUser != null)
                                    {
                                        if (Classes.StoryList == null)
                                            continue;

                                        var checkUserExits = Classes.StoryList.FirstOrDefault(a => a.Value == checkUser.user_id);
                                        if (checkUserExits.Value == null)
                                        {
                                            var ch = checkUserExits.Key?.FirstOrDefault(a => a.id == checkUser.id);
                                            if (ch == null)
                                            {
                                                listOfStories.Add(story);
                                                Classes.StoryList.Add(listOfStories, story.user_id);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var item in Classes.StoryList.Keys.ToList())
                                            {
                                                string userId = item.FirstOrDefault(a => a.user_id == checkUser.user_id)?.user_id;
                                                if (checkUserExits.Value == userId)
                                                {
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
                                        StoryAdapter.Add(story);

                                        listOfStories.Clear();

                                        if (Classes.StoryList == null)
                                            continue;

                                        var checkUserExits = Classes.StoryList.FirstOrDefault(a => a.Value == story.user_id);
                                        if (checkUserExits.Value == null)
                                        {
                                            listOfStories.Add(story);
                                            Classes.StoryList.Add(listOfStories, story.user_id);
                                        }
                                    }
                                }

                                this.Activity.RunOnUiThread(() =>
                                {
                                    StoryAdapter.BindEnd();
                                });
                            }
                        }
                    }
                    else if (apiStatus == 400)
                    {
                        if (respond is ErrorObject error)
                        {
                            var errortext = error._errors.Error_text;
                          

                            if (errortext.Contains("Invalid or expired access_token"))
                                API_Request.Logout(Activity);
                        }
                    }
                    else if (apiStatus == 404)
                    {
                        var error = respond.ToString();
                      
                    }
                }

                if (StoryAdapter.mStoryList.Count > 0)
                {
                    LastStoriessRecyler.Visibility = ViewStates.Visible;
                    LastStoriess_Empty.Visibility = ViewStates.Gone;
                }
                else
                {
                    LastStoriessRecyler.Visibility = ViewStates.Gone;
                    LastStoriess_Empty.Visibility = ViewStates.Visible;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StoryAdapterOnOnItemClick(object sender, AdapterClickEvents adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = StoryAdapter.GetItem(position);
                    if (item != null)
                    { 
                        var intent = new Intent(this.Context, typeof(View_Story_Activity));
                        intent.PutExtra("Story", JsonConvert.SerializeObject(item));
                        StartActivity(intent);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion


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