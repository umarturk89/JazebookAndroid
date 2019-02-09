using SQLite;

namespace WoWonder.SQLite
{
    public class DataTables
    {
        public class LoginTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDLogin { get; set; }

            public string UserID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string access_token { get; set; }
            public string Cookie { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public string Lang { get; set; }
        }

        public class LastUsersTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDLastUsers { get; set; }

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


            //Last_Message
            public string MId { get; set; }
            public string MFromId { get; set; }
            public string MGroupId { get; set; }
            public string MToId { get; set; }
            public string MText { get; set; }
            public string MMedia { get; set; }
            public string MMediaFileName { get; set; }
            public string MMediaFileNames { get; set; }
            public string MTime { get; set; }
            public string MSeen { get; set; }
            public string MDeletedOne { get; set; }
            public string MDeletedTwo { get; set; }
            public string MSentPush { get; set; }
            public string MNotificationId { get; set; }
            public string MTypeTwo { get; set; }
            public string MStickers { get; set; }
            public string MDateTime { get; set; }

        }

        public class MessageTB
        {
            [PrimaryKey, AutoIncrement] public int ID { get; set; }

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

        public class Call_UserTB
        {
            [PrimaryKey, AutoIncrement] public int ID { get; set; }

            public string video_call { get; set; }

            public string user_id { get; set; }
            public string avatar { get; set; }
            public string name { get; set; }

            //Data
            public string callID { get; set; }
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

        public class MyContactsTB
        {
            [PrimaryKey, AutoIncrement] public int ID { get; set; }

            public string UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Avatar { get; set; }
            public string Cover { get; set; }
            public string RelationshipId { get; set; }
            public string Address { get; set; }
            public string Working { get; set; }
            public string WorkingLink { get; set; }
            public string About { get; set; }
            public string School { get; set; }
            public string Gender { get; set; }
            public string Birthday { get; set; }
            public string Website { get; set; }
            public string Facebook { get; set; }
            public string Google { get; set; }
            public string Twitter { get; set; }
            public string Linkedin { get; set; }
            public string Youtube { get; set; }
            public string Vk { get; set; }
            public string Instagram { get; set; }
            public string Language { get; set; }
            public string IpAddress { get; set; }
            public string FollowPrivacy { get; set; }
            public string FriendPrivacy { get; set; }
            public string PostPrivacy { get; set; }
            public string MessagePrivacy { get; set; }
            public string ConfirmFollowers { get; set; }
            public string ShowActivitiesPrivacy { get; set; }
            public string BirthPrivacy { get; set; }
            public string VisitPrivacy { get; set; }
            public string Verified { get; set; }
            public string Lastseen { get; set; }
            public string Showlastseen { get; set; }
            public string ESentmeMsg { get; set; }
            public string ELastNotif { get; set; }
            public string NotificationSettings { get; set; }
            public string Status { get; set; }
            public string Active { get; set; }
            public string Admin { get; set; }
            public string Registered { get; set; }
            public string PhoneNumber { get; set; }
            public string IsPro { get; set; }
            public string ProType { get; set; }
            public string Joined { get; set; }
            public string Timezone { get; set; }
            public string Referrer { get; set; }
            public string Balance { get; set; }
            public string PaypalEmail { get; set; }
            public string NotificationsSound { get; set; }
            public string OrderPostsBy { get; set; }
            public string SocialLogin { get; set; }
            public string DeviceId { get; set; }
            public string WebDeviceId { get; set; }
            public string Wallet { get; set; }
            public string Lat { get; set; }
            public string Lng { get; set; }
            public string LastLocationUpdate { get; set; }
            public string ShareMyLocation { get; set; }
            public string LastDataUpdate { get; set; }
            public string SidebarData { get; set; }
            public string LastAvatarMod { get; set; }
            public string LastCoverMod { get; set; }
            public string Points { get; set; }
            public string LastFollowId { get; set; }
            public string ShareMyData { get; set; }
            public string LastLoginData { get; set; }
            public string TwoFactor { get; set; }
            public string AvatarFull { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
            public string MutualFriendsData { get; set; }
            public string LikesData { get; set; }
            public string GroupsData { get; set; }
            public string AlbumData { get; set; }
            public string LastseenUnixTime { get; set; }
            public string LastseenStatus { get; set; }
            public string FamilyMember { get; set; }
            public string LastseenTimeText { get; set; }
            public string UserPlatform { get; set; }
            public int IsFollowing { get; set; }

            //Details
            public string de_post_count { get; set; }
            public string de_album_count { get; set; }
            public string de_following_count { get; set; }
            public string de_followers_count { get; set; }
            public string de_groups_count { get; set; }
            public string de_likes_count { get; set; }

            public string St_ChatColor { get; set; } 
        }

        public class BlockedUsersTB
        {
            [PrimaryKey, AutoIncrement] public int AutoITBlockedUsers { get; set; }

            public string user_id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string avatar { get; set; }
            public string cover { get; set; }
            public string background_image { get; set; }
            public string relationship_id { get; set; }
            public string address { get; set; }
            public string working { get; set; }
            public string working_link { get; set; }
            public string about { get; set; }
            public string school { get; set; }
            public string gender { get; set; }
            public string birthday { get; set; }
            public string country_id { get; set; }
            public string website { get; set; }
            public string facebook { get; set; }
            public string google { get; set; }
            public string twitter { get; set; }
            public string linkedin { get; set; }
            public string youtube { get; set; }
            public string vk { get; set; }
            public string instagram { get; set; }
            public string language { get; set; }
            public string ip_address { get; set; }
            public string follow_privacy { get; set; }
            public string friend_privacy { get; set; }
            public string post_privacy { get; set; }
            public string message_privacy { get; set; }
            public string confirm_followers { get; set; }
            public string show_activities_privacy { get; set; }
            public string birth_privacy { get; set; }
            public string visit_privacy { get; set; }
            public string verified { get; set; }
            public string lastseen { get; set; }
            public string emailNotification { get; set; }
            public string e_liked { get; set; }
            public string e_wondered { get; set; }
            public string e_shared { get; set; }
            public string e_followed { get; set; }
            public string e_commented { get; set; }
            public string e_visited { get; set; }
            public string e_liked_page { get; set; }
            public string e_mentioned { get; set; }
            public string e_joined_group { get; set; }
            public string e_accepted { get; set; }
            public string e_profile_wall_post { get; set; }
            public string e_sentme_msg { get; set; }
            public string e_last_notif { get; set; }
            public string status { get; set; }
            public string active { get; set; }
            public string admin { get; set; }
            public string registered { get; set; }
            public string phone_number { get; set; }
            public string is_pro { get; set; }
            public string pro_type { get; set; }
            public string timezone { get; set; }
            public string referrer { get; set; }
            public string balance { get; set; }
            public string paypal_email { get; set; }
            public string notifications_sound { get; set; }
            public string order_posts_by { get; set; }
            public string device_id { get; set; }
            public string web_device_id { get; set; }
            public string wallet { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string last_location_update { get; set; }
            public string share_my_location { get; set; }
            public string last_data_update { get; set; }
            public string last_avatar_mod { get; set; }
            public string last_cover_mod { get; set; }
            public string avatar_full { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string lastseen_unix_time { get; set; }

            public string lastseen_status { get; set; }

            //Details
            public string de_post_count { get; set; }
            public string de_album_count { get; set; }
            public string de_following_count { get; set; }
            public string de_followers_count { get; set; }
            public string de_groups_count { get; set; }
            public string de_likes_count { get; set; }
        }

        public class MyProfileTB
        {
            [PrimaryKey, AutoIncrement] public int AutoIDMyProfile { get; set; }

            public string user_id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string avatar { get; set; }
            public string cover { get; set; }
            public string background_image { get; set; }
            public string relationship_id { get; set; }
            public string address { get; set; }
            public string working { get; set; }
            public string working_link { get; set; }
            public string about { get; set; }
            public string school { get; set; }
            public string gender { get; set; }
            public string birthday { get; set; }
            public string country_id { get; set; }
            public string website { get; set; }
            public string facebook { get; set; }
            public string google { get; set; }
            public string twitter { get; set; }
            public string linkedin { get; set; }
            public string youtube { get; set; }
            public string vk { get; set; }
            public string instagram { get; set; }
            public string language { get; set; }
            public string ip_address { get; set; }
            public string follow_privacy { get; set; }
            public string friend_privacy { get; set; }
            public string post_privacy { get; set; }
            public string message_privacy { get; set; }
            public string confirm_followers { get; set; }
            public string show_activities_privacy { get; set; }
            public string birth_privacy { get; set; }
            public string visit_privacy { get; set; }
            public string verified { get; set; }
            public string lastseen { get; set; }
            public string emailNotification { get; set; }
            public string e_liked { get; set; }
            public string e_wondered { get; set; }
            public string e_shared { get; set; }
            public string e_followed { get; set; }
            public string e_commented { get; set; }
            public string e_visited { get; set; }
            public string e_liked_page { get; set; }
            public string e_mentioned { get; set; }
            public string e_joined_group { get; set; }
            public string e_accepted { get; set; }
            public string e_profile_wall_post { get; set; }
            public string e_sentme_msg { get; set; }
            public string e_last_notif { get; set; }
            public string status { get; set; }
            public string active { get; set; }
            public string admin { get; set; }
            public string registered { get; set; }
            public string phone_number { get; set; }
            public string is_pro { get; set; }
            public string pro_type { get; set; }
            public string timezone { get; set; }
            public string referrer { get; set; }
            public string balance { get; set; }
            public string paypal_email { get; set; }
            public string notifications_sound { get; set; }
            public string order_posts_by { get; set; }
            public string device_id { get; set; }
            public string web_device_id { get; set; }
            public string wallet { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string last_location_update { get; set; }
            public string share_my_location { get; set; }
            public string last_data_update { get; set; }
            public string last_avatar_mod { get; set; }
            public string last_cover_mod { get; set; }
            public string avatar_full { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string lastseen_unix_time { get; set; }
            public string lastseen_status { get; set; }
            public string is_following { get; set; }
            public string can_follow { get; set; }
            public string is_following_me { get; set; }
            public string gender_text { get; set; }
            public string lastseen_time_text { get; set; }

            public string is_blocked { get; set; }

            //Details
            public string de_post_count { get; set; }
            public string de_album_count { get; set; }
            public string de_following_count { get; set; }
            public string de_followers_count { get; set; }
            public string de_groups_count { get; set; }
            public string de_likes_count { get; set; }
        }

        public class SearchFilterTB
        {

            [PrimaryKey, AutoIncrement] public int AutoIDNearByFilter { get; set; }

            public string UserId { get; set; }
            public int Gender { get; set; }
            public int ProfilePicture { get; set; }
            public int Status { get; set; }
        }
    } 
}
