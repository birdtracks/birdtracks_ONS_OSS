using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class ONS_JJAudio : StateMachineBehaviour
{
    [SerializeField] private float _delayTime;
    [SerializeField] private AnimationClip _animationClip;
    [SerializeField] private AudioClip _audioClip;

    private ONS_JJAudioManager _jjAudioManager;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        Debug.Log("anim clip name = " + _animationClip.name);
        
        if (!stateInfo.IsName(_animationClip.name)) return;

        if (_jjAudioManager == null)
        {
            _jjAudioManager = animator.GetComponent<ONS_JJAudioManager>();
        }
        
        _jjAudioManager.Set(_delayTime, _audioClip, animator);
        _jjAudioManager.BeginSequence();
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    
    
}
