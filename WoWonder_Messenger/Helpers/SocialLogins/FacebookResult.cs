using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace WoWonder.Helpers.SocialLogins {
    public class FacebookResult {
        [JsonProperty ("id")]
        public string id { get; set; }

        [JsonProperty ("name")]
        public string name { get; set; }

        [JsonProperty ("email")]
        public string email { get; set; }
    }
}