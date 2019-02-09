using System;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using WoWonder.Activities;
using Exception = Java.Lang.Exception;
using Object = Java.Lang.Object;
namespace WoWonder.Helpers.SocialLogins {
    public class SignInResultCallback : Object, IResultCallback {
        public MainActivity Activity { get; set; }

        public void OnResult (Object result) {
            try {
                var googleSignInResult = result as GoogleSignInResult;
                Activity.HandleSignInResult (googleSignInResult);
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }
    }
}