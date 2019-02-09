using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Luseen.Autolinklibrary;
using FFImageLoading;
using FFImageLoading.Views;
using MimeTypes;
using Refractored.Controls;

using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.DefaultUser;
using WoWonder.Functions;
using WoWonder.Helpers;
using Console = System.Console;
using Path = System.IO.Path;

namespace WoWonder.Adapters
{
    public class Message_Adapter : RecyclerView.Adapter
    {
        public event EventHandler<Message_AdapterClickEventArgs> ItemClick;
        public event EventHandler<Message_AdapterClickEventArgs> ItemLongClick;

        public ObservableCollection<Classes.Message> mmessage = new ObservableCollection<Classes.Message>();
        private ChatWindow_Activity Main_Activity;

        public Message_Adapter(ChatWindow_Activity activity)
        {
            try
            {
                Main_Activity = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                var itemView = mmessage[viewType];
                if (itemView != null)
                {
                    if (itemView.type == "right_gif" || (itemView.text == "" && (itemView.type == "right_text") && itemView.stickers.Contains(".gif")))
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_gif, parent, false);
                        GifViewHolder viewHolder = new GifViewHolder(row);
                        return viewHolder;
                    }
                    else if (itemView.type == "left_gif" || (itemView.text == "" && (itemView.type == "left_text") && itemView.stickers.Contains(".gif")))
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_gif, parent, false);
                        GifViewHolder viewHolder = new GifViewHolder(row);
                        return viewHolder;
                    }
                    else if (itemView.type == "right_text")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_view, parent, false);
                        TextViewHolder TextViewHolder = new TextViewHolder(row, OnLongClick);
                        return TextViewHolder;
                    }
                    else if (itemView.type == "left_text")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_view, parent, false);
                        TextViewHolder TextViewHolder = new TextViewHolder(row, OnLongClick);
                        return TextViewHolder;
                    }
                    else if (itemView.type == "right_image")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_image, parent, false);
                        ImageViewHolder ImageViewHolder = new ImageViewHolder(row);
                        return ImageViewHolder;
                    }
                    else if (itemView.type == "left_image")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_image, parent, false);
                        ImageViewHolder ImageViewHolder = new ImageViewHolder(row);
                        return ImageViewHolder;
                    }
                    else if (itemView.type == "right_Audio" || itemView.type == "right_audio")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Audio, parent, false);
                        SoundViewHolder SoundViewHolder = new SoundViewHolder(row);

                        return SoundViewHolder;
                    }
                    else if (itemView.type == "left_Audio" || itemView.type == "left_audio")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Audio, parent, false);
                        SoundViewHolder SoundViewHolder = new SoundViewHolder(row);
                        return SoundViewHolder;
                    }
                    else if (itemView.type == "right_contact")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Contact, parent, false);
                        ContactViewHolder ContactViewHolder = new ContactViewHolder(row);
                        return ContactViewHolder;
                    }
                    else if (itemView.type == "left_contact")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Contact, parent, false);
                        ContactViewHolder ContactViewHolder = new ContactViewHolder(row);
                        return ContactViewHolder;
                    }
                    else if (itemView.type == "right_video")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Video, parent, false);
                        VideoViewHolder VideoViewHolder = new VideoViewHolder(row);
                        return VideoViewHolder;
                    }
                    else if (itemView.type == "left_video")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Video, parent, false);
                        VideoViewHolder VideoViewHolder = new VideoViewHolder(row);
                        return VideoViewHolder;
                    }
                    else if (itemView.type == "right_sticker")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_sticker, parent, false);
                        StickerViewHolder StickerViewHolder = new StickerViewHolder(row);
                        return StickerViewHolder;
                    }
                    else if (itemView.type == "left_sticker")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_sticker, parent, false);
                        StickerViewHolder StickerViewHolder = new StickerViewHolder(row);
                        return StickerViewHolder;
                    }
                    else if (itemView.type == "right_file")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_file, parent, false);
                        FileViewHolder viewHolder = new FileViewHolder(row);
                        return viewHolder;
                    }
                    else if (itemView.type == "left_file")
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_file, parent, false);
                        FileViewHolder viewHolder = new FileViewHolder(row);
                        return viewHolder;
                    }
                    else
                    {
                        View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_notsupported, parent, false);
                        NotsupportedViewHolder viewHolder = new NotsupportedViewHolder(row);
                        return viewHolder;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder vh, int position)
        {
            try
            {
                int type = GetItemViewType(position);
                var item = mmessage[type];

                if (item != null)
                {
                    if (item.type == "right_gif" || item.type == "left_gif" || (item.text == "" && (item.type == "right_text" || item.type == "left_text") && item.stickers.Contains(".gif")))
                    {
                        GifViewHolder holder = vh as GifViewHolder;
                        LoadGifOfChatItem(holder, item);
                    }
                    else if (item.type == "right_text" || item.type == "left_text")
                    {
                        TextViewHolder holder = vh as TextViewHolder;
                        LoadTextOfchatItem(holder, item);
                    }
                    else if (item.type == "right_image" || item.type == "left_image")
                    {
                        ImageViewHolder holder = vh as ImageViewHolder;
                        LoadImageOfChatItem(holder, item);
                    }
                    else if (item.type == "right_Audio" || item.type == "left_Audio" || item.type == "right_audio" ||
                             item.type == "left_audio")
                    {
                        SoundViewHolder holder = vh as SoundViewHolder;
                        LoadAudioOfChatItem(holder, item);
                    }
                    else if (item.type == "right_contact" || item.type == "left_contact")
                    {
                        ContactViewHolder holder = vh as ContactViewHolder;
                        LoadContactOfChatItem(holder, item);
                    }
                    else if (item.type == "right_video" || item.type == "left_video")
                    {
                        VideoViewHolder holder = vh as VideoViewHolder;
                        LoadVideoOfChatItem(holder, item);
                    }
                    else if (item.type == "right_sticker" || item.type == "left_sticker")
                    {
                        StickerViewHolder holder = vh as StickerViewHolder;
                        LoadStickerOfChatItem(holder, item);
                    }
                    else if (item.type == "right_file" || item.type == "left_file")
                    {
                        FileViewHolder holder = vh as FileViewHolder;
                        LoadFileOfChatItem(holder, item);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.text))
                        {
                            TextViewHolder holderText = vh as TextViewHolder;
                            if (holderText != null)
                            {
                                holderText.AutoLinkTextView.Text = item.text;
                                holderText.Time.Text = item.time_text;
                            }
                        }
                        else
                        {
                            NotsupportedViewHolder holder = vh as NotsupportedViewHolder;
                            if (holder != null)
                                holder.AutoLinkNotsupportedView.Text = Main_Activity.GetText(Resource.String.Lbl_TextChatNotSupported);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //============================== Function Message ==============================

        #region Function Message

        public void Add(Classes.Message Message)
        {
            try
            {
                var check = mmessage.FirstOrDefault(a => a.M_id == Message.M_id);
                if (check == null)
                {
                    mmessage.Add(Message);
                    NotifyItemInserted(mmessage.IndexOf(mmessage.Last()));
                    //Scroll Down >> 
                    Main_Activity.ChatBoxListView.ScrollToPosition(mmessage.IndexOf(mmessage.Last()));

                    NotifyDataSetChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Insert(Classes.Message Message, string firstid)
        {
            try
            {
                var check = mmessage.FirstOrDefault(a => a.M_id == Message.M_id);
                if (check == null)
                {
                    mmessage.Insert(0, Message);
                    NotifyItemInserted(mmessage.IndexOf(mmessage.FirstOrDefault()));
                    //NotifyDataSetChanged();

                    var dd = mmessage.FirstOrDefault(a => a.M_id == firstid);
                    if (dd != null)
                    {
                        //Scroll Down >> 
                        Main_Activity.ChatBoxListView.ScrollToPosition(mmessage.IndexOf(dd));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void UpdateAll()
        {
            try
            {
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Update(Classes.Message Message)
        {
            try
            {
                var cheker = mmessage.FirstOrDefault(a => a.M_id == Message.M_id);
                if (cheker != null)
                {
                    cheker.M_id = Message.M_id;
                    cheker.from_id = Message.from_id;
                    cheker.group_id = Message.group_id;
                    cheker.to_id = Message.to_id;
                    cheker.text = Message.text;
                    cheker.media = Message.media;
                    cheker.mediaFileName = Message.mediaFileName;
                    cheker.mediaFileNames = Message.mediaFileNames;
                    cheker.time = Message.time;
                    cheker.seen = Message.seen;
                    cheker.deleted_one = Message.deleted_one;
                    cheker.deleted_two = Message.deleted_two;
                    cheker.sent_push = Message.sent_push;
                    cheker.notification_id = Message.notification_id;
                    cheker.type_two = Message.type_two;
                    cheker.stickers = Message.stickers;
                    cheker.time_text = Message.time_text;
                    cheker.position = Message.position;
                    cheker.type = Message.type;
                    cheker.file_size = Message.file_size;
                    cheker.avatar = Message.avatar;
                    cheker.MediaDuration = Message.MediaDuration;
                    cheker.Media_IsPlaying = Message.Media_IsPlaying;
                    cheker.ContactNumber = Message.ContactNumber;
                    cheker.ContactName = Message.ContactName;

                    Main_Activity.RunOnUiThread(() =>
                    {
                        NotifyItemChanged(mmessage.IndexOf(cheker));
                        Main_Activity.ChatBoxListView.ScrollToPosition(mmessage.IndexOf(mmessage.Last()));
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveAll()
        {
            try
            {
                mmessage.Clear();

                NotifyDataSetChanged();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        //==============================

        #region Function Load Message

        public void LoadTextOfchatItem(TextViewHolder holder, Classes.Message Message)
        {
            try
            {
                if (holder.Time.Text != Message.time_text)
                {
                    holder.Time.Text = Message.time_text;

                    if (Message.position == "left")
                    {
                        holder.AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail,
                            AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                        holder.AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.left_ModePhone_color));
                        holder.AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.left_ModeEmail_color));
                        holder.AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.left_ModeHashtag_color));
                        holder.AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.left_ModeUrl_color));
                        holder.AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.left_ModeMention_color));
                        holder.AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;
                    }
                    else
                    {
                        holder.AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail,
                            AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                        holder.AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.right_ModePhone_color));
                        holder.AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.right_ModeEmail_color));
                        holder.AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.right_ModeHashtag_color));
                        holder.AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.right_ModeUrl_color));
                        holder.AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(Main_Activity,
                            Resource.Color.right_ModeMention_color));
                        holder.AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;

                    }

                    holder.AutoLinkTextView.SetAutoLinkText(Message.text);
         
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AutoLinkTextViewOnAutoLinkOnClick(object sender1, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                var typetext = IMethods.Fun_String.Check_Regex(autoLinkOnClickEventArgs.P1);
                if (typetext == "Email")
                {
                    IMethods.IApp.SendEmail(Main_Activity, autoLinkOnClickEventArgs.P1);
                }
                else if (typetext == "Website")
                {
                    var url = autoLinkOnClickEventArgs.P1;
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1;
                    }

                    IMethods.IApp.OpenWebsiteUrl(Main_Activity, url);
                }
                else if (typetext == "Hashtag")
                {
                
                }
                else if (typetext == "Mention")
                {
                    var intent = new Intent(Main_Activity, typeof(OnlineSearch_Activity));
                    intent.PutExtra("Key", autoLinkOnClickEventArgs.P1.Replace("@", ""));
                    Main_Activity.StartActivity(intent);
                }
                else if (typetext == "Number")
                {
                    IMethods.IApp.SaveContacts(Main_Activity, autoLinkOnClickEventArgs.P1, "", "2");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadImageOfChatItem(ImageViewHolder holder, Classes.Message Message)
        {
            try
            {
                if (holder.ImageViewAsync.Tag?.ToString() != "loaded")
                {
                    holder.ImageViewAsync.Tag = "loaded";

                    string imageUrl = Message.media;
                    string FileSavedPath = "";
                    var ImageTrancform = ImageService.Instance.LoadUrl(imageUrl);

                    holder.Time.Text = Message.time_text;

                    if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                    {
                        var fileName = imageUrl.Split('/').Last();
                        string ImageFile = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimImage, fileName);

                        if (ImageFile == "File Dont Exists")
                        {
                            ImageCacheLoader.LoadImage("no_profile_image.png", holder.ImageViewAsync, false, false);
                          

                            holder.loadingProgressview.Indeterminate = false;
                            holder.loadingProgressview.Visibility = ViewStates.Visible;

                            string filePath = Path.Combine(IMethods.IPath.FolderDcimMyApp);
                            string MediaFile = filePath + "/" + fileName;
                            FileSavedPath = MediaFile;

                            WebClient WebClient = new WebClient();

                            WebClient.DownloadDataAsync(new Uri(imageUrl));
                            WebClient.DownloadProgressChanged += (sender, args) =>
                            {
                                var progress = args.ProgressPercentage;
                               
                            };

                            WebClient.DownloadDataCompleted += (s, e) =>
                            {
                                try
                                {
                                    if (!Directory.Exists(filePath))
                                        Directory.CreateDirectory(filePath);

                                    File.WriteAllBytes(MediaFile, e.Result);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }

                                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(MediaFile)));
                                Main_Activity.SendBroadcast(mediaScanIntent);

                                ImageCacheLoader.LoadImage(imageUrl, holder.ImageViewAsync, false, false);

                               

                                holder.loadingProgressview.Indeterminate = false;
                                holder.loadingProgressview.Visibility = ViewStates.Gone;
                            };
                        }
                        else
                        {
                            FileSavedPath = ImageFile;

                            ImageCacheLoader.LoadImage(imageUrl, holder.ImageViewAsync, false, false);

                            holder.loadingProgressview.Indeterminate = false;
                            holder.loadingProgressview.Visibility = ViewStates.Gone;
                        }
                    }
                    else
                    {
                        FileSavedPath = imageUrl;

                        var url = Android.Net.Uri.FromFile(new Java.IO.File(imageUrl));

                        ImageCacheLoader.LoadImage(url.Path, holder.ImageViewAsync, false, false);

                      

                        holder.loadingProgressview.Indeterminate = false;
                        holder.loadingProgressview.Visibility = ViewStates.Gone;
                    }

                    if (!holder.ImageViewAsync.HasOnClickListeners)
                    {
                        holder.ImageViewAsync.Click += (sender, args) =>
                        {
                            try
                            {
                                string ImageFile = IMethods.MultiMedia.CheckFileIfExits(FileSavedPath);

                                if (ImageFile != "File Dont Exists")
                                {
                                    Java.IO.File file2 = new Java.IO.File(FileSavedPath);
                                    var photoURI = FileProvider.GetUriForFile(Main_Activity, Main_Activity.PackageName + ".fileprovider", file2);

                                    Intent intent = new Intent();
                                    intent.SetAction(Intent.ActionView);
                                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                    intent.SetDataAndType(photoURI, "image/*");
                                    Main_Activity.StartActivity(intent);
                                }
                                else
                                {
                                    Toast.MakeText(Main_Activity, this.Main_Activity.GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadAudioOfChatItem(SoundViewHolder soundViewHolder, Classes.Message message)
        {
            try
            {
                if (message.SoundViewHolder == null)
                    message.SoundViewHolder = soundViewHolder;

                if (message.time_text  == this.Main_Activity.GetText(Resource.String.Lbl_Uploading))
                {
                    soundViewHolder.LoadingProgressview.Visibility = ViewStates.Visible;
                    soundViewHolder.PlayButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    soundViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                    soundViewHolder.PlayButton.Visibility = ViewStates.Visible;
                }

                message.SoundViewHolder.MsgTimeTextView.Text = message.time_text;

                var fileName = message.media.Split('/').Last();
                var mediaFile = IMethods.AudioRecorderAndPlayer.Check_Sound_File_if_Exits(IMethods.IPath.FolderDcimSound, fileName);
                if (mediaFile == "File Doesn't Exist" && (message.media.Contains("http")))
                {
                    WebClient WebClient = new WebClient();
                    message.SoundViewHolder.LoadingProgressview.Visibility = ViewStates.Visible;
                    message.SoundViewHolder.PlayButton.Visibility = ViewStates.Gone;
                    WebClient.DownloadDataAsync(new Uri(message.media));

                    WebClient.DownloadProgressChanged += (sender, args) =>
                    {
                        try
                        {
                            var progress = args.ProgressPercentage;
                            soundViewHolder.LoadingProgressview.Visibility = ViewStates.Visible;
                            soundViewHolder.PlayButton.Visibility = ViewStates.Gone;

                            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                            {
                                soundViewHolder.LoadingProgressview.SetProgress(progress, true);
                            }
                            else
                            {
                                soundViewHolder.LoadingProgressview.Progress = progress;
                            } 
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        } 
                    };

                    WebClient.DownloadDataCompleted += (s, e) =>
                    {
                        try
                        {
                            soundViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                            soundViewHolder.PlayButton.Visibility = ViewStates.Visible;

                            var Dir = IMethods.IPath.FolderDcimSound;
                            if (!Directory.Exists(Dir))
                                Directory.CreateDirectory(Dir);

                            mediaFile = Dir + "/" + fileName;
                            File.WriteAllBytes(mediaFile, e.Result);
                            message.SoundViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                            message.SoundViewHolder.PlayButton.Visibility = ViewStates.Visible;
                            message.SoundViewHolder.DurationTextView.Text = IMethods.AudioRecorderAndPlayer.GetTimeString(IMethods.AudioRecorderAndPlayer.Get_MediaFileDuration(mediaFile));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }; 
                }
                else
                {
                    message.SoundViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                    message.SoundViewHolder.PlayButton.Visibility = ViewStates.Visible;
                    message.SoundViewHolder.DurationTextView.Text = message.MediaDuration;
                    message.SoundViewHolder.MsgTimeTextView.Text = message.time_text;

                    if (string.IsNullOrEmpty(message.MediaDuration))
                        message.SoundViewHolder.DurationTextView.Text =IMethods.AudioRecorderAndPlayer.GetTimeString(IMethods.AudioRecorderAndPlayer.Get_MediaFileDuration(mediaFile));
                }

                if (!soundViewHolder.PlayButton.HasOnClickListeners)
                {
                    soundViewHolder.PlayButton.Click += (sender, args) =>
                    {
                        try
                        {
                            if (message.MediaPlayer == null)
                                message.MediaPlayer = new Android.Media.MediaPlayer();

                            if (soundViewHolder.PlayButton.Tag.ToString() == "Play")
                            {
                                try
                                {
                                    if (message.type == "left_Audio" || message.type == "right_Audio" ||
                                        message.type == "right_audio" || message.type == "left_audio")
                                    {
                                        if (message.type == "left_Audio" || message.type == "left_audio")
                                        {
                                            message.SoundViewHolder.PlayButton.SetImageResource(Resource.Drawable
                                                .ic_play_dark_arrow);
                                        }
                                        else
                                        {
                                            message.SoundViewHolder.PlayButton.SetImageResource(Resource.Drawable
                                                .ic_play_arrow);
                                        }

                                        try
                                        {
                                            if (message.MediaPlayer.IsPlaying)
                                            {
                                                message.MediaPlayer.Stop();
                                                message.MediaPlayer.Release();
                                                message.MediaPlayer = null;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                            message.MediaPlayer = null;
                                        }

                                        message.SoundViewHolder.PlayButton.Tag = "Play";
                                    }

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }

                                try
                                {
                                    if (message.type == "left_Audio" || message.type == "left_audio")
                                    {
                                        message.SoundViewHolder.PlayButton.SetImageResource(Resource.Drawable.ic_stop_dark_arrow);
                                    }
                                    else
                                    {
                                        soundViewHolder.PlayButton.SetImageResource(Resource.Drawable.ic_stop_white_24dp);
                                    }


                                    if (message.MediaPlayer != null)
                                    {
                                        message.MediaPlayer.Completion += (sender_, e) =>
                                        {
                                            message.MediaPlayer.Reset();
                                        };

                                        message.MediaPlayer.SetDataSource(mediaFile);
                                        message.MediaPlayer.Prepare();
                                        message.MediaPlayer.Prepared += (s, ee) =>
                                        {
                                            message.Media_IsPlaying = true;
                                            soundViewHolder.PlayButton.Tag = "Stop";

                                            if (message.MediaTimer == null)
                                                message.MediaTimer = new Timer();

                                            message.MediaTimer.Interval = 1000;
                                            message.MediaPlayer.Start();
                                            var DurationofSound = message.MediaPlayer.Duration;

                                            message.MediaTimer.Elapsed += (o, eventArgs) =>
                                            {
                                                if (message.MediaTimer.Enabled)
                                                {

                                                    if (message.MediaPlayer.CurrentPosition <
                                                        message.MediaPlayer.Duration)
                                                    {
                                                        message.SoundViewHolder.DurationTextView.Text =
                                                            IMethods.AudioRecorderAndPlayer.GetTimeString(
                                                                message.MediaPlayer.CurrentPosition);
                                                    }
                                                    else
                                                    {
                                                        message.SoundViewHolder.DurationTextView.Text =
                                                            IMethods.AudioRecorderAndPlayer.GetTimeString(
                                                                DurationofSound);
                                                        message.SoundViewHolder.PlayButton.Tag = "Play";
                                                        if (message.type == "left_Audio")
                                                        {
                                                            message.SoundViewHolder.PlayButton.SetImageResource(
                                                                Resource.Drawable.ic_play_dark_arrow);
                                                        }
                                                        else
                                                        {
                                                            message.SoundViewHolder.PlayButton.SetImageResource(
                                                                Resource.Drawable.ic_play_arrow);
                                                            message.MediaTimer.Enabled = false;
                                                            message.MediaTimer.Stop();
                                                            try
                                                            {
                                                                if (message.MediaPlayer.IsPlaying)
                                                                {
                                                                    message.Media_IsPlaying = false;
                                                                    message.MediaPlayer.Stop();
                                                                    message.MediaPlayer.Release();

                                                                }
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                message.SoundViewHolder.PlayButton.Tag = "Play";
                                                                Console.WriteLine(e);
                                                            }
                                                        }
                                                    }
                                                }
                                            };
                                            message.MediaTimer.Start();
                                        };
                                    }
                                    else
                                    {
                                        var AudioRecordAndPlayer =
                                            new IMethods.AudioRecorderAndPlayer(AppSettings.Application_Name);
                                        AudioRecordAndPlayer.PlayAudioFromPath(fileName);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);

                                }

                               
                            }
                            else
                            {
                                soundViewHolder.PlayButton.Tag = "Play";
                                if (message.type == "left_Audio" || message.type == "left_audio")
                                {
                                    message.SoundViewHolder.PlayButton.SetImageResource(Resource.Drawable
                                        .ic_play_dark_arrow);
                                }
                                else
                                {
                                    message.SoundViewHolder.PlayButton.SetImageResource(Resource.Drawable
                                        .ic_play_arrow);
                                }

                                message.Media_IsPlaying = false;
                                message.MediaTimer.Enabled = false;
                                message.MediaTimer.Stop();
                                soundViewHolder.DurationTextView.Text =
                                    IMethods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.Duration);
                                try
                                {
                                    if (message.MediaPlayer.IsPlaying)
                                    {
                                        message.Media_IsPlaying = false;
                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                        message.MediaPlayer.Stop();
                                        message.MediaPlayer.Release();
                                    }
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
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadContactOfChatItem(ContactViewHolder holder, Classes.Message Message)
        {
            try
            {
                if (holder.UserContactNameTextView.Text != Message.ContactName)
                {
                    holder.MsgTimeTextView.Text = Message.time_text;

                 

                    if (!string.IsNullOrEmpty(Message.ContactName))
                    {
                        holder.UserContactNameTextView.Text = Message.ContactName;
                        holder.UserNumberTextView.Text = Message.ContactNumber;
                    }

                    if (!holder.MainView.HasOnClickListeners)
                    {
                        if (Message.position == "left")
                        {
                            holder.MainView.Click += (sender, args) =>
                            {
                                try
                                {
                                    IMethods.IApp.SaveContacts(Main_Activity, holder.UserNumberTextView.Text,
                                        holder.UserContactNameTextView.Text, "2");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            };
                        }
                        else
                        {
                            holder.MainView.Click += (sender, args) =>
                            {
                                try
                                {
                                    IMethods.IApp.SaveContacts(Main_Activity, holder.UserNumberTextView.Text,
                                        holder.UserContactNameTextView.Text, "2");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadVideoOfChatItem(VideoViewHolder holder, Classes.Message Message)
        {
            try
            {
                if (holder.MsgTimeTextView.Text != Message.time_text)
                {
                    var fileName = Message.media.Split('/').Last();
                    var FileNameWithoutExtenion = fileName.Split('.').First();
                    var MediaFile = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimVideo, fileName);

                    holder.MsgTimeTextView.Text = Message.time_text;
                    holder.FilenameTextView.Text = IMethods.Fun_String.SubStringCutOf(FileNameWithoutExtenion, 10) + ".mp4";

                    if (MediaFile == "File Dont Exists" && (Message.media.Contains("http://") || Message.media.Contains("https://")))
                    {
                        holder.PlayButton.Visibility = ViewStates.Gone;
                        holder.loadingProgressview.Indeterminate = false;
                        holder.loadingProgressview.Visibility = ViewStates.Visible;

                        IMethods.Set_TextViewIcon("1", holder.IconView, IonIcons_Fonts.Videocamera);

                        ImageCacheLoader.LoadImage("ImagePlacholder.jpg", holder.ImageViewAsync, false, false);

                       

                        WebClient WebClient = new WebClient();
                        WebClient.DownloadDataAsync(new System.Uri(Message.media));
                        WebClient.DownloadProgressChanged += (sender, args) =>
                        {
                            var progress = args.ProgressPercentage;
                        
                        };

                        WebClient.DownloadDataCompleted += (s, e) =>
                        {
                            try
                            {
                                var Dir = IMethods.IPath.FolderDcimVideo;

                                if (!Directory.Exists(Dir))
                                    Directory.CreateDirectory(Dir);

                                MediaFile = Dir + "/" + fileName;
                                File.WriteAllBytes(MediaFile, e.Result);

                                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(MediaFile)));
                                Main_Activity.SendBroadcast(mediaScanIntent);

                                var BitmapImage = IMethods.MultiMedia.Retrieve_VideoFrame_AsBitmap(MediaFile);
                                IMethods.MultiMedia.Export_Bitmap_As_Image(BitmapImage, FileNameWithoutExtenion, Dir);

                                var file = Android.Net.Uri.FromFile(new Java.IO.File(Dir + "/" + FileNameWithoutExtenion + ".png"));

                                ImageCacheLoader.LoadImage(file.Path, holder.ImageViewAsync, false, false);

                               

                                holder.loadingProgressview.Indeterminate = false;
                                holder.loadingProgressview.Visibility = ViewStates.Gone;
                                holder.PlayButton.Visibility = ViewStates.Visible;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        };
                    }
                    else
                    {
                        var VidoPlaceHolderImage = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimVideo, FileNameWithoutExtenion + ".png");
                        if (VidoPlaceHolderImage == "File Dont Exists")
                        {
                            var BitmapImage = IMethods.MultiMedia.Retrieve_VideoFrame_AsBitmap(MediaFile);
                            IMethods.MultiMedia.Export_Bitmap_As_Image(BitmapImage, FileNameWithoutExtenion, IMethods.IPath.FolderDcimVideo);
                        }

                        IMethods.Set_TextViewIcon("1", holder.IconView, IonIcons_Fonts.Videocamera);

                        var file = Android.Net.Uri.FromFile(new Java.IO.File(IMethods.IPath.FolderDcimVideo + "/" + FileNameWithoutExtenion + ".png"));

                        ImageCacheLoader.LoadImage(file.Path, holder.ImageViewAsync, false, false);

                    
                    }

                    if (!holder.PlayButton.HasOnClickListeners)
                    {
                        holder.PlayButton.Click += (sender, args) =>
                        {
                            try
                            {
                                string ImageFile = IMethods.MultiMedia.CheckFileIfExits(MediaFile);

                                if (ImageFile != "File Dont Exists")
                                {
                                    Java.IO.File file2 = new Java.IO.File(MediaFile);
                                    var MediaURI = FileProvider.GetUriForFile(Main_Activity,
                                        Main_Activity.PackageName + ".fileprovider", file2);

                                    Intent intent = new Intent();
                                    intent.SetAction(Intent.ActionView);
                                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                    intent.SetDataAndType(MediaURI, "video/*");
                                    Main_Activity.StartActivity(intent);
                                }
                                else
                                {
                                    Toast.MakeText(Main_Activity,
                                        this.Main_Activity.GetText(Resource.String.Lbl_Something_went_wrong),
                                        ToastLength.Long).Show();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadStickerOfChatItem(StickerViewHolder holder, Classes.Message Message)
        {
            try
            {
                string imageUrl = Message.media;
                string FileSavedPath = "";

                var ImageTrancform = ImageService.Instance.LoadUrl(imageUrl);
                holder.Time.Text = Message.time_text;
                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    var fileName = imageUrl.Split('_').Last();
                    string ImageFile = IMethods.MultiMedia.GetMediaFrom_Disk(IMethods.IPath.FolderDiskSticker, fileName);
                    if (ImageFile == "File Dont Exists")
                    {
                        holder.loadingProgressview.Indeterminate = false;
                        holder.loadingProgressview.Visibility = ViewStates.Visible;

                        IMethods.MultiMedia.DownloadMediaTo_DiskAsync(IMethods.IPath.FolderDiskSticker, imageUrl);

                        ImageCacheLoader.LoadImage(imageUrl, holder.ImageViewAsync, false, false);

                       

                        holder.loadingProgressview.Indeterminate = false;
                        holder.loadingProgressview.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        FileSavedPath = ImageFile;

                        ImageCacheLoader.LoadImage(FileSavedPath, holder.ImageViewAsync, false, false);

                      

                        holder.loadingProgressview.Indeterminate = false;
                        holder.loadingProgressview.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    FileSavedPath = imageUrl;

                    ImageCacheLoader.LoadImage(FileSavedPath, holder.ImageViewAsync, false, false);

                    

                    holder.loadingProgressview.Indeterminate = false;
                    holder.loadingProgressview.Visibility = ViewStates.Gone;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadFileOfChatItem(FileViewHolder holder, Classes.Message Message)
        {
            try
            {
                var fileName = Message.media.Split('/').Last();
                var FileNameWithoutExtenion = fileName.Split('.').First();
                var FileNameExtenion = fileName.Split('.').Last();
                var MediaFile = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimFile, fileName);

                holder.MsgTimeTextView.Text = Message.time_text;
                holder.FileNameTextView.Text = IMethods.Fun_String.SubStringCutOf(FileNameWithoutExtenion, 10) + FileNameExtenion;
                holder.SizeFileTextView.Text = Message.file_size;

                if (FileNameExtenion.Contains("rar") || FileNameExtenion.Contains("RAR") ||
                    FileNameExtenion.Contains("zip") || FileNameExtenion.Contains("ZIP"))
                {
                    IMethods.Set_TextViewIcon("2", holder.Icon_typefile, "\uf1c6"); //ZipBox
                }
                else if (FileNameExtenion.Contains("txt") || FileNameExtenion.Contains("TXT"))
                {
                    IMethods.Set_TextViewIcon("2", holder.Icon_typefile, "\uf15c"); //NoteText
                }
                else if (FileNameExtenion.Contains("docx") || FileNameExtenion.Contains("DOCX") ||
                         FileNameExtenion.Contains("doc") || FileNameExtenion.Contains("DOC"))
                {
                    IMethods.Set_TextViewIcon("2", holder.Icon_typefile, "\uf1c2"); //FileWord
                }
                else if (FileNameExtenion.Contains("pdf") || FileNameExtenion.Contains("PDF"))
                {
                    IMethods.Set_TextViewIcon("2", holder.Icon_typefile, "\uf1c1"); //FilePdf
                }
                else if (FileNameExtenion.Contains("apk") || FileNameExtenion.Contains("APK"))
                {
                    IMethods.Set_TextViewIcon("2", holder.Icon_typefile, "\uf17b"); //Fileandroid
                }
                else
                {
                    IMethods.Set_TextViewIcon("2", holder.Icon_typefile, "\uf15b"); //file
                }

                if (MediaFile == "File Dont Exists" &&
                    (Message.media.Contains("http://") || Message.media.Contains("https://")))
                {
                    WebClient WebClient = new WebClient();
                    WebClient.DownloadDataAsync(new System.Uri(Message.media));
                    WebClient.DownloadProgressChanged += (sender, args) =>
                    {
                        var progress = args.ProgressPercentage;
                     
                    };

                    WebClient.DownloadDataCompleted += (s, e) =>
                    {
                        try
                        {
                            var Dir = IMethods.IPath.FolderDcimFile;

                            if (!Directory.Exists(Dir))
                                Directory.CreateDirectory(Dir);

                            MediaFile = Dir + "/" + fileName;
                            File.WriteAllBytes(MediaFile, e.Result);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    };
                }
                else
                {
                    var fileHolder = IMethods.MultiMedia.GetMediaFrom_Gallery(IMethods.IPath.FolderDcimFile, fileName);
                    var Dir = IMethods.IPath.FolderDcimFile;
                    if (fileHolder == "File Dont Exists")
                    {

                    }
                }

                if (!holder.MainView.HasOnClickListeners)
                {
                    holder.MainView.Click += (sender, args) =>
                    {
                        string ImageFile = IMethods.MultiMedia.CheckFileIfExits(MediaFile);

                        if (ImageFile != "File Dont Exists")
                        {
                            try
                            {
                                var Extenion = fileName.Split('.').Last();
                                string MimeType = MimeTypeMap.GetMimeType(Extenion);

                                Intent openFile = new Intent();
                                openFile.SetFlags(ActivityFlags.NewTask);
                                openFile.SetFlags(ActivityFlags.GrantReadUriPermission);
                                openFile.SetAction(Intent.ActionView);
                                openFile.SetDataAndType(Android.Net.Uri.Parse("content://" + MediaFile), MimeType);
                                Main_Activity.StartActivity(openFile);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        else
                        {
                            Toast.MakeText(Main_Activity, this.Main_Activity.GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadGifOfChatItem(GifViewHolder holder, Classes.Message item)
        {
            try
            {
                // G_fixed_height_small_url, // UrlGif - view  >>  mediaFileName
                // G_fixed_height_small_mp4, //MediaGif - sent >>  media

                if (!string.IsNullOrEmpty(item.mediaFileName))
                    ImageServiceLoader.Load_Image(holder.ImageGifView, "ImagePlacholder.jpg", item.mediaFileName, 2);
                else if (!string.IsNullOrEmpty(item.media))
                    ImageServiceLoader.Load_Image(holder.ImageGifView, "ImagePlacholder.jpg", item.media, 2);
                else if (!string.IsNullOrEmpty(item.stickers))
                    ImageServiceLoader.Load_Image(holder.ImageGifView, "ImagePlacholder.jpg", item.stickers, 2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadNotsupportedOfchatItem(NotsupportedViewHolder holder, Classes.Message Message)
        {
            try
            {
                holder.AutoLinkNotsupportedView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                holder.AutoLinkNotsupportedView.SetHashtagModeColor(Android.Graphics.Color.Blue);
                holder.AutoLinkNotsupportedView.SetUrlModeColor(Android.Graphics.Color.Blue);
                holder.AutoLinkNotsupportedView.SetPhoneModeColor(Android.Graphics.Color.SeaGreen);
                holder.AutoLinkNotsupportedView.SetEmailModeColor(ContextCompat.GetColor(Main_Activity, Resource.Color.main_color));

                holder.AutoLinkNotsupportedView.AutoLinkOnClick += AutoLinkNotsupportedViewOnAutoLinkOnClick;
                holder.AutoLinkNotsupportedView.SetAutoLinkText(Message.text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AutoLinkNotsupportedViewOnAutoLinkOnClick(object sender1,
            AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                Toast.MakeText(Main_Activity, this.Main_Activity.GetText(Resource.String.Lbl_TextChatNotSupported),
                    ToastLength.Long).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        //==============================
        public void BindEnd()
        {
            try
            {
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Clear()
        {
            try
            {
                mmessage.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount
        {
            get
            {
                try
                {
                    if (mmessage == null || mmessage.Count <= 0)
                        return 0;
                    return mmessage.Count;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return 0;
                }
            }
        }

        public Classes.Message GetItem(int position)
        {
            return mmessage[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        void OnClick(Message_AdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(Message_AdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    #region Holders

    public class TextViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public TextView Time { get; private set; }
        public View MainView { get; private set; }
        public EventHandler ClickHandler { get; set; }
        public AutoLinkTextView AutoLinkTextView { get; set; }

        #endregion

        public TextViewHolder(View itemView, Action<Message_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                AutoLinkTextView = itemView.FindViewById<AutoLinkTextView>(Resource.Id.active);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);

                AutoLinkTextView.LongClick += (sender, args) => longClickListener(new Message_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, args) => longClickListener(new Message_AdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Error");
            }
        }
    }

    public class ImageViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public EventHandler ClickHandler { get; set; }
        public ImageView ImageViewAsync { get; private set; }
        public ProgressBar loadingProgressview { get; set; }
        public TextView Time { get; private set; }

        #endregion

        public ImageViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                ImageViewAsync = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                loadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class SoundViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public TextView DurationTextView { get; set; }
        public TextView MsgTimeTextView { get; set; }
        public CircleButton PlayButton { get; set; }
        public ProgressBar LoadingProgressview { get; set; }

        #endregion

        public SoundViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                DurationTextView = itemView.FindViewById<TextView>(Resource.Id.Duration);
                PlayButton = itemView.FindViewById<CircleButton>(Resource.Id.playButton);
                MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                LoadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class ContactViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public TextView UserContactNameTextView { get; set; }
        public TextView UserNumberTextView { get; set; }
        public TextView MsgTimeTextView { get; set; }
        public CircleImageView profile_image { get; set; }

        #endregion

        public ContactViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                UserContactNameTextView = itemView.FindViewById<TextView>(Resource.Id.contactName);
                UserNumberTextView = itemView.FindViewById<TextView>(Resource.Id.numberText);
                MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                profile_image = itemView.FindViewById<CircleImageView>(Resource.Id.profile_image);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class VideoViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public EventHandler ClickHandler { get; set; }
        public ImageView ImageViewAsync { get; private set; }
        public ProgressBar loadingProgressview { get; set; }
        public TextView MsgTimeTextView { get; set; }
        public TextView IconView { get; set; }
        public TextView FilenameTextView { get; set; }
        public CircleButton PlayButton { get; set; }

        #endregion

        public VideoViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                ImageViewAsync = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                loadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                IconView = itemView.FindViewById<TextView>(Resource.Id.icon);
                FilenameTextView = itemView.FindViewById<TextView>(Resource.Id.fileName);
                PlayButton = itemView.FindViewById<CircleButton>(Resource.Id.playButton);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class StickerViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public EventHandler ClickHandler { get; set; }
        public ImageView ImageViewAsync { get; private set; }
        public ProgressBar loadingProgressview { get; set; }
        public TextView Time { get; private set; }

        #endregion

        public StickerViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                ImageViewAsync = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                loadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);

              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class GifViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public EventHandler ClickHandler { get; set; }
        public ImageViewAsync ImageGifView { get; private set; }
        public ProgressBar loadingProgressview { get; set; }
        public TextView Time { get; private set; }

        #endregion

        public GifViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                ImageGifView = itemView.FindViewById<ImageViewAsync>(Resource.Id.imggifdisplay);
                loadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                Time = itemView.FindViewById<TextView>(Resource.Id.time);

            
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class NotsupportedViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public EventHandler ClickHandler { get; set; }
        public AutoLinkTextView AutoLinkNotsupportedView { get; set; }

        #endregion

        public NotsupportedViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;

                AutoLinkNotsupportedView = itemView.FindViewById<AutoLinkTextView>(Resource.Id.active);
                AutoLinkNotsupportedView.Text = itemView.Context.GetText(Resource.String.Lbl_TextChatNotSupported);
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Error");
            }
        }
    }

    public class FileViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }
        public TextView FileNameTextView { get; set; }
        public TextView SizeFileTextView { get; set; }
        public TextView MsgTimeTextView { get; set; }
        public AppCompatTextView Icon_typefile { get; set; }

        #endregion

        public FileViewHolder(View itemView) : base(itemView)
        {
            try
            {
                MainView = itemView;
                FileNameTextView = itemView.FindViewById<TextView>(Resource.Id.fileName);
                SizeFileTextView = itemView.FindViewById<TextView>(Resource.Id.sizefileText);
                MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                Icon_typefile = itemView.FindViewById<AppCompatTextView>(Resource.Id.Icontypefile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    #endregion

    public class Message_AdapterViewHolder : RecyclerView.ViewHolder
    {
     

        public Message_AdapterViewHolder(View itemView, Action<Message_AdapterClickEventArgs> clickListener,
            Action<Message_AdapterClickEventArgs> longClickListener) : base(itemView)
        {
 
            itemView.Click += (sender, e) => clickListener(new Message_AdapterClickEventArgs
            { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new Message_AdapterClickEventArgs
            { View = itemView, Position = AdapterPosition });
        }
    }

    public class Message_AdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}