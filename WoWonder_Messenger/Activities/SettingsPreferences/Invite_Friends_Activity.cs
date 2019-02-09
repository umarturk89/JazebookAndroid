using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using WoWonder.Adapters;
using WoWonder.Functions;

using Exception = Java.Lang.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.SettingsPreferences
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation)]
    public class Invite_Friends_Activity : AppCompatActivity
    {

        #region Variables Basic

        private RecyclerView InviteFriendsRecyler;
        private RecyclerView.LayoutManager mLayoutManager;
        public static InviteFriends_Adapter mAdapter;

        public IMethods.PhoneContactManager.UserContact Contact = new IMethods.PhoneContactManager.UserContact();

        public string InviteSMSText = "";

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                IMethods.IApp.FullScreenApp(this);

                // Set our view from the "BlockedUsers_Layout" layout resource
                SetContentView(Resource.Layout.Invite_Friends_Layout);

                //Get values
                InviteFriendsRecyler = FindViewById<RecyclerView>(Resource.Id.InviteFriends_Recylerview);

                //Set ToolBar
                var ToolBar = FindViewById<Toolbar>(Resource.Id.Searchtoolbar);
                ToolBar.Title = this.GetText(Resource.String.Lbl_Invite_Friends);

                SetSupportActionBar(ToolBar);

                SupportActionBar.SetDisplayShowCustomEnabled(true);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                SupportActionBar.SetDisplayShowHomeEnabled(true);

                //Set Adapter
                mLayoutManager = new LinearLayoutManager(this);
                InviteFriendsRecyler.SetLayoutManager(mLayoutManager);
                mAdapter = new InviteFriends_Adapter(this);
                mAdapter.mUsersPhoneContacts = new ObservableCollection<IMethods.PhoneContactManager.UserContact>();
                InviteFriendsRecyler.SetAdapter(mAdapter);


                GetAllContacts();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                 
                //Event
                mAdapter.ItemClick += MAdapterOnItemClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                //Event
                mAdapter.ItemClick -= MAdapterOnItemClick;

                base.OnPause();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void GetAllContacts()
        {
            try
            {
                var listContacts =new ObservableCollection<IMethods.PhoneContactManager.UserContact>(IMethods.PhoneContactManager.GetAllContacts());
                var OrderBydate = listContacts.OrderBy(a => a.UserDisplayName);

                mAdapter.mUsersPhoneContacts = new ObservableCollection<IMethods.PhoneContactManager.UserContact>(OrderBydate);
                mAdapter.NotifyDataSetChanged();

                InviteSMSText = GetText(Resource.String.Lbl_InviteSMSText_1) + " " + AppSettings.Application_Name + " " +
                                GetText(Resource.String.Lbl_InviteSMSText_2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MAdapterOnItemClick(object sender, InviteFriends_AdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var Position = adapterClickEvents.Position;
                if (Position >= 0)
                {
                    var item = mAdapter.GetItem(Position);

                    Contact = item;
                    if (item != null)
                    {
                        if ((int) Build.VERSION.SdkInt < 23)
                        {
                            IMethods.IApp.SendSMS(this, item.PhoneNumber, InviteSMSText);
                            return;
                        }
                        else
                        {
                            //Check to see if any permission in our group is available, if one, then all are
                            if (CheckSelfPermission(Manifest.Permission.SendSms) == Permission.Granted)
                            {
                                IMethods.IApp.SendSMS(this, item.PhoneNumber, InviteSMSText);
                                return;
                            }
                            else
                            {
                                GetLocationPermissionAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void GetLocationPermissionAsync()
        {
            RequestPermissions(new String[]
            {

                Manifest.Permission.SendSms,
                Manifest.Permission.BroadcastSms,
            }, 105);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 105)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    IMethods.IApp.SendSMS(this, Contact.PhoneNumber, InviteSMSText);
                }
                else
                {
                    Toast.MakeText(this, this.GetText(Resource.String.Lbl_Permission_is_denailed), ToastLength.Long)
                        .Show();
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }


        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                ImageService.Instance.InvalidateMemoryCache();
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}