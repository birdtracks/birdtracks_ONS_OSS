using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Birdtracks.Game.ONS.MuddyPaws
{
    [CreateAssetMenu(menuName = "StoryData_MP", fileName = "Muddy Paws Data")]
    public class MuddyPawsQuestionData : ScriptableObject
    {
        public List<QuestionData> muddyPawsQuestionList = new List<QuestionData>();
    }

    [Serializable]
    public class QuestionData
    {
        public AudioClip ENGQuestionAudioClip;
        public AudioClip ENGPromptQuestionAudioClip;
        public AudioClip SSWQuestionAudioClip;
        public AudioClip SSWPromptQuestionAudioClip;
        public Sprite[] StorySpriteSheet;
        public string MainQuestionSubtitle;
        public string NudgeQuestionSubtitle;
        public int MaxVADRecordLength = 1;
        public bool Completed;
    }
}
