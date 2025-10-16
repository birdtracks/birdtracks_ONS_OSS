using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Birdtracks.Game.ONS
{
    [CreateAssetMenu(menuName = "Game/Tasks/Voice/Puzzle Options")]
    public class VoiceAssessmentOptions : ScriptableObject
    {
        public string VADAssessmentName;
        public string SceneName;
        public AudioClip PayoffClip;
        public int Difficulty;
    }
}
