using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using Com.OneSignal.Abstractions;
using Com.OneSignal.Android;
using Org.Json;
using WoWonder.Activities.ChatWindow;
using WoWonder.Functions;
using OneSignal = Com.OneSignal.OneSignal;
using OSNotification = Com.OneSignal.Abstractions.OSNotification;
using OSNotificationPayload = Com.OneSignal.Abstractions.OSNotificationPayload;


namespace WoWonder.Frameworks.onesignal {
    public class OneSignalNotification {
        //Force your app to Register notifcation derictly without loading it from server (For Best Result)

        public static string OneSignalAPP_ID = "d08ab374-deed-4e2a-bdb5-5202d877c4bc";
        public static string userid;

        public static void RegisterNotificationDevice () {
            try {
                if (UserDetails.NotificationPopup) {
                    if (OneSignalAPP_ID != "") {
                        OneSignal.Current.StartInit (OneSignalAPP_ID)
                            .InFocusDisplaying (OSInFocusDisplayOption.Notification)
                            .HandleNotificationReceived (HandleNotificationReceived)
                            .HandleNotificationOpened (HandleNotificationOpened)
                            .EndInit ();
                        OneSignal.Current.IdsAvailable (IdsAvailable);
                        OneSignal.Current.RegisterForPushNotifications ();

                        AppSettings.ShowNotification = true;
                    }
                }
            } catch (Exception ex) {
                var exception = ex.ToString ();
            }
        }

        public static void Un_RegisterNotificationDevice () {
            try {
                OneSignal.Current.SetSubscription (false);
               AppSettings.ShowNotification = false;
            } catch (Exception ex) {
                var exception = ex.ToString ();
            }
        }

        private static void IdsAvailable (string userID, string pushToken) {
            try {
                UserDetails.Device_ID = userID;
            } catch (Exception ex) {
                var exception = ex.ToString ();
            }
        }

        private static void HandleNotificationReceived (OSNotification notification) {
            try {

                OSNotificationPayload payload = notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;

                string message = payload.body;

            } catch (Exception ex) {
             
                var exception = ex.ToString ();
            }
        }

        private static void HandleNotificationOpened (OSNotificationOpenedResult result) {
            try {
                OSNotificationPayload payload = result.notification.payload;
                Dictionary<string, object> additionalData = payload.additionalData;
                string message = payload.body;
                string actionID = result.action.actionID;

                if (additionalData != null) {
                    foreach (var item in additionalData) {
                        if (item.Key == "user_id") {
                            userid = item.Value.ToString ();
                        }
                    }

                    //Intent intent = new Intent(Application.Context.PackageName + ".FOO");
                    Intent intent = new Intent (Application.Context, typeof (ChatWindow_Activity));
                    intent.PutExtra ("UserID", userid);
                    intent.PutExtra ("Notfi", "Notfi");
                    intent.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    intent.AddFlags (ActivityFlags.SingleTop);
                    intent.SetAction (Intent.ActionView);
                    Application.Context.StartActivity (intent);

                    if (additionalData.ContainsKey ("discount")) {
                        // Take user to your store..

                    }
                }
                if (actionID != null) {
                    // actionSelected equals the id on the button the user pressed.
                    // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present. 

                }
            } catch (Exception ex) {
                var exception = ex.ToString ();
            }
        }
    }

    public class NotificationExtenderServiceHandeler : NotificationExtenderService, NotificationCompat.IExtender {
        protected override void OnHandleIntent (Intent intent) {

        }

        protected override bool OnNotificationProcessing (OSNotificationReceivedResult p0) {
            OverrideSettings overrideSettings = new OverrideSettings ();
            overrideSettings.Extender = new NotificationCompat.CarExtender ();

            Com.OneSignal.Android.OSNotificationPayload payload = p0.Payload;
            JSONObject additionalData = payload.AdditionalData;

            if (additionalData.Has ("room_name")) {
                string room_name = additionalData.Get ("room_name").ToString ();
                string Call_type = additionalData.Get ("call_type").ToString ();
                string Call_id = additionalData.Get ("call_id").ToString ();
                string From_id = additionalData.Get ("from_id").ToString ();
                string to_id = additionalData.Get ("to_id").ToString ();

                return false;
            } else {
                return true;
            }
        }

        public NotificationCompat.Builder Extend (NotificationCompat.Builder builder) {
            return builder;
        }
    }
}