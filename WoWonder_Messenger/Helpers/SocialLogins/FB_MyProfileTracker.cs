using System;
using Xamarin.Facebook;
using Exception = System.Exception;

namespace WoWonder.Helpers.SocialLogins {
    public class FB_MyProfileTracker : ProfileTracker {
        public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

        protected override void OnCurrentProfileChanged (Profile oldProfile, Profile currentProfile) {
            try {
                mOnProfileChanged?.Invoke (this, new OnProfileChangedEventArgs (currentProfile));
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }
    }

    public class OnProfileChangedEventArgs : EventArgs {
        public Profile mProfile;
        public OnProfileChangedEventArgs (Profile profile) {
            try {
                mProfile = profile;
            } catch (Exception e) {
                Console.WriteLine (e);
            }
        }
        //Extract or delete HTML tags based on their name or whether or not they contain some attributes or content with the HTML editor pro online program.
    }
}