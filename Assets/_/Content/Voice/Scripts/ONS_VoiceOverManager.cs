using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BirdTracks.Game.Core;
using Birdtracks.Game.ONS;
using UnityEngine;
using UnityEngine.Playables;

public class ONS_VoiceOverManager : MonoBehaviour, INotificationReceiver
{
    [SerializeField] private BoolGameVariable _isSeswati; 
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private VoiceOverAudioLibrary _voiceOverAudioLibrary;
    [SerializeField] private ONS_CharacterLipSync _lipSync;

    private List<AudioClip> _activeLanguageClips;
    
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        
    }

    private void OnEnable()
    {
        SetActiveLanguage();
    }

    private void SetActiveLanguage()
    {
        Language language = _isSeswati.Value ? Language.Seswati : Language.English;
        
        _voiceOverAudioLibrary.SetActiveLanguage(language);
        
        _voiceOverAudioLibrary.InitializeLookup();
    }

    public void PlayAudioClip(string id)
    {
        AudioClip clip = FindClip(id);

        if (clip == null)
        {
            Debug.LogError("no clip found");
            return;
        }

        _audioSource.clip = clip;
        _lipSync?.PlayLipSync(_audioSource.clip.length);
        _audioSource.Play();
    }

    private AudioClip FindClip(string id)
    {
        return _voiceOverAudioLibrary.GetClip(id);
    }
}
