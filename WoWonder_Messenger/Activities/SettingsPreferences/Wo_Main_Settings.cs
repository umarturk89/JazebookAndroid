using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Preferences;
using Java.Util;
using WoWonder.Functions;
using WoWonder.SQLite;


namespace WoWonder.Activities.SettingsPreferences
{
    public class Wo_Main_Settings
    {
        public static ISharedPreferences Shared_Data;
        public static string App_Language;
        public static string Local_Language;

        public static async void Init()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var ss = await dbDatabase.CheckTablesStatus();

                dbDatabase.OpenConnection();

                Shared_Data = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                App_Language = Shared_Data.GetString("Lang_key", "");

                if (string.IsNullOrEmpty(App_Language))
                {
                    SetDefaultSettings();
                    if (App_Language.Contains("ar"))
                    {
                        AppSettings.Lang = "ar";
                        AppSettings.FlowDirection_RightToLeft = true;
                    }
                    else
                    {
                        AppSettings.Lang = "";
                        AppSettings.FlowDirection_RightToLeft = false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SetDefaultSettings()
        {
            try
            {
                var data = Classes.MyProfileList.FirstOrDefault(a => a.user_id == UserDetails.User_id);
                if (data != null)
                {
                    Shared_Data.Edit().PutString("whocanfollow_key", data.follow_privacy).Commit();
                    Shared_Data.Edit().PutString("whocanMessage_key", data.message_privacy).Commit();
                    Shared_Data.Edit().PutString("whocanseemybirthday_key", data.birth_privacy).Commit();
                }

               

                var Lang = Shared_Data.GetString("Lang_key", "Auto");
                if (Lang == "ar")
                {
                    Shared_Data.Edit().PutString("Lang_key", "ar").Commit();
                    AppSettings.Lang = "ar";
                    AppSettings.FlowDirection_RightToLeft = true;
                }
                else
                {
                    Shared_Data.Edit().PutString("Lang_key", "Auto").Commit();
                    AppSettings.Lang = "";
                    AppSettings.FlowDirection_RightToLeft = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void SetApplicationLang(string lang)
        {
            try
            {
                Configuration config = new Configuration();
                if (string.IsNullOrEmpty(lang))
                {
                    if (App_Language == "Auto" || App_Language == "")
                    {
                        config.Locale = Locale.Default;
                        Local_Language = config.Locale.Language;
                    }
                    else
                    {
                        config.Locale = Locale.Default = new Locale(App_Language);
                    }

                    if (config.Locale.Language.Contains("ar"))
                    {
                        AppSettings.Lang = "ar";
                        AppSettings.FlowDirection_RightToLeft = true;
                    }
                    else
                    {
                        AppSettings.Lang = "";
                        AppSettings.FlowDirection_RightToLeft = false;
                    }
                }
                else
                {
                    config.Locale = Locale.Default = new Locale(lang);

                    if (lang.Contains("ar"))
                    {
                        AppSettings.Lang = "ar";
                        AppSettings.FlowDirection_RightToLeft = true;
                    }
                    else if (lang.Contains("Auto") || lang.Contains(""))
                    {
                        config.Locale = Locale.Default = new Locale(Local_Language);
                        if (Local_Language.Contains("ar"))
                        {
                            AppSettings.Lang = "ar";
                            AppSettings.FlowDirection_RightToLeft = true;
                        }
                        else
                        {
                            AppSettings.Lang = "";
                            AppSettings.FlowDirection_RightToLeft = false;
                        }
                    }
                    else
                    {
                        AppSettings.Lang = "";
                        AppSettings.FlowDirection_RightToLeft = false;
                    }
                }

                Application.Context.Resources.UpdateConfiguration(config, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}