using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Birdtracks.Game.ONS.GreenDress
{
    [CreateAssetMenu(menuName = "StoryData", fileName = "Green Dress Data")]
    public class StoryQuestionData : ScriptableObject
    {
        public string questionText;
        public AudioClip ENGQuestionAudioClip;
        public AudioClip SSWQuestionAudioClip;

        public AudioClip ENGPromptQuestionAudioClip;
        public AudioClip SSWPromptQuestionAudioClip;

        public AudioClip ENGAcknowledgeAnswerAudioClipP1;
        public AudioClip ENGAcknowledgeAnswerAudioClipP2;
            
        public AudioClip SSWAcknowledgeAnswerAudioClipP1;
        public AudioClip SSWAcknowledgeAnswerAudioClipP2;
        
        

        public List<QuestionData> questions = new List<QuestionData>();
        
    }

    [Serializable]
    public class QuestionData
    {
        public Sprite Image;
        public bool IsCorrectAnswer;
    }
}
