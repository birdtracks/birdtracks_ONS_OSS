
using System;

namespace BirdTracks.Game.Core
{
    [Serializable]
    public sealed class TangramResult : IResult
    {
        public string Id;
        public string UserId;
        public string SceneName;
        public int NumPuzzlePieces;
        public int NumCorrectPieces;
        public float TimeElapsed;
        public float ActiveTime;
        public float InactiveTime;

        string IResult.Id
        {
            get
            {
                return this.Id;
            }
        }
    }
}