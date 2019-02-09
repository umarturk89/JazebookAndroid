using Android.App;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.SQLite;
using WoWonder_API.Classes.Global;
using WoWonder_API.Classes.Message;
using WoWonder_API.Classes.Story;
using WoWonder_API.Requests;


namespace WoWonder.Functions
{
    public class MessageController
    {
        //############# DONT'T MODIFY HERE #############
        //========================= Functions =========================
        public static async Task SendMessageTask(string userid, string messageId, string text = "", string contact = "", string pathFile = "", string image_url = "", string stickerId = "", string gifUrl = "")
        {
            try
            {
                var (Api_status, respond) = await WoWonder_API.Requests.RequestsAsync.Message.Send_Message(userid, messageId, text, contact, pathFile, image_url, stickerId, gifUrl);
                if (Api_status == 200)
                {
                    if (respond is SendMessageObject result)
                    {
                        UpdateLastIdMessage(result, image_url);
                    }
                }
                else if (Api_status == 400)
                {
                    if (respond is ErrorObject error)
                    {
                        var errortext = error._errors.Error_text;
                      
                    }
                }
                else if (Api_status == 404)
                {
                    var error = respond.ToString();
              
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void UpdateLastIdMessage(SendMessageObject ChatMessages, string filepath)
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();

                foreach (var messageInfo in ChatMessages.message_data)
                {
                    var M_id = messageInfo.id;
                    var from_id = messageInfo.from_id; // user id
                    var group_id = messageInfo.group_id;
                    var to_id = messageInfo.to_id;
                    var text = IMethods.Fun_String.DecodeString(messageInfo.text);
                    var textlong = messageInfo.text;
                    var media = messageInfo.media;
                    var mediaFileName = messageInfo.mediaFileName;
                    var mediaFileNames = messageInfo.mediaFileNames;
                    var time = messageInfo.time;
                    var seen = messageInfo.seen;
                    var deleted_one = messageInfo.deleted_one;
                    var deleted_two = messageInfo.deleted_two;
                    var sent_push = messageInfo.sent_push;
                    var notification_id = messageInfo.notification_id;
                    var type_two = messageInfo.type_two;
                    var stickers = IMethods.Fun_String.StringNullRemover(messageInfo.stickers);
                    var time_text = messageInfo.time_text;
                    var position = messageInfo.position;
                    var type = messageInfo.type;
                    var file_size = messageInfo.file_size;
                    var send_time = messageInfo.message_hash_id;

                    var avatar = messageInfo.messageUser.avatar;

                    var cheker = ChatWindow_Activity.MAdapter.mmessage.FirstOrDefault(a => a.M_id == send_time);
                    if (cheker != null)
                    {
                        #region right_text

                        if (type == "right_gif" || text == "" && (type == "right_text" || type == "left_text") && stickers.Contains(".gif"))
                        {
                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = "right_gif";
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;

                           

                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendGifFile);
                                    }

                                    updater.LastMessage.Text = text;

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);

                               
                                  
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                        
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }
                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                        } 
                        else if (type == "right_text")
                        {
                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;

                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendMessage);
                                    }

                                    updater.LastMessage.Text = text.Contains("http") ? text : IMethods.Fun_String.DecodeString(text);

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                           
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                } 
                            }

                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                        }

                        #endregion

                        #region right_sticker
                        else if (type == "right_sticker")
                        {
                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;
                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendStickerFile);
                                    }
                                    updater.LastMessage.Text = text;

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                                  
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                         
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }


                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                            
                        }

                        #endregion

                        #region right_contact
                        else if (type == "right_contact")
                        {
                            string[] stringSeparators = new string[] { "&quot;" };
                            var name = textlong.Split(stringSeparators, StringSplitOptions.None);
                            var string_name = name[3];
                            var string_number = name[7];
                            cheker.ContactName = string_name;
                            cheker.ContactNumber = string_number;

                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;
                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                       
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }

                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                            
                        }

                        #endregion

                        #region right_file
                        else if (type == "right_file")
                        {
                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;

                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendFile);
                                    }
                                    updater.LastMessage.Text = text;

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                                    
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                     
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }


                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                           
                        }

                        #endregion

                        #region right_video
                        else if (type == "right_video")
                        {
                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;

                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendVideoFile);
                                    }
                                    updater.LastMessage.Text = text;

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                                  
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                         
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }


                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                            
                        }

                        #endregion

                        #region right_image
                        else if (type == "right_image")
                        {
                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;

                           

                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendImageFile);
                                    }
                                    updater.LastMessage.Text = text;

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                                  
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                        
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }

                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                           

                        }

                        #endregion

                        #region right_audio
                        else if (type == "right_audio" || type == "right_Audio")
                        {
                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;

                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendAudioFile);
                                    }
                                    updater.LastMessage.Text = text;

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                                   
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                       
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }


                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                            
                        }
                        #endregion

                       
                        else
                        {
                            cheker.M_id = M_id;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;

                            if (text.Contains("http"))
                            {
                                cheker.text = text;
                            }
                            else
                            {
                                cheker.text = IMethods.Fun_String.DecodeString(text);
                            }

                            cheker.M_id = M_id;
                            cheker.time_text = time_text;
                            cheker.from_id = from_id;
                            cheker.group_id = group_id;
                            cheker.to_id = to_id;
                            cheker.type = type;
                            cheker.position = position;
                            cheker.media = media;
                            cheker.mediaFileName = mediaFileName;
                            cheker.mediaFileNames = mediaFileNames;
                            cheker.time = time;
                            cheker.seen = "1";
                            cheker.deleted_one = deleted_one;
                            cheker.deleted_two = deleted_two;
                            cheker.sent_push = sent_push;
                            cheker.notification_id = notification_id;
                            cheker.type_two = type_two;
                            cheker.stickers = stickers;
                            cheker.position = position;
                            cheker.file_size = file_size;
                            cheker.avatar = avatar;

                            var updater = Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(a => a.UserId == to_id);
                            if (updater != null)
                            {
                                var index = Last_Messages_Fragment.mAdapter.MLastMessagesUser.IndexOf(Last_Messages_Fragment.mAdapter.MLastMessagesUser.FirstOrDefault(X => X.UserId == to_id));
                                if (index > -1)
                                {
                                    if (string.IsNullOrEmpty(text))
                                    {
                                        text = Application.Context.GetText(Resource.String.Lbl_SendMessage);
                                    }
                                    updater.LastMessage.Text = text;

                                    Last_Messages_Fragment.mAdapter.Move(updater);
                                    Last_Messages_Fragment.mAdapter.Update(updater);
                                  
                                }
                            }
                            else
                            {
                                var userdata = dbDatabase.Get_DataOneUser(to_id);

                                if (userdata != null)
                                {
                                    Classes.Get_Users_List_Object.User user = new Classes.Get_Users_List_Object.User
                                    {
                                        UserId = to_id,
                                        Username = userdata.Username,
                                        ProfilePicture = userdata.Avatar,
                                        CoverPicture = userdata.Cover,
                                        LastseenTimeText = userdata.LastseenTimeText,
                                        Lastseen = userdata.Lastseen,
                                        Url = userdata.Url,
                                        Name = userdata.Name,
                                        LastseenUnixTime = userdata.LastseenUnixTime,
                                        ChatColor = AppSettings.MainColor,
                                        Verified = userdata.Verified,
                                        LastMessage = new Classes.Get_Users_List_Object.LastMessage()
                                        {
                                            Id = M_id,
                                            FromId = from_id,
                                            GroupId = group_id,
                                            ToId = to_id,
                                            Text = text,
                                            Media = media,
                                            MediaFileName = mediaFileName,
                                            MediaFileNames = mediaFileNames,
                                            Time = time,
                                            Seen = seen,
                                            DeletedOne = deleted_one,
                                            DeletedTwo = deleted_two,
                                            SentPush = sent_push,
                                            NotificationId = notification_id,
                                            TypeTwo = type_two,
                                            Stickers = stickers,
                                 
                                        },
                                    };

                                    Last_Messages_Fragment.mAdapter.Insert(user);

                                    var userOb = new ObservableCollection<Classes.Get_Users_List_Object.User>
                                    {
                                        user
                                    };

                                    //Update All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(userOb);
                                }
                            }


                            //Update All data users to database
                            dbDatabase.Insert_Or_Update_To_one_MessagesTable(cheker);
                            dbDatabase.Dispose();
                        }

                        //Update fata RecyclerView Messeges.
                        if (type != "right_sticker")
                            ChatWindow_Activity.Update_One_Messeges(cheker);

                        if (SettingsPrefsFragment.S_SoundControl)
                            IMethods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");

                        
                    }
                }
                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}