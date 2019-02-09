using DT.Xamarin.Agora;

namespace WoWonder.Frameworks.Agora {
    public class AgoraRtcHandler : IRtcEngineEventHandler {
        private AgoraVideoCallActivity _context;

        public AgoraRtcHandler (AgoraVideoCallActivity activity) {
            _context = activity;
        }

        public override void OnFirstRemoteVideoDecoded (int p0, int p1, int p2, int p3) {
            _context.OnFirstRemoteVideoDecoded (p0, p1, p2, p3);
        }

        public override void OnConnectionLost () {
            _context.OnConnectionLost ();
        }

        public override void OnUserOffline (int p0, int p1) {
            _context.OnUserOffline (p0, p1);
        }

        public override void OnUserMuteVideo (int p0, bool p1) {
            _context.OnUserMuteVideo (p0, p1);
        }

        public override void OnFirstLocalVideoFrame (int p0, int p1, int p2) {
            _context.OnFirstLocalVideoFrame (p0, p1, p2);
        }

        public override void OnNetworkQuality (int p0, int p1, int p2) {
            _context.OnNetworkQuality (p0, p1, p2);
        }

        public override void OnUserJoined (int p0, int p1) {
            _context.OnUserJoined (p0, p1);
        }
    }

    public static class EnumExtensions {
        public static string GetModeString (EncryptionType value) {
            switch (value) {
                case EncryptionType.xts128:
                    return "aes-128-xts";
                case EncryptionType.xts256:
                    return "aes-256-xts";
            }
            return string.Empty;
        }

        public static string GetDescriptionString (EncryptionType value) {
            switch (value) {
                case EncryptionType.xts128:
                    return "AES 128";
                case EncryptionType.xts256:
                    return "AES 256";
            }
            return string.Empty;
        }

        public enum EncryptionType {
            xts128 = 0, // = "aes-128-xts",
            xts256 = 1 // = "aes-256-xts"
        }
    }
}