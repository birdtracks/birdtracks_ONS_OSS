using System;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public struct PlaySessionItem
    {
        public string GameName;
        public string Description;
        public Func<Coroutine> LoadCallback;
    }
}