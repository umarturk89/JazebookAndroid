using DT.Xamarin.Agora;

namespace WoWonder.Frameworks.Agora {
    public class AgoraRtcAudioCallHandler : IRtcEngineEventHandler {
        private AgoraAudioCallActivity _context;

        public AgoraRtcAudioCallHandler (AgoraAudioCallActivity activity) {
            _context = activity;
        }

        public override void OnConnectionLost () {
            _context.OnConnectionLost ();
        }

        public override void OnUserOffline (int p0, int p1) {
            _context.OnUserOffline (p0, p1);
        }

        public override void OnNetworkQuality (int p0, int p1, int p2) {
            _context.OnNetworkQuality (p0, p1, p2);
        }

        public override void OnUserJoined (int p0, int p1) {
            _context.OnUserJoined (p0, p1);
        }
    }
}