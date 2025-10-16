using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ONS_CharacterLipSync : MonoBehaviour, INotificationReceiver
{
    [SerializeField] private float _delayTime;
    [SerializeField] private Animator _animator;
    [SerializeField] private ONS_LipSyncManager _audioManager;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        
    }

    public void PlayLipSync(float clipLength)
    {
        if (_audioManager == null)
        {
            _audioManager = _animator.GetComponent<ONS_LipSyncManager>();
        }
        
        _audioManager.Set(_delayTime, _animator, clipLength);
        _audioManager.BeginSequence();
    }
}
