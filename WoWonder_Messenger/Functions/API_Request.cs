using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Common.Apis;
using Android.Widget;
using FFImageLoading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using UniversalImageLoader.Core;
using WoWonder.Activities;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.SQLite;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using static WoWonder_API.Requests.RequestsAsync;

namespace WoWonder.Functions
{
    public class API_Request
    {
        //############# DONT'T MODIFY HERE #############

        //========================= Variables =========================
        public static RestClient client = new RestClient();
        public static Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public string Time = unixTimestamp.ToString();
        public static bool is_Friend = false;

        //Main API URLS
        //*********************************************************
        public static string API_Get_users_friends = Client.WebsiteUrl + "/app_api.php?application=phone&type=get_user_list_info";
        public static string API_Get_users_list = Client.WebsiteUrl + "/app_api.php?application=phone&type=get_users_list";
        public static string API_Get_User_Messages = Client.WebsiteUrl + "/app_api.php?application=phone&type=get_user_messages";
        public static string API_Send_Video_Call_Answer = Client.WebsiteUrl + "/app_api.php?application=phone&type=video_call_answer";
        public static string API_Create_Video_Call_Answer = Client.WebsiteUrl + "/app_api.php?application=phone&type=create_video_call";
        public static string API_Check_For_Video_Answer = Client.WebsiteUrl + "/app_api.php?application=phone&type=check_for_answer";
        public static string API_Send_Audio_Call_Answer = Client.WebsiteUrl + "/app_api.php?application=phone&type=audio_call_answer";
        public static string API_Create_Audio_Call_Answer = Client.WebsiteUrl + "/app_api.php?application=phone&type=create_audio_call";
        public static string API_Send_Agora_Call_Action = Client.WebsiteUrl + "/app_api.php?application=phone&type=call_agora_actions";
        public static string API_Create_Agora_Call_Event = Client.WebsiteUrl + "/app_api.php?application=phone&type=create_agora_call";
        public static string API_Check_For_Agora_Answer = Client.WebsiteUrl + "/app_api.php?application=phone&type=check_agora_for_answer";
        public static string API_Get_Search_Gifs = "https://api.giphy.com/v1/gifs/search?api_key=b9427ca5441b4f599efa901f195c9f58&q=";
        //########################## Client ##########################

        public static async Task<(int, dynamic)> Get_users_friends_Async(string Aftercontact)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("user_profile_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                        new KeyValuePair<string, string>("after_user_id", Aftercontact),
                        new KeyValuePair<string, string>("list_type", "all"),
                    });

                    var response = await client.PostAsync(API_Get_users_friends, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Classes.UserContacts>(json);
                    if (data.ApiStatus == 200)
                    {
                        if (data.Users.Count > 0)
                        {
                            if (is_Friend)
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                //Insert Or Replace To Database
                                dbDatabase.Insert_Or_Replace_MyContactTable(new ObservableCollection<Classes.UserContacts.User>(data.Users));
                                dbDatabase.Dispose();

                                is_Friend = false;
                            }
                        }

                        return (200, data);
                    }
                    else
                    {
                        var error = JsonConvert.DeserializeObject<ErrorObject>(json);
                        return (404, error);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error : " + e.Message);
                Console.WriteLine(e);
                return (400, e.Message);
            }
        }

        public static async Task<string> Get_users_list_Async()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("user_profile_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                        new KeyValuePair<string, string>("list_type", "all")
                    });

                    var response = await client.PostAsync(API_Get_users_list, formContent);
                  
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        return json; // return json;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #region Messages

        public static async Task<JArray> Get_User_Messages_Async(string recipient_id, string before_message_id,
            string LastMessage_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("recipient_id", recipient_id),
                        new KeyValuePair<string, string>("before_message_id", before_message_id),
                        new KeyValuePair<string, string>("after_message_id", LastMessage_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Get_User_Messages, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        var messages = JObject.Parse(json).SelectToken("messages").ToString();
                        JArray ChatMessages = JArray.Parse(messages);
                        return ChatMessages;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<JArray> Messages_Loadmore_Async(string recipient_id, string before_message_id) /// ////////******//////
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("recipient_id", recipient_id),
                        new KeyValuePair<string, string>("before_message_id", before_message_id),
                        new KeyValuePair<string, string>("after_message_id", "0"),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Get_User_Messages, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        var messages = JObject.Parse(json).SelectToken("messages").ToString();
                        JArray ChatMessages = JArray.Parse(messages);
                        return ChatMessages;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<string> Updater_Messages_Async(string recipient_id, string before_message_id, string LastMessage_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("recipient_id", recipient_id),
                        new KeyValuePair<string, string>("before_message_id", before_message_id),
                        new KeyValuePair<string, string>("after_message_id", LastMessage_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Get_User_Messages, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        return json;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Call

        public static async Task<string> Send_Twilio_Video_Call_Answer_Async(string Answer_Type, string call_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("answer_type", Answer_Type),
                        new KeyValuePair<string, string>("call_id", call_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Send_Video_Call_Answer, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["status"].ToString();
                    if (apiStatus == "200")
                    {
                        return !string.IsNullOrEmpty(data["url"].ToString()) ? data["url"].ToString() : "";
                    }
                    else
                    {
                        return "Decline";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<Classes.Call_User> Create_Twilio_Video_Call_Answer_Async(string recipient_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("recipient_id", recipient_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Create_Video_Call_Answer, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["status"].ToString();
                    if (apiStatus == "200")
                    {
                        Classes.Call_User videoData = new Classes.Call_User
                        {
                            access_token = data["access_token"].ToString(),
                            access_token_2 = data["access_token_2"].ToString(),
                            id = data["id"].ToString(),
                            room_name = data["room_name"].ToString(),
                            type = "Twilio_video_call"
                        };

                        return videoData;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<string> Check_Twilio_Call_Answer_Async(string Call_id, string Call_type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("call_id", Call_id),
                        new KeyValuePair<string, string>("call_type", Call_type),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Check_For_Video_Answer, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        string callstatus = data["call_status"].ToString();
                        return callstatus;
                    }
                    else
                    {
                        return "400";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        public static async Task<string> Send_Twilio_Audio_Call_Answer_Async(string Answer_Type, string call_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("answer_type", Answer_Type),
                        new KeyValuePair<string, string>("call_id", call_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Send_Audio_Call_Answer, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["status"].ToString();
                    if (apiStatus == "200")
                    {
                        string UrlVideo = data["url"].ToString();
                        return UrlVideo;
                    }
                    else
                    {
                        return "Decline";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<Classes.Call_User> Create_Twilio_Audio_Call_Answer_Async(string recipient_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("recipient_id", recipient_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Create_Audio_Call_Answer, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["status"].ToString();
                    if (apiStatus == "200")
                    {
                        Classes.Call_User videoData = new Classes.Call_User
                        {
                            access_token = data["access_token"].ToString(),
                            access_token_2 = data["access_token_2"].ToString(),
                            id = data["id"].ToString(),
                            room_name = data["room_name"].ToString(),
                            type = "Twilio_audio_call"
                        };
                        return videoData;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// ###############################
        /// Agora framework Api 

        public static async Task<string> Send_Agora_Call_Action_Async(string Answer_Type, string call_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("answer_type", Answer_Type),
                        new KeyValuePair<string, string>("call_id", call_id),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Send_Agora_Call_Action, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["status"].ToString();
                    return apiStatus;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<string> Check_Agora_Call_Answer_Async(string Call_id, string Call_type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("call_id", Call_id),
                        new KeyValuePair<string, string>("call_type", Call_type),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Check_For_Agora_Answer, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["api_status"].ToString();
                    if (apiStatus == "200")
                    {
                        string callstatus = data["call_status"].ToString();
                        return callstatus;
                    }
                    else
                    {
                        return "400";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task<Classes.Call_User> Create_Agora_Call_Event_Async(string recipient_id, string call_type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("user_id", UserDetails.User_id),
                        new KeyValuePair<string, string>("recipient_id", recipient_id),
                        new KeyValuePair<string, string>("call_type", call_type),
                        new KeyValuePair<string, string>("s", UserDetails.access_token),
                    });

                    var response = await client.PostAsync(API_Create_Agora_Call_Event, formContent);
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    string apiStatus = data["status"].ToString();
                    if (apiStatus == "200")
                    {
                        Classes.Call_User vidodata = new Classes.Call_User();

                        if (data.ContainsKey("id"))
                            vidodata.id = data["id"].ToString();
                        if (data.ContainsKey("room_name"))
                            vidodata.room_name = data["room_name"].ToString();

                        vidodata.type = call_type;

                        return vidodata;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// ###############################

        #endregion

        public static async Task<ObservableCollection<GifGiphyClass.Datum>> Search_Gifs_Web(string search_key, string offset)
        {
            try
            {
                if (!IMethods.CheckConnectivity())
                {
                    Toast.MakeText(Application.Context, Application.Context.GetString(Resource.String.Lbl_Error_check_internet_connection), ToastLength.Short).Show();
                    return null;
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(API_Get_Search_Gifs + search_key);
                        response.EnsureSuccessStatusCode();
                        string json = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<GifGiphyClass.RootObject>(json);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            if (data.meta.status == 200)
                            {
                                return new ObservableCollection<GifGiphyClass.Datum>(data.data);
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static async Task Get_MyProfileData_Api(Activity context)
        {
            try
            {
                if (IMethods.CheckConnectivity())
                {
                    (int api_status, var respond) = await Global.Get_User_Data(UserDetails.User_id);
                    if (api_status == 200)
                    {
                        if (respond is GetUserDataObject result)
                        {
                            context.RunOnUiThread(() =>
                            {
                                SqLiteDatabase dbDatabase = new SqLiteDatabase();

                                // user_data
                                if (result.user_data != null)
                                {
                                    //Insert Or Update All data user_data
                                    dbDatabase.Insert_Or_Update_To_MyProfileTable(result.user_data);

                                    if (result.following.Length > 0)
                                    {
                                        var list = result.following.Select(user => new Classes.UserContacts.User()
                                        {
                                            UserId = user.user_id,
                                            Username = user.username,
                                            Email = user.email,
                                            FirstName = user.first_name,
                                            LastName = user.last_name,
                                            Avatar = user.avatar,
                                            Cover = user.cover,
                                            RelationshipId = user.relationship_id,
                                    
                                            Address = user.address,
                                            Working = user.working,
                                            WorkingLink = user.working_link,
                                            About = user.about,
                                            School = user.school,
                                            Gender = user.gender,
                                            Birthday = user.birthday,
                                            Website = user.website,
                                            Facebook = user.facebook,
                                            Google = user.google,
                                            Twitter = user.twitter,
                                            Linkedin = user.linkedin,
                                            Youtube = user.youtube,
                                            Vk = user.vk,
                                            Instagram = user.instagram,
                                            Language = user.language,
                                            IpAddress = user.ip_address,
                                            FollowPrivacy = user.follow_privacy,
                                            FriendPrivacy = user.friend_privacy,
                                            PostPrivacy = user.post_privacy,
                                            MessagePrivacy = user.message_privacy,
                                            ConfirmFollowers = user.confirm_followers,
                                            ShowActivitiesPrivacy = user.show_activities_privacy,
                                            BirthPrivacy = user.birth_privacy,
                                            VisitPrivacy = user.visit_privacy,
                                            Lastseen = user.lastseen,
                                            Showlastseen = user.showlastseen,
                                            SentmeMsg = user.e_sentme_msg,
                                            LastNotif = user.e_last_notif,
                                            Status = user.status,
                                            Active = user.active,
                                            Admin = user.admin,
                                            Registered = user.registered,
                                            PhoneNumber = user.phone_number,
                                            IsPro = user.is_pro,
                                            ProType = user.pro_type,
                                            Joined = user.joined,
                                            Timezone = user.timezone,
                                            Referrer = user.referrer,
                                            Balance = user.balance,
                                            PaypalEmail = user.paypal_email,
                                            NotificationsSound = user.notifications_sound,
                                            OrderPostsBy = user.order_posts_by,
                                            SocialLogin = user.social_login,
                                            DeviceId = user.device_id,
                                            WebDeviceId = user.web_device_id,
                                            Wallet = user.wallet,
                                            Lat = user.lat,
                                            Lng = user.lng,
                                            LastDataUpdate = user.last_location_update,
                                            ShareMyLocation = user.share_my_location,
                                            Url = user.url,
                                            Name = user.name,
                                            LastseenUnixTime = user.lastseen_unix_time,
                                     
                                            Details = new Details()
                                            {
                                                post_count = user.details.post_count,
                                                album_count = user.details.album_count,
                                                following_count = user.details.following_count,
                                                followers_count = user.details.followers_count,
                                                groups_count = user.details.groups_count,
                                                likes_count = user.details.likes_count,
                                            },
                                        }).ToList();

                                        //Insert Or Update All data Groups
                                        dbDatabase.Insert_Or_Replace_MyContactTable(new ObservableCollection<Classes.UserContacts.User>(list));
                                    }

                                    dbDatabase.Dispose();
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Get_MyProfileData_Api(context).ConfigureAwait(false);
            }
        }

        public static bool RunLogout = false;

        public static async void Logout(Activity context)
        {
            try
            {
                if (RunLogout == false)
                {
                    RunLogout = true;

                    await RemoveData("Logout");

                    context.RunOnUiThread(async () =>
                    {
                        IMethods.IPath.DeleteAll_MyFolderDisk();

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.ClearAll();
                        dbDatabase.DropAll();

                        

                  
                        Last_Messages_Fragment.mAdapter.Clear();
                    

                        Java.Lang.Runtime.GetRuntime().RunFinalization();
                        Java.Lang.Runtime.GetRuntime().Gc();
                        TrimCache(context);

                        var ss = await dbDatabase.CheckTablesStatus();
                        dbDatabase.Dispose();

                        Intent intent = new Intent(context, typeof(MainActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        context.StartActivity(intent);
                        context.FinishAffinity();
                    });

                    Wo_Main_Settings.Shared_Data.Edit().Clear().Commit();

                    RunLogout = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async void Delete(Activity context)
        {
            try
            {
                if (RunLogout == false)
                {
                    RunLogout = true;

                    await RemoveData("Delete");

                    context.RunOnUiThread(async () =>
                    {
                        IMethods.IPath.DeleteAll_MyFolderDisk();

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.ClearAll();
                        dbDatabase.DropAll();

                        Classes.ClearAllList();

                        Java.Lang.Runtime.GetRuntime().RunFinalization();
                        Java.Lang.Runtime.GetRuntime().Gc();
                        TrimCache(context);

                        var ss = await dbDatabase.CheckTablesStatus();
                        dbDatabase.Dispose();

                        Intent intent = new Intent(context, typeof(MainActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        context.StartActivity(intent);
                        context.FinishAffinity();
                    });

                    Wo_Main_Settings.Shared_Data.Edit().Clear().Commit();

                    RunLogout = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void TrimCache(Activity context)
        {
            try
            {
                Java.IO.File dir = context.CacheDir;
                if (dir != null && dir.IsDirectory)
                {
                    DeleteDir(dir);
                }

                context.DeleteDatabase(SqLiteDatabase.PathCombine);
            }
            catch (Exception e)
            {
               
                Console.WriteLine(e);
            }
        }

        public static bool DeleteDir(Java.IO.File dir)
        {
            try
            {
                if (dir != null && dir.IsDirectory)
                {
                    string[] children = dir.List();
                    foreach (string child in children)
                    {
                        bool success = DeleteDir(new Java.IO.File(dir, child));
                        if (!success)
                        {
                            return false;
                        }
                    }
                }

                // The directory is now empty so delete it
                return dir.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static async Task RemoveData(string type)
        {
            try
            {
                if (type == "Logout")
                {
                    if (IMethods.CheckConnectivity())
                    {
                        var data = await Global.Get_Delete_Token();
                    }
                }
                else if (type == "Delete")
                {
                    IMethods.IPath.DeleteAll_MyFolder();

                    if (IMethods.CheckConnectivity())
                    {
                        var data = await Global.Delete_User(UserDetails.Password);
                    }
                }

                try
                {
                    if (MainActivity.mGoogleApiClient != null)
                    {
                        await Auth.GoogleSignInApi.SignOut(MainActivity.mGoogleApiClient);
                    }

                    var accessToken = AccessToken.CurrentAccessToken;
                    var isLoggedIn = accessToken != null && !accessToken.IsExpired;
                    if (isLoggedIn && Profile.CurrentProfile != null)
                    {
                        LoginManager.Instance.LogOut();
                    }

                    UserDetails.ClearAllValueUserDetails();

                    ImageLoader.Instance.ClearMemoryCache();
                    ImageLoader.Instance.ClearDiskCache();

                    ImageService.Instance.InvalidateMemoryCache();

                    GC.Collect();

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}