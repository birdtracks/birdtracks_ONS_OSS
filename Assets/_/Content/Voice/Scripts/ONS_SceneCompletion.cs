using System.Collections;
using System.Collections.Generic;
using BirdTracks.Game.Core;
using UnityEngine;

public class ONS_SceneCompletion : MonoBehaviour
{
    [SerializeField] private VADLevelSequenceHandler m_VADLevelSequenceHandler;
    [SerializeField] private GameEvent m_StoryCompleted = default;
    [SerializeField] private LoadingScreen m_LoadingScreen = default;
    
    //include some placeholders for the data that can get collated here


    private void OnEnable()
    {
        m_StoryCompleted.Subscribe(StoryCompletedHandler);
    }

    private void StoryCompletedHandler(GameEvent arg0)
    {
        m_VADLevelSequenceHandler.OnClicked_Continue();
        Debug.Log("story complete");
    }
}
