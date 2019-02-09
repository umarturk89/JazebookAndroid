using DT.Xamarin.Agora;

namespace WoWonder.Frameworks.Agora {
    /// <summary>
    // This Class provides an overview of Agora.io real-time communications Connection with #WoWonder script
    //Each Agora account can create multiple projects, and each project has a unique App ID.
    //Sign up for a new account at https://dashboard.agora.io/.
    //Click Add New Project on the Projects page of the dashboard.
    //Fill in the Project Name and click Submit.
    /// </summary>

    public class AgoraSettings {
        public const string AgoraAPI = "17d3c989123e4977be8cc5a1ea3102b8";

        public static readonly int VideoQuality = Constants.VideoProfile1080p5;
        public static readonly int ProfileDefault = 30;
        public static readonly string EncryptionPhraseDefault = "";
        public static readonly int EncryptionTypeDefault = (int) EnumExtensions.EncryptionType.xts128;
    }
}