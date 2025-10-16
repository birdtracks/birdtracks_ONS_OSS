using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryData_Cats", fileName = "Cats Question Data")]
public class CatsQuestionData : ScriptableObject
{
    public List<QuestionData> CatsQuestionList = new List<QuestionData>();
}

[Serializable]
public class QuestionData
{
    public AudioClip ENGQuestionAudioClip;
    public AudioClip SSWQuestionAudioClip;

    public AudioClip ENGPromptQuestionAudioClip;
    public AudioClip SSWPromptQuestionAudioClip;

    public string MainQuestionSubtitle;
    public string NudgeQuestionSubtitle;
    public string AcknowledgeSubtitle;
    public int maxListeningTime;
    public bool Completed;
}
