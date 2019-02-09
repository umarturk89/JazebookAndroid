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
    public class AccessTokenObject {
        [JsonProperty ("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty ("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty ("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty ("scope")]
        public string Scope { get; set; }

        [JsonProperty ("token_type")]
        public string TokenType { get; set; }

        [JsonProperty ("id_token")]
        public string IdToken { get; set; }
    }
}