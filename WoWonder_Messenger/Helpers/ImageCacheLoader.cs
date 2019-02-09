using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using UniversalImageLoader.Core;
using UniversalImageLoader.Core.Assist;
using UniversalImageLoader.Core.Display;
using UniversalImageLoader.Core.Listener;
using UniversalImageLoader.Utils;
using Console = System.Console;
using Exception = Java.Lang.Exception;
using Uri = Android.Net.Uri;

namespace WoWonder.Helpers
{
    public class ImageCacheLoader
    {
        public static DisplayImageOptions DefaultOptions;
        public static DisplayImageOptions CircleOptions;
        public static ImageSize SizeMinimized = new ImageSize(30, 30);
        public static void InitImageLoader(Context context)
        {
            try
            {
                SetImageOption();

                ImageLoaderConfiguration config = new ImageLoaderConfiguration.Builder(context)
                    .ThreadPriority(Thread.NormPriority - 2)
                    .TasksProcessingOrder(QueueProcessingType.Lifo)
                    .DefaultDisplayImageOptions(DefaultOptions)
                    .DiskCacheSize(100 * 1024 * 1024) 
                    .Build();

                ImageLoader.Instance.SetDefaultLoadingListener(new ImageLoadingListener());

                if (!ImageLoader.Instance.IsInited)
                    ImageLoader.Instance.Init(config);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SetImageOption()
        {
            try
            {
                DefaultOptions = new DisplayImageOptions.Builder()
                    .ShowImageOnLoading(Resource.Drawable.no_profile_image) // resource or drawable
                    .ShowImageForEmptyUri(Resource.Drawable.no_profile_image) // empty
                    .ShowImageOnFail(Resource.Drawable.no_profile_image)  // error
                    .CacheInMemory(true)
                 
                    .CacheOnDisk(true)
                    .ConsiderExifParams(true)
                    .Displayer(new FadeInBitmapDisplayer(200))
                    
                    .BitmapConfig(Bitmap.Config.Rgb565)
                    .ImageScaleType(ImageScaleType.Exactly)
                    .Build();

                CircleOptions = new DisplayImageOptions.Builder()
                   
                    .CacheInMemory(true)
                    .CacheOnDisk(true)
                    .ConsiderExifParams(true)
                    
                    .BitmapConfig(Bitmap.Config.Rgb565)
                     .Displayer( new RoundedBitmapDisplayer(1000))
                    .ImageScaleType(ImageScaleType.Exactly)
                    .Build(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">any url like websites or disk or storage will work</param>
        /// <param name="image">ImageView view</param>
        /// <param name="downsized"> compress the image to 100 x 100 px</param>
        /// <param name="circle">Set to true with Downsized true if you want to display the image as circle</param>
        public static void LoadImage(string url, ImageView image, bool downsized, bool circle)
        {
            try
            {
                var options = DefaultOptions;

                if (circle)
                    options = CircleOptions;

                if (downsized)
                {
                    ImageLoader.Instance.LoadImage(url, SizeMinimized, options, new WithParamImageLoadingListener(image, circle));
                }
                else
                {
                    if (!string.IsNullOrEmpty(url) && (url.Contains("file://") || url.Contains("content://") || url.Contains("storage")))
                    {
                        ImageLoader.Instance.DisplayImage(url, image, options, new WithParamImageLoadingListener(image, circle));
                    }
                    else
                    {
                      
                            ImageLoader.Instance.DisplayImage(url, image, options);
                            image.Tag = "true";
                       
                    }
                }
                    
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        public class ImageLoadingListener : Java.Lang.Object, IImageLoadingListener
        {
            private bool cacheFound = false;
            public void OnLoadingCancelled(string imageUri, View view)
            {

            }

            public void OnLoadingComplete(string imageUri, View view, Bitmap loadedImage)
            {
                try
                {
                  
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnLoadingFailed(string imageUri, View view, FailReason failReason)
            {

            }

            public void OnLoadingStarted(string imageUri, View view)
            {
                try
                {
                    
                    IList<string> memCache = MemoryCacheUtils.FindCacheKeysForImageUri(imageUri, ImageLoader.Instance.MemoryCache);
                    if (memCache?.Count != 0)
                    {
                        cacheFound = true;
                    }

                    if (!cacheFound)
                    {
                        File discCache = DiskCacheUtils.FindInCache(imageUri, ImageLoader.Instance.DiskCache);
                        if (discCache != null)
                        {
                            
                            cacheFound = discCache.Exists();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class WithParamImageLoadingListener : Java.Lang.Object, IImageLoadingListener
        {
            public ImageView Image;
            public bool  Circle;
            private bool cacheFound = false;

            public WithParamImageLoadingListener(ImageView image, bool circle)
            {
                try
                {
                    Image = image;
                    Circle = circle;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnLoadingCancelled(string imageUri, View view)
            {

            }

            public void OnLoadingComplete(string imageUri, View view, Bitmap loadedImage)
            {
                try
                {
                  
                    if (view != null)
                    {
                      
                    }
                    else
                    {
                        if (Circle)
                        {
                            Bitmap croppedBitmap = GetCroppedBitmap(loadedImage);
                            Image.SetImageBitmap(croppedBitmap);
                        }
                        else
                        {
                            Image.SetImageBitmap(loadedImage);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                } 
            }

            public void OnLoadingFailed(string imageUri, View view, FailReason failReason)
            {
                try
                {
                    GC.Collect();

                    Bitmap myBitmap;
                    Resources res = Application.Context.Resources;

                    if (imageUri.Contains("d-avatar.jpg") || imageUri.Contains("no_profile_image.png") || imageUri.Contains("no_profile_image_circle.png") || imageUri.Contains("d-cover.jpg") ||
                        imageUri.Contains("ImagePlacholder.jpg") || imageUri.Contains("ImagePlacholder_circel.png"))
                    {
                        if (Circle)
                        {
                            if (imageUri.Contains("d-avatar.jpg") || imageUri.Contains("no_profile_image_circle"))
                                myBitmap = BitmapFactory.DecodeResource(res, Resource.Drawable.no_profile_image_circle);
                            else if (imageUri.Contains("no_profile_image.png"))
                                myBitmap = BitmapFactory.DecodeResource(res, Resource.Drawable.no_profile_image);
                            else if (imageUri.Contains("ImagePlacholder.jpg"))
                                myBitmap = BitmapFactory.DecodeResource(res, Resource.Drawable.ImagePlacholder);
                            else if (imageUri.Contains("d-cover.jpg.jpg"))
                                myBitmap = BitmapFactory.DecodeResource(res, Resource.Drawable.bridge);
                            else if (imageUri.Contains("ImagePlacholder_circel.png"))
                                myBitmap = BitmapFactory.DecodeResource(res, Resource.Drawable.ImagePlacholder_circel);
                            else
                                myBitmap = BitmapFactory.DecodeResource(res, Resource.Drawable.no_profile_image);

                            Bitmap croppedBitmap = GetCroppedBitmap(myBitmap);
                            Image.SetImageBitmap(croppedBitmap);
                        }
                        else
                        {
                            if (imageUri.Contains("d-avatar.jpg") || imageUri.Contains("no_profile_image_circle.png"))
                                Image.SetImageResource(Resource.Drawable.no_profile_image_circle);
                            else if (imageUri.Contains("no_profile_image.jpg"))
                                Image.SetImageResource(Resource.Drawable.no_profile_image);
                            else if (imageUri.Contains("ImagePlacholder.jpg"))
                                Image.SetImageResource(Resource.Drawable.ImagePlacholder);
                            else if (imageUri.Contains("ImagePlacholder_circel.png"))
                                Image.SetImageResource(Resource.Drawable.ImagePlacholder_circel);
                            else if (imageUri.Contains("d-cover.jpg.jpg"))
                                Image.SetImageResource(Resource.Drawable.bridge);
                            else
                                Image.SetImageResource(Resource.Drawable.no_profile_image);
                        }
                    }
                    else if (!string.IsNullOrEmpty(imageUri) && imageUri.Contains("http"))
                    {
                        Image.SetImageURI(Uri.Parse(imageUri));
                    }
                    else if (!string.IsNullOrEmpty(imageUri) && (imageUri.Contains("file://") || imageUri.Contains("content://") || imageUri.Contains("storage")))
                    {
                        var file = Android.Net.Uri.FromFile(new Java.IO.File(imageUri));
                        myBitmap = BitmapFactory.DecodeFile(file.Path);
                        if (Circle)
                        {
                            Bitmap croppedBitmap = GetCroppedBitmap(myBitmap);
                            Image.SetImageBitmap(croppedBitmap);
                        }
                        else
                        {
                            try
                            {
                                if (myBitmap != null)
                                {
                                    Image.SetImageBitmap(myBitmap);
                                }
                                else
                                {
                                    BitmapFactory.Options options = new BitmapFactory.Options();
                                    options.InSampleSize = 2;
                                    Bitmap bitmap = BitmapFactory.DecodeFile(file.Path, options);
                                    Image.SetImageBitmap(bitmap);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            catch (OutOfMemoryError ex)
                            {
                                GC.Collect();
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    else
                    {
                        Image.SetImageResource(Resource.Drawable.no_profile_image);
                    }
                    Image.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                } 
            }

            public Bitmap GetCroppedBitmap(Bitmap bitmap)
            {
                try
                {
                    Bitmap output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
                    Canvas canvas = new Canvas(output);

                    Color color = Android.Graphics.Color.ParseColor("#424242");
                    Paint paint = new Paint();
                    Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);

                    paint.AntiAlias = true;
                    canvas.DrawARGB(0, 0, 0, 0);
                    paint.Color = color;
               
                    canvas.DrawCircle(bitmap.Width / 2, bitmap.Height / 2, bitmap.Width / 2, paint);

                    paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                    canvas.DrawBitmap(bitmap, rect, rect, paint);
                  
                    return output;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return bitmap;
                } 
            }

            private Bitmap AddBorderBitmap(Bitmap bitmap, int borderSize)
            {
                try
                {
                    Bitmap bmpWithBorder = Bitmap.CreateBitmap(bitmap.Width + borderSize * 2, bitmap.Height + borderSize * 2, bitmap.GetConfig());
                    Canvas canvas = new Canvas(bmpWithBorder);
                    canvas.DrawColor(Color.Red);
                    canvas.DrawBitmap(bitmap, borderSize, borderSize, null);
                    return bmpWithBorder;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return bitmap;
                }
            }

            public void OnLoadingStarted(string imageUri, View view)
            {
                try
                {
                  
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        } 
    }
}