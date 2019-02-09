using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using SQLite;
using WoWonder.Activities.ChatWindow;
using WoWonder.Functions;
using WoWonder_API;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.User;
using Exception = System.Exception;

namespace WoWonder.SQLite
{
    public class SqLiteDatabase : IDisposable
    {
        //############# DON'T MODIFY HERE #############
        public static string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static string PathCombine = Path.Combine(Folder, "WoWonderMessenger.db");
        public SQLiteConnection Connection;

        //Open Connection in Database
        //*********************************************************

        #region Connection

        public void OpenConnection()
        {
            try
            {
                Connection = new SQLiteConnection(PathCombine);
                string path = Connection.DatabasePath;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<bool> CheckTablesStatus()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    OpenConnection();

                    Connection?.CreateTable<DataTables.LoginTB>();
                    Connection?.CreateTable<DataTables.LastUsersTB>();
                    Connection?.CreateTable<DataTables.MessageTB>();
                    Connection?.CreateTable<DataTables.Call_UserTB>();
                    Connection?.CreateTable<DataTables.MyContactsTB>();
                    Connection?.CreateTable<DataTables.BlockedUsersTB>();
                    Connection?.CreateTable<DataTables.MyProfileTB>();
                    Connection?.CreateTable<DataTables.SearchFilterTB>();
                    Connection?.Dispose();
                    Connection?.Close();

                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
              
                return false;
            }
        }

        //Close Connection in Database
        public void Dispose()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection?.Dispose();
                    Connection?.Close();
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }
        }

        public void DeleteDatabase()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Folder);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
              
            }
        }

        public void ClearAll()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection?.DeleteAll<DataTables.LoginTB>();
                    Connection?.DeleteAll<DataTables.LastUsersTB>();
                    Connection?.DeleteAll<DataTables.MessageTB>();
                    Connection?.DeleteAll<DataTables.Call_UserTB>();
                    Connection?.DeleteAll<DataTables.MyContactsTB>();
                    Connection?.DeleteAll<DataTables.BlockedUsersTB>();
                    Connection?.DeleteAll<DataTables.MyProfileTB>();
                    Connection?.DeleteAll<DataTables.SearchFilterTB>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
             
            }
        }

        //Delete table 
        public void DropAll()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection?.DropTable<DataTables.LoginTB>();
                    Connection?.DropTable<DataTables.LastUsersTB>();
                    Connection?.DropTable<DataTables.MessageTB>();
                    Connection?.DropTable<DataTables.Call_UserTB>();
                    Connection?.DropTable<DataTables.MyContactsTB>();
                    Connection?.DropTable<DataTables.BlockedUsersTB>();
                    Connection?.DropTable<DataTables.MyProfileTB>();
                    Connection?.DropTable<DataTables.SearchFilterTB>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
             
            }
        }

        #endregion

        //########################## End SQLite_Entity ##########################

        //Start SQL_Commander >>  General 
        //*********************************************************

        #region General

        public void InsertRow(object row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.Insert(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void UpdateRow(object row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.Update(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void DeleteRow(object row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.Delete(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void InsertListOfRows(List<object> row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.InsertAll(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //Start SQL_Commander >>  Custom 
        //*********************************************************

        #region Login

        //Get data Login
        public DataTables.LoginTB Get_data_Login_Credentials()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var dataUser = Connection.Table<DataTables.LoginTB>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        UserDetails.Username = dataUser.Username;
                        UserDetails.Full_name = dataUser.Username;
                        UserDetails.Password = dataUser.Password;
                        UserDetails.access_token = dataUser.access_token;
                        UserDetails.User_id = dataUser.UserID;
                        UserDetails.Status = dataUser.Status;
                        UserDetails.Cookie = dataUser.Cookie;
                        UserDetails.Email = dataUser.Email;
                        AppSettings.Lang = dataUser.Lang;
                        Current.AccessToken = dataUser.access_token;
                        Classes.DataUserLoginList.Add(dataUser);

                        return dataUser;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        //Delete data Login
        public void Delete_Login_Credentials()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var data = Connection.Table<DataTables.LoginTB>().FirstOrDefault(c => c.Status == "Active");
                    if (data != null)
                    {
                        Connection.Delete(data);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Clear All data Login
        public void Clear_Login_Credentials()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.DeleteAll<DataTables.LoginTB>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region My Contacts >> Following

        //Insert data To My Contact Table
        public void Insert_Or_Replace_MyContactTable(ObservableCollection<Classes.UserContacts.User> usersContactList)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.MyContactsTB>().ToList();
                    var list = usersContactList.Select(user => new DataTables.MyContactsTB
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Avatar = user.Avatar,
                        Cover = user.Cover,
                        RelationshipId = user.RelationshipId,
                        LastseenTimeText = user.LastseenTimeText,
                        Address = user.Address,
                        Working = user.Working,
                        WorkingLink = user.WorkingLink,
                        About = user.About,
                        School = user.School,
                        Gender = user.Gender,
                        Birthday = user.Birthday,
                        Website = user.Website,
                        Facebook = user.Facebook,
                        Google = user.Google,
                        Twitter = user.Twitter,
                        Linkedin = user.Linkedin,
                        Youtube = user.Youtube,
                        Vk = user.Vk,
                        Instagram = user.Instagram,
                        Language = user.Language,
                        IpAddress = user.IpAddress,
                        FollowPrivacy = user.FollowPrivacy,
                        FriendPrivacy = user.FriendPrivacy,
                        PostPrivacy = user.PostPrivacy,
                        MessagePrivacy = user.MessagePrivacy,
                        ConfirmFollowers = user.ConfirmFollowers,
                        ShowActivitiesPrivacy = user.ShowActivitiesPrivacy,
                        BirthPrivacy = user.BirthPrivacy,
                        VisitPrivacy = user.VisitPrivacy,
                        Lastseen = user.Lastseen,
                        Showlastseen = user.Showlastseen,
                        ESentmeMsg = user.SentmeMsg,
                        ELastNotif = user.LastNotif,
                        Status = user.Status,
                        Active = user.Active,
                        Admin = user.Admin,
                        Registered = user.Registered,
                        PhoneNumber = user.PhoneNumber,
                        IsPro = user.IsPro,
                        ProType = user.ProType,
                        Joined = user.Joined,
                        Timezone = user.Timezone,
                        Referrer = user.Referrer,
                        Balance = user.Balance,
                        PaypalEmail = user.PaypalEmail,
                        NotificationsSound = user.NotificationsSound,
                        OrderPostsBy = user.OrderPostsBy,
                        SocialLogin = user.SocialLogin,
                        DeviceId = user.DeviceId,
                        WebDeviceId = user.WebDeviceId,
                        Wallet = user.Wallet,
                        Lat = user.Lat,
                        Lng = user.Lng,
                        LastDataUpdate = user.LastDataUpdate,
                        ShareMyLocation = user.ShareMyLocation,
                        Url = user.Url,
                        Name = user.Name,
                        LastseenUnixTime = user.LastseenUnixTime,
                        UserPlatform = user.UserPlatform,
                        IsFollowing = user.IsFollowing,
                        de_post_count = user.Details.post_count,
                        de_album_count = user.Details.album_count,
                        de_following_count = user.Details.following_count,
                        de_followers_count = user.Details.followers_count,
                        de_groups_count = user.Details.groups_count,
                        de_likes_count = user.Details.likes_count,
                        St_ChatColor = user.St_ChatColor ?? AppSettings.MainColor,
                    }).ToList();

                    if (list.Count > 0)
                    {
                        Connection.BeginTransaction();
                        //Bring new  
                        var newItemList = list.Where(c => !result.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                        if (newItemList.Count > 0)
                        {
                            Connection.InsertAll(newItemList);
                        }

                        var deleteItemList = result.Where(c => !list.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                        if (deleteItemList.Count > 0)
                        {
                            foreach (var delete in deleteItemList)
                            {
                                Connection.Delete(delete);
                            }
                        }
                        Connection.UpdateAll(list);
                        Connection.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To My Contact Table
        public ObservableCollection<Classes.UserContacts.User> Get_MyContact(int id, int nSize)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var query = Connection.Table<DataTables.MyContactsTB>().Where(w => w.ID >= id).OrderBy(q => q.ID).Take(nSize).ToList();
                    if (query.Count > 0)
                    {
                        var list = query.Select(user => new Classes.UserContacts.User
                        {

                            UserId = user.UserId,
                            Username = user.Username,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Avatar = user.Avatar,
                            Cover = user.Cover,
                            RelationshipId = user.RelationshipId,
                            LastseenTimeText = user.LastseenTimeText,
                            Address = user.Address,
                            Working = user.Working,
                            WorkingLink = user.WorkingLink,
                            About = user.About,
                            School = user.School,
                            Gender = user.Gender,
                            Birthday = user.Birthday,
                            Website = user.Website,
                            Facebook = user.Facebook,
                            Google = user.Google,
                            Twitter = user.Twitter,
                            Linkedin = user.Linkedin,
                            Youtube = user.Youtube,
                            Vk = user.Vk,
                            Instagram = user.Instagram,
                            Language = user.Language,
                            IpAddress = user.IpAddress,
                            FollowPrivacy = user.FollowPrivacy,
                            FriendPrivacy = user.FriendPrivacy,
                            PostPrivacy = user.PostPrivacy,
                            MessagePrivacy = user.MessagePrivacy,
                            ConfirmFollowers = user.ConfirmFollowers,
                            ShowActivitiesPrivacy = user.ShowActivitiesPrivacy,
                            BirthPrivacy = user.BirthPrivacy,
                            VisitPrivacy = user.VisitPrivacy,
                            Lastseen = user.Lastseen,
                            Showlastseen = user.Showlastseen,
                            SentmeMsg = user.ESentmeMsg,
                            LastNotif = user.ELastNotif,
                            Status = user.Status,
                            Active = user.Active,
                            Admin = user.Admin,
                            Registered = user.Registered,
                            PhoneNumber = user.PhoneNumber,
                            IsPro = user.IsPro,
                            ProType = user.ProType,
                            Joined = user.Joined,
                            Timezone = user.Timezone,
                            Referrer = user.Referrer,
                            Balance = user.Balance,
                            PaypalEmail = user.PaypalEmail,
                            NotificationsSound = user.NotificationsSound,
                            OrderPostsBy = user.OrderPostsBy,
                            SocialLogin = user.SocialLogin,
                            DeviceId = user.DeviceId,
                            WebDeviceId = user.WebDeviceId,
                            Wallet = user.Wallet,
                            Lat = user.Lat,
                            Lng = user.Lng,
                            LastDataUpdate = user.LastDataUpdate,
                            ShareMyLocation = user.ShareMyLocation,
                            Url = user.Url,
                            Name = user.Name,
                            LastseenUnixTime = user.LastseenUnixTime,
                            UserPlatform = user.UserPlatform,
                            IsFollowing = user.IsFollowing,
                            Details = new Details()
                            {
                                post_count = user.de_post_count,
                                album_count = user.de_album_count,
                                following_count = user.de_following_count,
                                followers_count = user.de_followers_count,
                                groups_count = user.de_groups_count,
                                likes_count = user.de_likes_count,
                            },
                            St_ChatColor = user.St_ChatColor ?? AppSettings.MainColor,
                        }).ToList();

                        if (list.Count > 0)
                        {
                            return new ObservableCollection<Classes.UserContacts.User>(list);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Get data One user To My Contact Table
        public Classes.UserContacts.User Get_DataOneUser(string userId)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var user = Connection.Table<DataTables.MyContactsTB>().FirstOrDefault(c => c.UserId == userId);
                    if (user != null)
                    {
                        Classes.UserContacts.User item = new Classes.UserContacts.User()
                        {
                            UserId = user.UserId,
                            Username = user.Username,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Avatar = user.Avatar,
                            Cover = user.Cover,
                            RelationshipId = user.RelationshipId,
                            LastseenTimeText = user.LastseenTimeText,
                            Address = user.Address,
                            Working = user.Working,
                            WorkingLink = user.WorkingLink,
                            About = user.About,
                            School = user.School,
                            Gender = user.Gender,
                            Birthday = user.Birthday,
                            Website = user.Website,
                            Facebook = user.Facebook,
                            Google = user.Google,
                            Twitter = user.Twitter,
                            Linkedin = user.Linkedin,
                            Youtube = user.Youtube,
                            Vk = user.Vk,
                            Instagram = user.Instagram,
                            Language = user.Language,
                            IpAddress = user.IpAddress,
                            FollowPrivacy = user.FollowPrivacy,
                            FriendPrivacy = user.FriendPrivacy,
                            PostPrivacy = user.PostPrivacy,
                            MessagePrivacy = user.MessagePrivacy,
                            ConfirmFollowers = user.ConfirmFollowers,
                            ShowActivitiesPrivacy = user.ShowActivitiesPrivacy,
                            BirthPrivacy = user.BirthPrivacy,
                            VisitPrivacy = user.VisitPrivacy,
                            Lastseen = user.Lastseen,
                            Showlastseen = user.Showlastseen,
                            SentmeMsg = user.ESentmeMsg,
                            LastNotif = user.ELastNotif,
                            Status = user.Status,
                            Active = user.Active,
                            Admin = user.Admin,
                            Registered = user.Registered,
                            PhoneNumber = user.PhoneNumber,
                            IsPro = user.IsPro,
                            ProType = user.ProType,
                            Joined = user.Joined,
                            Timezone = user.Timezone,
                            Referrer = user.Referrer,
                            Balance = user.Balance,
                            PaypalEmail = user.PaypalEmail,
                            NotificationsSound = user.NotificationsSound,
                            OrderPostsBy = user.OrderPostsBy,
                            SocialLogin = user.SocialLogin,
                            DeviceId = user.DeviceId,
                            WebDeviceId = user.WebDeviceId,
                            Wallet = user.Wallet,
                            Lat = user.Lat,
                            Lng = user.Lng,
                            LastDataUpdate = user.LastDataUpdate,
                            ShareMyLocation = user.ShareMyLocation,
                            Url = user.Url,
                            Name = user.Name,
                            LastseenUnixTime = user.LastseenUnixTime,
                            UserPlatform = user.UserPlatform,
                            IsFollowing = user.IsFollowing,
                            Details = new Details()
                            {
                                post_count = user.de_post_count,
                                album_count = user.de_album_count,
                                following_count = user.de_following_count,
                                followers_count = user.de_followers_count,
                                groups_count = user.de_groups_count,
                                likes_count = user.de_likes_count,
                            },
                            St_ChatColor = user.St_ChatColor ?? AppSettings.MainColor,
                        };
                        return item;
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

        //Remove data To My Contact Table
        public void Delete_UsersContact(string userId)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var user = Connection.Table<DataTables.MyContactsTB>().FirstOrDefault(c => c.UserId == userId);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Clear All data UsersContact
        public void Clear_UsersContact()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.DeleteAll<DataTables.MyContactsTB>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Blocked Users

        //Insert data To Blocked Users Table
        public void Insert_Or_Replace_BlockedUsersTable(
            ObservableCollection<GetBlockedUsersObject.BlockedUsers> blockedUsersList)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.BlockedUsersTB>().ToList();
                    var list = blockedUsersList.Select(user => new DataTables.BlockedUsersTB
                    {
                        user_id = user.user_id,
                        username = user.username,
                        email = user.email,
                        first_name = user.first_name,
                        last_name = user.last_name,
                        avatar = user.avatar,
                        cover = user.cover,
                        background_image = user.background_image,
                        relationship_id = user.relationship_id,
                        address = user.address,
                        working = user.working,
                        working_link = user.working_link,
                        about = user.about,
                        school = user.school,
                        gender = user.gender,
                        birthday = user.birthday,
                        country_id = user.country_id,
                        website = user.website,
                        facebook = user.facebook,
                        google = user.google,
                        twitter = user.twitter,
                        linkedin = user.linkedin,
                        youtube = user.youtube,
                        vk = user.vk,
                        instagram = user.instagram,
                        language = user.language,
                        ip_address = user.ip_address,
                        follow_privacy = user.follow_privacy,
                        friend_privacy = user.friend_privacy,
                        post_privacy = user.post_privacy,
                        message_privacy = user.message_privacy,
                        confirm_followers = user.confirm_followers,
                        show_activities_privacy = user.show_activities_privacy,
                        birth_privacy = user.birth_privacy,
                        verified = user.verified,
                        lastseen = user.lastseen,
                        emailNotification = user.emailNotification,
                        e_liked = user.e_liked,
                        e_wondered = user.e_wondered,
                        e_shared = user.e_shared,
                        e_followed = user.e_followed,
                        e_commented = user.e_commented,
                        e_visited = user.e_visited,
                        e_liked_page = user.e_liked_page,
                        e_mentioned = user.e_mentioned,
                        e_joined_group = user.e_joined_group,
                        e_accepted = user.e_accepted,
                        e_profile_wall_post = user.e_profile_wall_post,
                        e_sentme_msg = user.e_sentme_msg,
                        e_last_notif = user.e_last_notif,
                        status = user.status,
                        active = user.active,
                        admin = user.admin,
                        registered = user.registered,
                        phone_number = user.phone_number,
                        is_pro = user.is_pro,
                        pro_type = user.pro_type,
                        timezone = user.timezone,
                        referrer = user.referrer,
                        balance = user.balance,
                        paypal_email = user.paypal_email,
                        notifications_sound = user.notifications_sound,
                        order_posts_by = user.order_posts_by,
                        device_id = user.device_id,
                        web_device_id = user.web_device_id,
                        wallet = user.wallet,
                        lat = user.lat,
                        lng = user.lng,
                        last_location_update = user.last_location_update,
                        share_my_location = user.share_my_location,
                        last_data_update = user.last_data_update,
                        last_avatar_mod = user.last_avatar_mod,
                        last_cover_mod = user.last_cover_mod,
                        avatar_full = user.avatar_full,
                        url = user.url,
                        name = user.name,
                        lastseen_unix_time = user.lastseen_unix_time,
                        lastseen_status = user.lastseen_status,
                        de_post_count = user.details.post_count,
                        de_album_count = user.details.album_count,
                        de_following_count = user.details.following_count,
                        de_followers_count = user.details.followers_count,
                        de_groups_count = user.details.groups_count,
                        de_likes_count = user.details.likes_count,
                    }).ToList();

                    if (list.Count > 0)
                    {
                        Connection.BeginTransaction();
                        //Bring new  
                        var newItemList = list.Where(c => !result.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                        if (newItemList.Count > 0)
                        {
                            Connection.InsertAll(newItemList);
                        }

                        var deleteItemList = result.Where(c => !list.Select(fc => fc.user_id).Contains(c.user_id)).ToList();
                        if (deleteItemList.Count > 0)
                        {
                            foreach (var delete in deleteItemList)
                            {
                                Connection.Delete(delete);
                            }
                        }
                        Connection.UpdateAll(list);
                        Connection.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public ObservableCollection<GetBlockedUsersObject.BlockedUsers> Get_Blocked_Users()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.BlockedUsersTB>().ToList();
                    if (result.Count > 0)
                    {
                        var list = result.Select(user => new GetBlockedUsersObject.BlockedUsers
                        {
                            user_id = user.user_id,
                            username = user.username,
                            email = user.email,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            avatar = user.avatar,
                            cover = user.cover,
                            background_image = user.background_image,
                            relationship_id = user.relationship_id,
                            address = user.address,
                            working = user.working,
                            working_link = user.working_link,
                            about = user.about,
                            school = user.school,
                            gender = user.gender,
                            birthday = user.birthday,
                            country_id = user.country_id,
                            website = user.website,
                            facebook = user.facebook,
                            google = user.google,
                            twitter = user.twitter,
                            linkedin = user.linkedin,
                            youtube = user.youtube,
                            vk = user.vk,
                            instagram = user.instagram,
                            language = user.language,
                            ip_address = user.ip_address,
                            follow_privacy = user.follow_privacy,
                            friend_privacy = user.friend_privacy,
                            post_privacy = user.post_privacy,
                            message_privacy = user.message_privacy,
                            confirm_followers = user.confirm_followers,
                            show_activities_privacy = user.show_activities_privacy,
                            birth_privacy = user.birth_privacy,
                            verified = user.verified,
                            lastseen = user.lastseen,
                            emailNotification = user.emailNotification,
                            e_liked = user.e_liked,
                            e_wondered = user.e_wondered,
                            e_shared = user.e_shared,
                            e_followed = user.e_followed,
                            e_commented = user.e_commented,
                            e_visited = user.e_visited,
                            e_liked_page = user.e_liked_page,
                            e_mentioned = user.e_mentioned,
                            e_joined_group = user.e_joined_group,
                            e_accepted = user.e_accepted,
                            e_profile_wall_post = user.e_profile_wall_post,
                            e_sentme_msg = user.e_sentme_msg,
                            e_last_notif = user.e_last_notif,
                            status = user.status,
                            active = user.active,
                            admin = user.admin,
                            registered = user.registered,
                            phone_number = user.phone_number,
                            is_pro = user.is_pro,
                            pro_type = user.pro_type,
                            timezone = user.timezone,
                            referrer = user.referrer,
                            balance = user.balance,
                            paypal_email = user.paypal_email,
                            notifications_sound = user.notifications_sound,
                            order_posts_by = user.order_posts_by,
                            device_id = user.device_id,
                            web_device_id = user.web_device_id,
                            wallet = user.wallet,
                            lat = user.lat,
                            lng = user.lng,
                            last_location_update = user.last_location_update,
                            share_my_location = user.share_my_location,
                            last_data_update = user.last_data_update,
                            last_avatar_mod = user.last_avatar_mod,
                            last_cover_mod = user.last_cover_mod,
                            avatar_full = user.avatar_full,
                            url = user.url,
                            name = user.name,
                            lastseen_unix_time = user.lastseen_unix_time,
                            lastseen_status = user.lastseen_status,
                            details = new Details()
                            {
                                post_count = user.de_post_count,
                                album_count = user.de_album_count,
                                following_count = user.de_following_count,
                                followers_count = user.de_followers_count,
                                groups_count = user.de_groups_count,
                                likes_count = user.de_likes_count,
                            }
                        }).ToList();

                        return new ObservableCollection<GetBlockedUsersObject.BlockedUsers>(list);
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

        #region My Profile

        //Insert Or Update data My Profile Table
        public void Insert_Or_Update_To_MyProfileTable(GetUserDataObject.User_Data user)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var data = Connection.Table<DataTables.MyProfileTB>().FirstOrDefault(a => a.user_id == user.user_id);
                    if (data != null)
                    {
                        data.user_id = user.user_id;
                        data.username = user.username;
                        data.email = user.email;
                        data.first_name = user.first_name;
                        data.last_name = user.last_name;
                        data.avatar = user.avatar;
                        data.cover = user.cover;
                        data.background_image = user.background_image;
                        data.relationship_id = user.relationship_id;
                        data.address = user.address;
                        data.working = user.working;
                        data.working_link = user.working_link;
                        data.about = user.about;
                        data.school = user.school;
                        data.gender = user.gender;
                        data.birthday = user.birthday;
                        data.country_id = user.country_id;
                        data.website = user.website;
                        data.facebook = user.facebook;
                        data.google = user.google;
                        data.twitter = user.twitter;
                        data.linkedin = user.linkedin;
                        data.youtube = user.youtube;
                        data.vk = user.vk;
                        data.instagram = user.instagram;
                        data.language = user.language;
                        data.ip_address = user.ip_address;
                        data.follow_privacy = user.follow_privacy;
                        data.friend_privacy = user.friend_privacy;
                        data.post_privacy = user.post_privacy;
                        data.message_privacy = user.message_privacy;
                        data.confirm_followers = user.confirm_followers;
                        data.show_activities_privacy = user.show_activities_privacy;
                        data.birth_privacy = user.birth_privacy;
                        data.visit_privacy = user.visit_privacy;
                        data.lastseen = user.lastseen;
                        data.emailNotification = user.emailNotification;
                        data.e_liked = user.e_liked;
                        data.e_wondered = user.e_wondered;
                        data.e_shared = user.e_shared;
                        data.e_followed = user.e_followed;
                        data.e_commented = user.e_commented;
                        data.e_visited = user.e_visited;
                        data.e_liked_page = user.e_liked_page;
                        data.e_mentioned = user.e_mentioned;
                        data.e_joined_group = user.e_joined_group;
                        data.e_accepted = user.e_accepted;
                        data.e_profile_wall_post = user.e_profile_wall_post;
                        data.e_sentme_msg = user.e_sentme_msg;
                        data.e_last_notif = user.e_last_notif;
                        data.status = user.status;
                        data.active = user.active;
                        data.admin = user.admin;
                        data.registered = user.registered;
                        data.phone_number = user.phone_number;
                        data.is_pro = user.is_pro;
                        data.pro_type = user.pro_type;
                        data.timezone = user.timezone;
                        data.referrer = user.referrer;
                        data.balance = user.balance;
                        data.paypal_email = user.paypal_email;
                        data.notifications_sound = user.notifications_sound;
                        data.order_posts_by = user.order_posts_by;
                        data.device_id = user.device_id;
                        data.web_device_id = user.web_device_id;
                        data.wallet = user.wallet;
                        data.lat = user.lat;
                        data.lng = user.lng;
                        data.last_location_update = user.last_location_update;
                        data.share_my_location = user.share_my_location;
                        data.last_data_update = user.last_data_update;
                        data.last_avatar_mod = user.last_avatar_mod;
                        data.last_cover_mod = user.last_cover_mod;
                        data.avatar_full = user.avatar_full;
                        data.url = user.url;
                        data.name = user.name;
                        data.lastseen_unix_time = user.lastseen_unix_time;
                        data.lastseen_status = user.lastseen_status;
                        data.is_following = user.is_following.ToString();
                        data.can_follow = user.can_follow.ToString();
                        data.is_following_me = user.is_following_me.ToString();
                        data.gender_text = user.gender_text;
                        data.lastseen_status = user.lastseen_status;
                        data.lastseen_time_text = user.lastseen_time_text;
                        data.is_blocked = user.is_blocked.ToString();
                        data.de_post_count = user.details.post_count;
                        data.de_album_count = user.details.album_count;
                        data.de_following_count = user.details.following_count;
                        data.de_followers_count = user.details.followers_count;
                        data.de_groups_count = user.details.groups_count;
                        data.de_likes_count = user.details.likes_count;

                        UserDetails.avatar = user.avatar;
                        UserDetails.cover = user.cover;
                        UserDetails.Username = user.username;
                        UserDetails.Full_name = user.name;
                        UserDetails.Email = user.email;

                        Connection.Update(data);
                    }
                    else
                    {
                        DataTables.MyProfileTB udb = new DataTables.MyProfileTB
                        {
                            user_id = user.user_id,
                            username = user.username,
                            email = user.email,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            avatar = user.avatar,
                            cover = user.cover,
                            background_image = user.background_image,
                            relationship_id = user.relationship_id,
                            address = user.address,
                            working = user.working,
                            working_link = user.working_link,
                            about = user.about,
                            school = user.school,
                            gender = user.gender,
                            birthday = user.birthday,
                            country_id = user.country_id,
                            website = user.website,
                            facebook = user.facebook,
                            google = user.google,
                            twitter = user.twitter,
                            linkedin = user.linkedin,
                            youtube = user.youtube,
                            vk = user.vk,
                            instagram = user.instagram,
                            language = user.language,
                            ip_address = user.ip_address,
                            follow_privacy = user.follow_privacy,
                            friend_privacy = user.friend_privacy,
                            post_privacy = user.post_privacy,
                            message_privacy = user.message_privacy,
                            confirm_followers = user.confirm_followers,
                            show_activities_privacy = user.show_activities_privacy,
                            birth_privacy = user.birth_privacy,
                            visit_privacy = user.visit_privacy,
                            lastseen = user.lastseen,
                            emailNotification = user.emailNotification,
                            e_liked = user.e_liked,
                            e_wondered = user.e_wondered,
                            e_shared = user.e_shared,
                            e_followed = user.e_followed,
                            e_commented = user.e_commented,
                            e_visited = user.e_visited,
                            e_liked_page = user.e_liked_page,
                            e_mentioned = user.e_mentioned,
                            e_joined_group = user.e_joined_group,
                            e_accepted = user.e_accepted,
                            e_profile_wall_post = user.e_profile_wall_post,
                            e_sentme_msg = user.e_sentme_msg,
                            e_last_notif = user.e_last_notif,
                            status = user.status,
                            active = user.active,
                            admin = user.admin,
                            registered = user.registered,
                            phone_number = user.phone_number,
                            is_pro = user.is_pro,
                            pro_type = user.pro_type,
                            timezone = user.timezone,
                            referrer = user.referrer,
                            balance = user.balance,
                            paypal_email = user.paypal_email,
                            notifications_sound = user.notifications_sound,
                            order_posts_by = user.order_posts_by,
                            device_id = user.device_id,
                            web_device_id = user.web_device_id,
                            wallet = user.wallet,
                            lat = user.lat,
                            lng = user.lng,
                            last_location_update = user.last_location_update,
                            share_my_location = user.share_my_location,
                            last_data_update = user.last_data_update,
                            last_avatar_mod = user.last_avatar_mod,
                            last_cover_mod = user.last_cover_mod,
                            avatar_full = user.avatar_full,
                            url = user.url,
                            name = user.name,
                            lastseen_unix_time = user.lastseen_unix_time,
                            lastseen_status = user.lastseen_status,
                            is_following = user.is_following.ToString(),
                            can_follow = user.can_follow.ToString(),
                            is_following_me = user.is_following_me.ToString(),
                            gender_text = user.gender_text,
                            lastseen_time_text = user.lastseen_time_text,
                            is_blocked = user.is_blocked.ToString(),
                            de_post_count = user.details.post_count,
                            de_album_count = user.details.album_count,
                            de_following_count = user.details.following_count,
                            de_followers_count = user.details.followers_count,
                            de_groups_count = user.details.groups_count,
                            de_likes_count = user.details.likes_count,
                        };

                        UserDetails.avatar = udb.avatar;
                        UserDetails.cover = udb.cover;
                        UserDetails.Username = udb.username;
                        UserDetails.Full_name = udb.name;
                        UserDetails.Email = udb.email;

                        //Insert 
                        Connection.Insert(udb);
                    }

                    Classes.MyProfileList = new ObservableCollection<GetUserDataObject.User_Data>();
                    if (Classes.MyProfileList.Count > 0)
                    {
                        Classes.MyProfileList.Clear();
                        Classes.MyProfileList.Add(user);
                    }
                    else
                    {
                        Classes.MyProfileList.Add(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To My Profile Table
        public ObservableCollection<GetUserDataObject.User_Data> Get_MyProfile(string userid)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var listdata = new ObservableCollection<GetUserDataObject.User_Data>();
                    var user = Connection.Table<DataTables.MyProfileTB>()
                        .FirstOrDefault(a => a.user_id == userid);
                    if (user != null)
                    {
                        GetUserDataObject.User_Data data = new GetUserDataObject.User_Data()
                        {
                            user_id = user.user_id,
                            username = user.username,
                            email = user.email,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            avatar = user.avatar,
                            cover = user.cover,
                            background_image = user.background_image,
                            relationship_id = user.relationship_id,
                            address = user.address,
                            working = user.working,
                            working_link = user.working_link,
                            about = user.about,
                            school = user.school,
                            gender = user.gender,
                            birthday = user.birthday,
                            country_id = user.country_id,
                            website = user.website,
                            facebook = user.facebook,
                            google = user.google,
                            twitter = user.twitter,
                            linkedin = user.linkedin,
                            youtube = user.youtube,
                            vk = user.vk,
                            instagram = user.instagram,
                            language = user.language,
                            ip_address = user.ip_address,
                            follow_privacy = user.follow_privacy,
                            friend_privacy = user.friend_privacy,
                            post_privacy = user.post_privacy,
                            message_privacy = user.message_privacy,
                            confirm_followers = user.confirm_followers,
                            show_activities_privacy = user.show_activities_privacy,
                            birth_privacy = user.birth_privacy,
                            visit_privacy = user.visit_privacy,
                            lastseen = user.lastseen,
                            emailNotification = user.emailNotification,
                            e_liked = user.e_liked,
                            e_wondered = user.e_wondered,
                            e_shared = user.e_shared,
                            e_followed = user.e_followed,
                            e_commented = user.e_commented,
                            e_visited = user.e_visited,
                            e_liked_page = user.e_liked_page,
                            e_mentioned = user.e_mentioned,
                            e_joined_group = user.e_joined_group,
                            e_accepted = user.e_accepted,
                            e_profile_wall_post = user.e_profile_wall_post,
                            e_sentme_msg = user.e_sentme_msg,
                            e_last_notif = user.e_last_notif,
                            status = user.status,
                            active = user.active,
                            admin = user.admin,
                            registered = user.registered,
                            phone_number = user.phone_number,
                            is_pro = user.is_pro,
                            pro_type = user.pro_type,
                            timezone = user.timezone,
                            referrer = user.referrer,
                            balance = user.balance,
                            paypal_email = user.paypal_email,
                            notifications_sound = user.notifications_sound,
                            order_posts_by = user.order_posts_by,
                            device_id = user.device_id,
                            web_device_id = user.web_device_id,
                            wallet = user.wallet,
                            lat = user.lat,
                            lng = user.lng,
                            last_location_update = user.last_location_update,
                            share_my_location = user.share_my_location,
                            last_data_update = user.last_data_update,
                            last_avatar_mod = user.last_avatar_mod,
                            last_cover_mod = user.last_cover_mod,
                            avatar_full = user.avatar_full,
                            url = user.url,
                            name = user.name,
                            lastseen_unix_time = user.lastseen_unix_time,
                            lastseen_status = user.lastseen_status,
                            is_following = Convert.ToInt32(user.is_following),
                            can_follow = Convert.ToInt32(user.can_follow),
                            is_following_me = Convert.ToInt32(user.is_following_me),
                            gender_text = user.gender_text,
                            lastseen_time_text = user.lastseen_time_text,
                            is_blocked = Convert.ToBoolean(user.is_blocked),
                            details = new Details()
                            {
                                post_count = user.de_post_count,
                                album_count = user.de_album_count,
                                following_count = user.de_following_count,
                                followers_count = user.de_followers_count,
                                groups_count = user.de_groups_count,
                                likes_count = user.de_likes_count,
                            }
                        };

                        listdata.Add(data);

                        UserDetails.Username = data.name;
                        UserDetails.Full_name = data.name;
                        UserDetails.avatar = data.avatar;
                        UserDetails.cover = data.cover;

                        Classes.MyProfileList = new ObservableCollection<GetUserDataObject.User_Data>();
                        Classes.MyProfileList.Clear();
                        Classes.MyProfileList = listdata;

                        return listdata;
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

        public GetUserDataObject.User_Data Get_UserProfile(string userid)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var user = Connection.Table<DataTables.MyProfileTB>().FirstOrDefault(a => a.user_id == userid);
                    if (user != null)
                    {
                        GetUserDataObject.User_Data data = new GetUserDataObject.User_Data()
                        {
                            user_id = user.user_id,
                            username = user.username,
                            email = user.email,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            avatar = user.avatar,
                            cover = user.cover,
                            background_image = user.background_image,
                            relationship_id = user.relationship_id,
                            address = user.address,
                            working = user.working,
                            working_link = user.working_link,
                            about = user.about,
                            school = user.school,
                            gender = user.gender,
                            birthday = user.birthday,
                            country_id = user.country_id,
                            website = user.website,
                            facebook = user.facebook,
                            google = user.google,
                            twitter = user.twitter,
                            linkedin = user.linkedin,
                            youtube = user.youtube,
                            vk = user.vk,
                            instagram = user.instagram,
                            language = user.language,
                            ip_address = user.ip_address,
                            follow_privacy = user.follow_privacy,
                            friend_privacy = user.friend_privacy,
                            post_privacy = user.post_privacy,
                            message_privacy = user.message_privacy,
                            confirm_followers = user.confirm_followers,
                            show_activities_privacy = user.show_activities_privacy,
                            birth_privacy = user.birth_privacy,
                            visit_privacy = user.visit_privacy,
                            lastseen = user.lastseen,
                            emailNotification = user.emailNotification,
                            e_liked = user.e_liked,
                            e_wondered = user.e_wondered,
                            e_shared = user.e_shared,
                            e_followed = user.e_followed,
                            e_commented = user.e_commented,
                            e_visited = user.e_visited,
                            e_liked_page = user.e_liked_page,
                            e_mentioned = user.e_mentioned,
                            e_joined_group = user.e_joined_group,
                            e_accepted = user.e_accepted,
                            e_profile_wall_post = user.e_profile_wall_post,
                            e_sentme_msg = user.e_sentme_msg,
                            e_last_notif = user.e_last_notif,
                            status = user.status,
                            active = user.active,
                            admin = user.admin,
                            registered = user.registered,
                            phone_number = user.phone_number,
                            is_pro = user.is_pro,
                            pro_type = user.pro_type,
                            timezone = user.timezone,
                            referrer = user.referrer,
                            balance = user.balance,
                            paypal_email = user.paypal_email,
                            notifications_sound = user.notifications_sound,
                            order_posts_by = user.order_posts_by,
                            device_id = user.device_id,
                            web_device_id = user.web_device_id,
                            wallet = user.wallet,
                            lat = user.lat,
                            lng = user.lng,
                            last_location_update = user.last_location_update,
                            share_my_location = user.share_my_location,
                            last_data_update = user.last_data_update,
                            last_avatar_mod = user.last_avatar_mod,
                            last_cover_mod = user.last_cover_mod,
                            avatar_full = user.avatar_full,
                            url = user.url,
                            name = user.name,
                            lastseen_unix_time = user.lastseen_unix_time,
                            lastseen_status = user.lastseen_status,
                            is_following = Convert.ToInt32(user.is_following),
                            can_follow = Convert.ToInt32(user.can_follow),
                            is_following_me = Convert.ToInt32(user.is_following_me),
                            gender_text = user.gender_text,
                            lastseen_time_text = user.lastseen_time_text,
                            is_blocked = Convert.ToBoolean(user.is_blocked),
                            details = new Details()
                            {
                                post_count = user.de_post_count,
                                album_count = user.de_album_count,
                                following_count = user.de_following_count,
                                followers_count = user.de_followers_count,
                                groups_count = user.de_groups_count,
                                likes_count = user.de_likes_count,
                            }
                        };

                        return data;
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

        #region Search Filter 

        public void InsertOrUpdate_SearchFilter(DataTables.SearchFilterTB dataFilter)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var data = Connection.Table<DataTables.SearchFilterTB>().FirstOrDefault(c => c.UserId == dataFilter.UserId);
                    if (data == null)
                    {
                        Connection.Insert(dataFilter);
                    }
                    else
                    {
                        data.UserId = dataFilter.UserId;
                        data.ProfilePicture = dataFilter.ProfilePicture;
                        data.Gender = dataFilter.Gender;
                        data.Status = dataFilter.Status;
                        Connection.Update(data);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public DataTables.SearchFilterTB GetSearchFilterById()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var data = Connection.Table<DataTables.SearchFilterTB>()
                        .FirstOrDefault(c => c.UserId == UserDetails.User_id);
                    if (data != null)
                        return data;
                    else
                        return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Message

        //Insert data To Message Table
        public void Insert_Or_Replace_MessagesTable(ObservableCollection<Classes.Message> messageList)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    List<DataTables.MessageTB> listOfDatabaseForInsert = new List<DataTables.MessageTB>();
                    List<DataTables.MessageTB> listOfDatabaseForUpdate = new List<DataTables.MessageTB>();

                    // get data from database
                    var resultMessage = Connection.Table<DataTables.MessageTB>().ToList();
                    var listAllMessage = resultMessage.Select(messages => new Classes.Message()
                    {
                        M_id = messages.M_id,
                        from_id = messages.from_id,
                        group_id = messages.group_id,
                        to_id = messages.to_id,
                        text = messages.text,
                        media = messages.media,
                        mediaFileName = messages.mediaFileName,
                        mediaFileNames = messages.mediaFileNames,
                        time = messages.time,
                        seen = messages.seen,
                        deleted_one = messages.deleted_one,
                        deleted_two = messages.deleted_two,
                        sent_push = messages.sent_push,
                        notification_id = messages.notification_id,
                        type_two = messages.type_two,
                        stickers = messages.stickers,
                        time_text = messages.time_text,
                        position = messages.position,
                        type = messages.type,
                        file_size = messages.file_size,
                        avatar = messages.avatar,
                        MediaDuration = messages.MediaDuration,
                        Media_IsPlaying = messages.Media_IsPlaying,
                        ContactNumber = messages.ContactNumber,
                        ContactName = messages.ContactName,
                    }).ToList();

                    foreach (var messages in messageList)
                    {
                        DataTables.MessageTB maTb = new DataTables.MessageTB()
                        {
                            M_id = messages.M_id,
                            from_id = messages.from_id,
                            group_id = messages.group_id,
                            to_id = messages.to_id,
                            text = messages.text,
                            media = messages.media,
                            mediaFileName = messages.mediaFileName,
                            mediaFileNames = messages.mediaFileNames,
                            time = messages.time,
                            seen = messages.seen,
                            deleted_one = messages.deleted_one,
                            deleted_two = messages.deleted_two,
                            sent_push = messages.sent_push,
                            notification_id = messages.notification_id,
                            type_two = messages.type_two,
                            stickers = messages.stickers,
                            time_text = messages.time_text,
                            position = messages.position,
                            type = messages.type,
                            file_size = messages.file_size,
                            avatar = messages.avatar,
                            MediaDuration = messages.MediaDuration,
                            Media_IsPlaying = messages.Media_IsPlaying,
                            ContactNumber = messages.ContactNumber,
                            ContactName = messages.ContactName,
                        };

                        var dataCheck = listAllMessage.FirstOrDefault(a => a.M_id == messages.M_id);
                        if (dataCheck != null)
                        {
                            var checkForUpdate = resultMessage.FirstOrDefault(a => a.M_id == dataCheck.M_id);
                            if (checkForUpdate != null)
                            {
                                checkForUpdate.M_id = messages.M_id;
                                checkForUpdate.from_id = messages.from_id;
                                checkForUpdate.group_id = messages.group_id;
                                checkForUpdate.to_id = messages.to_id;
                                checkForUpdate.text = messages.text;
                                checkForUpdate.media = messages.media;
                                checkForUpdate.mediaFileName = messages.mediaFileName;
                                checkForUpdate.mediaFileNames = messages.mediaFileNames;
                                checkForUpdate.time = messages.time;
                                checkForUpdate.seen = messages.seen;
                                checkForUpdate.deleted_one = messages.deleted_one;
                                checkForUpdate.deleted_two = messages.deleted_two;
                                checkForUpdate.sent_push = messages.sent_push;
                                checkForUpdate.notification_id = messages.notification_id;
                                checkForUpdate.type_two = messages.type_two;
                                checkForUpdate.stickers = messages.stickers;
                                checkForUpdate.time_text = messages.time_text;
                                checkForUpdate.position = messages.position;
                                checkForUpdate.type = messages.type;
                                checkForUpdate.file_size = messages.file_size;
                                checkForUpdate.avatar = messages.avatar;
                                checkForUpdate.MediaDuration = messages.MediaDuration;
                                checkForUpdate.Media_IsPlaying = messages.Media_IsPlaying;
                                checkForUpdate.ContactNumber = messages.ContactNumber;
                                checkForUpdate.ContactName = messages.ContactName;

                                listOfDatabaseForUpdate.Add(checkForUpdate);
                            }
                            else
                            {
                                listOfDatabaseForInsert.Add(maTb);
                            }
                        }
                        else
                        {
                            listOfDatabaseForInsert.Add(maTb);
                        }
                    }

                    Connection.BeginTransaction();

                    //Bring new  
                    if (listOfDatabaseForInsert.Count > 0)
                    {
                        Connection.InsertAll(listOfDatabaseForInsert);
                    }

                    if (listOfDatabaseForUpdate.Count > 0)
                    {
                        Connection.UpdateAll(listOfDatabaseForUpdate);
                    }
                    Connection.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Update one Messages Table
        public void Insert_Or_Update_To_one_MessagesTable(Classes.Message credentials)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var data = Connection.Table<DataTables.MessageTB>().FirstOrDefault(a => a.M_id == credentials.M_id);
                    if (data != null)
                    {
                        data.M_id = credentials.M_id;
                        data.from_id = credentials.from_id;
                        data.group_id = credentials.group_id;
                        data.to_id = credentials.to_id;
                        data.text = credentials.text;
                        data.media = credentials.media;
                        data.mediaFileName = credentials.mediaFileName;
                        data.mediaFileNames = credentials.mediaFileNames;
                        data.time = credentials.time;
                        data.seen = credentials.seen;
                        data.deleted_one = credentials.deleted_one;
                        data.deleted_two = credentials.deleted_two;
                        data.sent_push = credentials.sent_push;
                        data.notification_id = credentials.notification_id;
                        data.type_two = credentials.type_two;
                        data.stickers = credentials.stickers;
                        data.time_text = credentials.time_text;
                        data.position = credentials.position;
                        data.type = credentials.type;
                        data.file_size = credentials.file_size;
                        data.avatar = credentials.avatar;
                        data.MediaDuration = credentials.MediaDuration;
                        data.Media_IsPlaying = credentials.Media_IsPlaying;
                        data.ContactNumber = credentials.ContactNumber;
                        data.ContactName = credentials.ContactName;

                        Connection.Update(data);
                    }
                    else
                    {
                        DataTables.MessageTB mdb = new DataTables.MessageTB
                        {
                            M_id = credentials.M_id,
                            from_id = credentials.from_id,
                            group_id = credentials.group_id,
                            to_id = credentials.to_id,
                            text = credentials.text,
                            media = credentials.media,
                            mediaFileName = credentials.mediaFileName,
                            mediaFileNames = credentials.mediaFileNames,
                            time = credentials.time,
                            seen = credentials.seen,
                            deleted_one = credentials.deleted_one,
                            deleted_two = credentials.deleted_two,
                            sent_push = credentials.sent_push,
                            notification_id = credentials.notification_id,
                            type_two = credentials.type_two,
                            stickers = credentials.stickers,
                            time_text = credentials.time_text,
                            position = credentials.position,
                            type = credentials.type,
                            file_size = credentials.file_size,
                            avatar = credentials.avatar,
                            MediaDuration = credentials.MediaDuration,
                            Media_IsPlaying = credentials.Media_IsPlaying,
                            ContactNumber = credentials.ContactNumber,
                            ContactName = credentials.ContactName
                        };

                        //Insert  one Messages Table
                        Connection.Insert(mdb);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data To Messages
        public string GetMessages_CredentialsList(string from_id, string to_id, string before_message_id)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var before_q = "";
                    if (before_message_id != "0")
                    {
                        before_q = "AND M_id < " + before_message_id + " AND M_id <> " + before_message_id + " ";
                    }

                    var query = Connection.Query<DataTables.MessageTB>
                        ("SELECT * FROM MessageTB WHERE ((from_id =" + from_id + " and to_id=" + to_id + ") OR (from_id =" + to_id + " and to_id=" + from_id + ")) " + before_q);

                    var query_limit_from = query.Count - 35;
                    if (query_limit_from < 1)
                    {
                        query_limit_from = 0;
                    }

                    List<DataTables.MessageTB> Query = query.Where(w => w.from_id == from_id && w.to_id == to_id || w.to_id == from_id && w.from_id == to_id).OrderBy(q => q.ID).TakeLast(35).ToList();
                
                    if (Query.Count > 0)
                    {
                        foreach (var item in Query)
                        {
                            Classes.Message m = new Classes.Message
                            {
                                M_id = item.M_id,
                                from_id = item.from_id,
                                group_id = item.group_id,
                                to_id = item.to_id,
                                text = item.text,
                                media = item.media,
                                mediaFileName = item.mediaFileName,
                                mediaFileNames = item.mediaFileNames,
                                time = item.time,
                                seen = item.seen,
                                deleted_one = item.deleted_one,
                                deleted_two = item.deleted_two,
                                sent_push = item.sent_push,
                                notification_id = item.notification_id,
                                type_two = item.type_two,
                                stickers = item.stickers,
                                time_text = item.time_text,
                                position = item.position,
                                type = item.type,
                                file_size = item.file_size,
                                avatar = item.avatar,
                                MediaDuration = item.MediaDuration,
                                Media_IsPlaying = item.Media_IsPlaying,
                                ContactNumber = item.ContactNumber,
                                ContactName = item.ContactName
                            };

                            if (before_message_id == "0")
                            {
                                ChatWindow_Activity.MAdapter?.Add(m);
                            }
                            else
                            {
                                ChatWindow_Activity.MAdapter?.Insert(m, before_message_id);
                            }
                        }
                        return "1";
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "0";
            }
        }

        //Get data To where first Messages >> load more
        public List<DataTables.MessageTB> GetMessageList(string from_id, string to_id, string before_message_id)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                   

                    var before_q = "";
                    if (before_message_id != "0")
                    {
                        before_q = "AND M_id < " + before_message_id + " AND M_id <> " + before_message_id + " ";
                    }

                    var query = Connection.Query<DataTables.MessageTB>
                        ("SELECT * FROM MessageTB WHERE ((from_id =" + from_id + " and to_id=" + to_id + ") OR (from_id =" + to_id + " and to_id=" + from_id + ")) " + before_q);

                    List<DataTables.MessageTB> Query = query.Where(w => w.from_id == from_id && w.to_id == to_id || w.to_id == from_id && w.from_id == to_id).OrderBy(q => q.ID).TakeLast(25).ToList();


                    List<DataTables.MessageTB> Result = Query;
                    return Result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //Remove data To Messages Table
        public void Delete_OneMessageUser(string Message_ID)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var user = Connection.Table<DataTables.MessageTB>().FirstOrDefault(c => c.M_id == Message_ID);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteAllMessagesUser(string from_id, string to_id)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var query = Connection.Query<DataTables.MessageTB>("Delete FROM MessageTB WHERE ((from_id =" + from_id + " and to_id=" + to_id + ") OR (from_id =" + to_id + " and to_id=" + from_id + "))");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Remove All data To Messages Table
        public void ClearAll_Messages()
        {
            try
            {
                Connection.DeleteAll<DataTables.MessageTB>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Call_User

        public void Insert_CallUser(Classes.Call_User user)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    DataTables.Call_UserTB cv = new DataTables.Call_UserTB
                    {
                        callID = user.id,
                        user_id = user.user_id,
                        avatar = user.avatar,
                        name = user.name,
                        access_token = user.access_token,
                        access_token_2 = user.access_token_2,
                        from_id = user.from_id,
                        active = user.active,
                        time = user.time,
                        status = user.status,
                        room_name = user.room_name,
                        type = user.type,
                        typeIcon = user.typeIcon,
                        typeColor = user.typeColor
                    };

                    Connection.Insert(cv);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Update_CallUser(DataTables.Call_UserTB user)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.Update(user);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public ObservableCollection<Classes.Call_User> Get_CallUserList()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var list = new ObservableCollection<Classes.Call_User>();
                    var Result = Connection.Table<DataTables.Call_UserTB>().ToList();
                    if (Result.Count > 0)
                    {
                        foreach (var item in Result)
                        {
                            Classes.Call_User cv = new Classes.Call_User
                            {
                                id = item.callID,
                                user_id = item.user_id,
                                avatar = item.avatar,
                                name = item.name,
                                access_token = item.access_token,
                                access_token_2 = item.access_token_2,
                                from_id = item.from_id,
                                active = item.active,
                                time = item.time,
                                status = item.status,
                                room_name = item.room_name,
                                type = item.type,
                                typeIcon = item.typeIcon,
                                typeColor = item.typeColor
                            };

                            list.Add(cv);
                        }

                        return new ObservableCollection<Classes.Call_User>(list.OrderBy(a => a.id).ToList());
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

        public void Delete_CallUser_Credential(string id)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var call = Connection.Table<DataTables.Call_UserTB>().FirstOrDefault(c => c.callID == id);
                    if (call != null)
                    {
                        Connection.Delete(call);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Clear_CallUser_List()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.DeleteAll<DataTables.Call_UserTB>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Last_User_Chat

        //Insert Or Update data To Users Table
        public void Insert_Or_Update_LastUsersChat(ObservableCollection<Classes.Get_Users_List_Object.User> lastUsersList)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.LastUsersTB>().ToList();
                    var list = lastUsersList.Select(user => new DataTables.LastUsersTB
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        ProfilePicture = user.ProfilePicture,
                        CoverPicture = user.CoverPicture,
                        LastseenTimeText = user.LastseenTimeText,
                        Lastseen = user.Lastseen,
                        Url = user.Url,
                        Name = user.Name,
                        LastseenUnixTime = user.LastseenUnixTime,
                        ChatColor = user.ChatColor,
                        Verified = user.ChatColor,
                        MId = user.LastMessage.Id,
                        MFromId = user.LastMessage.FromId,
                        MGroupId = user.LastMessage.GroupId,
                        MToId = user.LastMessage.ToId,
                        MText = user.LastMessage.Text,
                        MMedia = user.LastMessage.Media,
                        MMediaFileName = user.LastMessage.MediaFileName,
                        MMediaFileNames = user.LastMessage.MediaFileNames,
                        MTime = user.LastMessage.Time,
                        MSeen = user.LastMessage.Seen,
                        MDeletedOne = user.LastMessage.DeletedOne,
                        MDeletedTwo = user.LastMessage.DeletedTwo,
                        MSentPush = user.LastMessage.SentPush,
                        MNotificationId = user.LastMessage.NotificationId,
                        MTypeTwo = user.LastMessage.TypeTwo,
                        MStickers = user.LastMessage.Stickers,
                        MDateTime = user.LastMessage.DateTime,
                    }).ToList();

                    if (list.Count > 0)
                    {
                        Connection.BeginTransaction();
                        //Bring new  
                        var newItemList = list.Where(c => !result.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                        if (newItemList.Count > 0)
                        {
                            Connection.InsertAll(newItemList);
                        }

                        var deleteItemList = result.Where(c => !list.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                        if (deleteItemList.Count > 0)
                        {
                            foreach (var delete in deleteItemList)
                            {
                                Connection.Delete(delete);
                            }
                        }
                        Connection.UpdateAll(list);
                        Connection.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To Last Users Chat
        public ObservableCollection<Classes.Get_Users_List_Object.User> Get_LastUsersChat_List()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.LastUsersTB>().ToList();
                    if (result.Count > 0)
                    {
                        var list = result.Select(user => new Classes.Get_Users_List_Object.User
                        {
                            UserId = user.UserId,
                            Username = user.Username,
                            ProfilePicture = user.ProfilePicture,
                            CoverPicture = user.CoverPicture,
                            LastseenTimeText = user.LastseenTimeText,
                            Lastseen = user.Lastseen,
                            Url = user.Url,
                            Name = user.Name,
                            LastseenUnixTime = user.LastseenUnixTime,
                            ChatColor = user.ChatColor,
                            Verified = user.ChatColor,
                            LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                            {
                                Id = user.MId,
                                FromId = user.MFromId,
                                GroupId = user.MGroupId,
                                ToId = user.MToId,
                                Text = user.MText,
                                Media = user.MMedia,
                                MediaFileName = user.MMediaFileName,
                                MediaFileNames = user.MMediaFileNames,
                                Time = user.MTime,
                                Seen = user.MSeen,
                                DeletedOne = user.MDeletedOne,
                                DeletedTwo = user.MDeletedTwo,
                                SentPush = user.MSentPush,
                                NotificationId = user.MNotificationId,
                                TypeTwo = user.MTypeTwo,
                                Stickers = user.MStickers,
                                DateTime = user.MDateTime,
                            }
                        }).ToList();

                        return new ObservableCollection<Classes.Get_Users_List_Object.User>(list);
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

        //Remove data from Users Table
        public void Delete_LastUsersChat(string userID)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var user = Connection.Table<DataTables.LastUsersTB>().FirstOrDefault(c => c.UserId == userID);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Remove All data To Users Table
        public void ClearAll_LastUsersChat()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.DeleteAll<DataTables.LastUsersTB>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }
} 