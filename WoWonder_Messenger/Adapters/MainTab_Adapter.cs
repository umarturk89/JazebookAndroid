using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using Android.Support.V4.App;
using Java.Lang;

namespace WoWonder.Adapters {
    public class MainTab_Adapter : FragmentPagerAdapter {
        public List<SupportFragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }

        public MainTab_Adapter (SupportFragmentManager sfm) : base (sfm) {
            Fragments = new List<SupportFragment> ();
            FragmentNames = new List<string> ();
        }

        public void AddFragment (SupportFragment fragment, string name) {
            Fragments.Add (fragment);
            FragmentNames.Add (name);
        }

        public override int Count {
            get {
                return Fragments.Count;
            }
        }

        public override SupportFragment GetItem (int position) {
            return Fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted (int position) {
            return new Java.Lang.String (FragmentNames[position]);
        }
    }
}