using Android.Gms.Common.Apis;
using Java.Lang;
using WoWonder.Activities;

namespace WoWonder.Helpers.SocialLogins {
    public class SignOutResultCallback : Object, IResultCallback {
        public MainActivity Activity { get; set; }

        public void OnResult (Object result) {
            //Activity.UpdateUI(false);
        }
    }
}