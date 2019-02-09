using System.Collections.Generic;
using Android.Support.V4.App;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace WoWonder.Adapters {
    public class StickersTab_Adapter : FragmentPagerAdapter {
        public List<Fragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }

        public StickersTab_Adapter (FragmentManager sfm) : base (sfm) {
            Fragments = new List<Fragment> ();
            FragmentNames = new List<string> ();
        }

        public void AddFragment (Fragment fragment, string name) {
            Fragments.Add (fragment);
            FragmentNames.Add (name);
        }

        public override int Count {
            get {
                return Fragments.Count;
            }
        }

        public override Fragment GetItem (int position) {
            return Fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted (int position) {
            return new Java.Lang.String (FragmentNames[position]);
        }
    }
}