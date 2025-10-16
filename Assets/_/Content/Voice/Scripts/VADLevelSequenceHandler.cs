using System.Collections;
using System.Collections.Generic;
using BirdTracks.Game.Core;
using UnityEngine;

public class VADLevelSequenceHandler : MonoBehaviour
{
    [SerializeField] private LoadSceneButton m_LoadSceneButton;
    private VADGameSettings _settings;
    private int _index;
    
    public void Initialize(VADGameSettings settings, int index)
    {
        _settings = settings;
        _index = index;
    }
    
    public void OnClicked_Continue()
    {
        StartCoroutine(ContinueRoutine());
    }

    private IEnumerator ContinueRoutine()
    {
        _index++;

        if(_index == _settings.StoryData.Count)
        {
            Debug.Log("Next Game");
            PlaySessionService.Instance.LoadNextGame();
        }
        else
        {
            m_LoadSceneButton.m_SceneName = _settings.StoryData[_index].SceneName;

            yield return m_LoadSceneButton.Load(() =>
            {
                FindObjectOfType<VADLevelSequenceHandler>().Initialize(_settings, _index);
            });
        }
    }
    
}
