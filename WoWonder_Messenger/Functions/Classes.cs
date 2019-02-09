using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using Android.Media;
using FFImageLoading.Work;
using Newtonsoft.Json;
using WoWonder.Adapters;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Story;
using WoWonder_API.Classes.User;
using Stream = System.IO.Stream;

namespace WoWonder.Functions
{
    public class UserDetails
    {
        //############# DONT'T MODIFY HERE #############
        //Auto Session bindable objects 
        //*********************************************************
        public static string access_token = "";
        public static string User_id = "";
        public static string Username = "";
        public static string Full_name = "";
        public static string Password = "";
        public static string Email = "";
        public static string Cookie = "";
        public static string Status = "";
        public static string avatar = "";
        public static string cover = "";
        public static string Device_ID = "";
        public static string Lang = "";
        public static string Lat = "";
        public static string Lng = "";
        public static bool NotificationPopup { get; set; } = true;

        public static Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public string Time = unixTimestamp.ToString();

        public static void ClearAllValueUserDetails()
        {
            try
            {
                access_token = string.Empty;
                User_id = string.Empty;
                Username = string.Empty;
                Full_name = string.Empty;
                Password = string.Empty;
                Email = string.Empty;
                Cookie = string.Empty;
                Status = string.Empty;
                avatar = string.Empty;
                cover = string.Empty;
                Device_ID = string.Empty;
                Lang = string.Empty;
                Lat = string.Empty;
                Lng = string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class Classes
    {
        //############# DONT'T MODIFY HERE #############
        //List Items Declaration 
        //*********************************************************
        public static ObservableCollection<DataTables.LoginTB> DataUserLoginList =new ObservableCollection<DataTables.LoginTB>();
        public static ObservableCollection<GetUserDataObject.User_Data> MyProfileList =new ObservableCollection<GetUserDataObject.User_Data>();
        public static ObservableCollection<Classes.Get_Users_List_Object.User> UserList = new ObservableCollection<Classes.Get_Users_List_Object.User>();
        public static Dictionary<List<GetStoriesObject.Story>, string> StoryList =new Dictionary<List<GetStoriesObject.Story>, string>();
         
        public static void ClearAllList()
        {
            try
            {
                DataUserLoginList.Clear();
                MyProfileList.Clear();
                StoryList.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void AddRange<T>(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            try
            {
                items.ToList().ForEach(collection.Add);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        public static IEnumerable<T> TakeLast<T>(IEnumerable<T> source, int n)
        {
            var enumerable = source as T[] ?? source.ToArray();

            return enumerable.Skip(Math.Max(0, enumerable.Count() - n));
        }

        //Classes 
        //*********************************************************

        public class Get_Users_List_Object
        {
            public int ApiStatus { get; set; }
            public string ApiText { get; set; }
            public string ApiVersion { get; set; }
            public string ThemeUrl { get; set; }
            public List<User> Users { get; set; }
            public bool VideoCall { get; set; }
            public List<object> VideoCallUser { get; set; }
            public bool AudioCall { get; set; }
            public List<object> AudioCallUser { get; set; }
            public bool AgoraCall { get; set; }
            public List<object> AgoraCallData { get; set; }

            public class User
            {
                public string UserId { get; set; }
                public string Username { get; set; }
                public string Name { get; set; }
                public string ProfilePicture { get; set; }
                public string CoverPicture { get; set; }
                public string Verified { get; set; }
                public string Lastseen { get; set; }
                public string LastseenUnixTime { get; set; }
                public string LastseenTimeText { get; set; }
                public string Url { get; set; }
                public string ChatColor { get; set; }
                public LastMessage LastMessage { get; set; }
            }

            public class LastMessage
            {
                public string Id { get; set; }
                public string FromId { get; set; }
                public string GroupId { get; set; }
                public string ToId { get; set; }
                public string Text { get; set; }
                public string Media { get; set; }
                public string MediaFileName { get; set; }
                public string MediaFileNames { get; set; }
                public string Time { get; set; }
                public string Seen { get; set; }
                public string DeletedOne { get; set; }
                public string DeletedTwo { get; set; }
                public string SentPush { get; set; }
                public string NotificationId { get; set; }
                public string TypeTwo { get; set; }
                public string Stickers { get; set; }
                public string DateTime { get; set; }
            }
        }

        public class UserContacts
        {
            [JsonProperty("api_status")]
            public int ApiStatus { get; set; }
            [JsonProperty("api_text")]
            public string ApiText { get; set; }
            [JsonProperty("api_version")]
            public string ApiVersion { get; set; }
            [JsonProperty("theme_url")]
            public string ThemeUrl { get; set; }
            [JsonProperty("users")]
            public List<User> Users { get; set; }
      

            public class User
            {
                [JsonProperty("user_id")]
                public string UserId { get; set; }
                [JsonProperty("username")]
                public string Username { get; set; }
                [JsonProperty("email")]
                public string Email { get; set; }
                [JsonProperty("first_name")]
                public string FirstName { get; set; }
                [JsonProperty("last_name")]
                public string LastName { get; set; }
                [JsonProperty("avatar")]
                public string Avatar { get; set; }
                [JsonProperty("cover")]
                public string Cover { get; set; }
                [JsonProperty("relationship_id")]
                public string RelationshipId { get; set; }
                [JsonProperty("address")]
                public string Address { get; set; }
                [JsonProperty("working")]
                public string Working { get; set; }
                [JsonProperty("working_link")]
                public string WorkingLink { get; set; }
                [JsonProperty("about")]
                public string About { get; set; }
                [JsonProperty("school")]
                public string School { get; set; }
                [JsonProperty("gender")]
                public string Gender { get; set; }
                [JsonProperty("birthday")]
                public string Birthday { get; set; }
                [JsonProperty("website")]
                public string Website { get; set; }
                [JsonProperty("facebook")]
                public string Facebook { get; set; }
                [JsonProperty("google")]
                public string Google { get; set; }
                [JsonProperty("twitter")]
                public string Twitter { get; set; }
                [JsonProperty("linkedin")]
                public string Linkedin { get; set; }
                [JsonProperty("youtube")]
                public string Youtube { get; set; }
                [JsonProperty("vk")]
                public string Vk { get; set; }
                [JsonProperty("instagram")]
                public string Instagram { get; set; }
                [JsonProperty("language")]
                public string Language { get; set; }
                [JsonProperty("ip_address")]
                public string IpAddress { get; set; }
                [JsonProperty("follow_privacy")]
                public string FollowPrivacy { get; set; }
                [JsonProperty("friend_privacy")]
                public string FriendPrivacy { get; set; }
                [JsonProperty("post_privacy")]
                public string PostPrivacy { get; set; }
                [JsonProperty("message_privacy")]
                public string MessagePrivacy { get; set; }
                [JsonProperty("confirm_followers")]
                public string ConfirmFollowers { get; set; }
                [JsonProperty("show_activities_privacy")]
                public string ShowActivitiesPrivacy { get; set; }
                [JsonProperty("birth_privacy")]
                public string BirthPrivacy { get; set; }
                [JsonProperty("visit_privacy")]
                public string VisitPrivacy { get; set; }
                [JsonProperty("verified")]
                public string Verified { get; set; }
                [JsonProperty("lastseen")]
                public string Lastseen { get; set; }
                [JsonProperty("showlastseen")]
                public string Showlastseen { get; set; }
                [JsonProperty("e_sentme_msg")]
                public string SentmeMsg { get; set; }
                [JsonProperty("e_last_notif")]
                public string LastNotif { get; set; }
                [JsonProperty("notification_settings")]
                public string NotificationSettings { get; set; }
                [JsonProperty("status")]
                public string Status { get; set; }
                [JsonProperty("active")]
                public string Active { get; set; }
                [JsonProperty("admin")]
                public string Admin { get; set; }
                [JsonProperty("registered")]
                public string Registered { get; set; }
                [JsonProperty("phone_number")]
                public string PhoneNumber { get; set; }
                [JsonProperty("is_pro")]
                public string IsPro { get; set; }
                [JsonProperty("pro_type")]
                public string ProType { get; set; }
                [JsonProperty("joined")]
                public string Joined { get; set; }
                [JsonProperty("timezone")]
                public string Timezone { get; set; }
                [JsonProperty("referrer")]
                public string Referrer { get; set; }
                [JsonProperty("balance")]
                public string Balance { get; set; }
                [JsonProperty("paypal_email")]
                public string PaypalEmail { get; set; }
                [JsonProperty("notifications_sound")]
                public string NotificationsSound { get; set; }
                [JsonProperty("order_posts_by")]
                public string OrderPostsBy { get; set; }
                [JsonProperty("social_login")]
                public string SocialLogin { get; set; }
                [JsonProperty("device_id")]
                public string DeviceId { get; set; }
                [JsonProperty("web_device_id")]
                public string WebDeviceId { get; set; }
                [JsonProperty("wallet")]
                public string Wallet { get; set; }
                [JsonProperty("lat")]
                public string Lat { get; set; }
                [JsonProperty("lng")]
                public string Lng { get; set; }
                [JsonProperty("last_location_update")]
                public string LastLocationUpdate { get; set; }
                [JsonProperty("share_my_location")]
                public string ShareMyLocation { get; set; }
                [JsonProperty("last_data_update")]
                public string LastDataUpdate { get; set; }
                [JsonProperty("details")]
                public Details Details { get; set; }
                [JsonProperty("sidebar_data")]
                public string SidebarData { get; set; }
                [JsonProperty("last_avatar_mod")]
                public string LastAvatarMod { get; set; }
                [JsonProperty("last_cover_mod")]
                public string LastCoverMod { get; set; }
                [JsonProperty("points")]
                public string Points { get; set; }
                [JsonProperty("last_follow_id")]
                public string LastFollowId { get; set; }
                [JsonProperty("share_my_data")]
                public string ShareMyData { get; set; }
                [JsonProperty("last_login_data")]
                public string LastLoginData { get; set; }
                [JsonProperty("two_factor")]
                public string TwoFactor { get; set; }
                [JsonProperty("avatar_full")]
                public string AvatarFull { get; set; }
                [JsonProperty("url")]
                public string Url { get; set; }
                [JsonProperty("name")]
                public string Name { get; set; }
                [JsonProperty("following_data")]
                public List<string> FollowingData { get; set; }
                [JsonProperty("followers_data")]
                public List<string> FollowersData { get; set; }
                [JsonProperty("mutual_friends_data")]
                public List<string> MutualFriendsData { get; set; }
                [JsonProperty("likes_data")]
                public List<string> LikesData { get; set; }
                [JsonProperty("groups_data")]
                public List<string> GroupsData { get; set; }
                [JsonProperty("album_data")]
                public string AlbumData { get; set; }
                [JsonProperty("lastseen_unix_time")]
                public string LastseenUnixTime { get; set; }
                [JsonProperty("lastseen_status")]
                public string LastseenStatus { get; set; }
                [JsonProperty("family_member")]
                public string FamilyMember { get; set; }
                [JsonProperty("lastseen_time_text")]
                public string LastseenTimeText { get; set; }
                [JsonProperty("user_platform")]
                public string UserPlatform { get; set; }
                [JsonProperty("is_following")]
                public int IsFollowing { get; set; }

                public string St_ChatColor { get; set; }
            }
        }
         
        public class UserChat : UserContacts.User
        {

        }

        public class Message
        {
            //Should Not Be removed
            public MediaPlayer MediaPlayer { get; set; }
            public Timer MediaTimer { get; set; }
            public SoundViewHolder SoundViewHolder { get; set; }
            public ImageViewHolder ImageViewHolder { get; set; }

            public string M_id { get; set; } // Message id
            public string from_id { get; set; } // My user id
            public string group_id { get; set; }
            public string to_id { get; set; }
            public string text { get; set; }
            public string media { get; set; }
            public string mediaFileName { get; set; }
            public string mediaFileNames { get; set; }
            public string time { get; set; }
            public string seen { get; set; }
            public string deleted_one { get; set; }
            public string deleted_two { get; set; }
            public string sent_push { get; set; }
            public string notification_id { get; set; }
            public string type_two { get; set; }
            public string stickers { get; set; }
            public string time_text { get; set; } //CreatedAt
            public string position { get; set; }
            public string type { get; set; }
            public string file_size { get; set; }
            public string avatar { get; set; }

            //style
            public string MediaDuration { get; set; }
            public bool Media_IsPlaying { get; set; }
            public string ContactNumber { get; set; }
            public string ContactName { get; set; }
            public string ChatColor { get; set; }
        }
         
        public class Storyitems
        {
            public string Label { get; set; }
            public ImageSource Image { get; set; }
            public Stream ImageStream { get; set; }
            public string ImageFullPath { get; set; }
        }

        public class Call_User
        {
            public string video_call { get; set; }

            public string user_id { get; set; }
            public string avatar { get; set; }
            public string name { get; set; }

            //Data
            public string id { get; set; } // call_id
            public string access_token { get; set; }
            public string access_token_2 { get; set; }
            public string from_id { get; set; }
            public string to_id { get; set; }
            public string active { get; set; }
            public string called { get; set; }
            public string time { get; set; }
            public string declined { get; set; }
            public string url { get; set; }
            public string status { get; set; }
            public string room_name { get; set; }
            public string type { get; set; }

            //Style       
            public string typeIcon { get; set; }
            public string typeColor { get; set; }

        } 
    }
}