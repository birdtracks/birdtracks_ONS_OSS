using System;

namespace BirdTracks.Game.Core
{
    [Serializable]
    public struct Session
    {
        public string SessionId;
    }

    [Serializable]
    public class FlutterInitArgs
    {
        public Session Session;
    }
}
