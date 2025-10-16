using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ONS_LipSyncManager : MonoBehaviour
{
    private float _delayAnimationStartTime;
    private float _audioClipLength;
    private Animator _animator;
    
    private Coroutine _waitForAudioEndCoroutine;
    private Coroutine _waitForAudioBeginCoroutine;

    void OnEnable()
    {
        Debug.Log($"start lip sync manager on {gameObject.name}");
        _animator = GetComponent<Animator>();
        _animator.SetBool("EndTalkLoop", true);
    }
    
    public void Set(float delayTime, Animator animator, float clipLength)
    {
        _delayAnimationStartTime = delayTime;
        _animator = animator;
        _audioClipLength = clipLength;
    }
    
    public void BeginSequence()
    {
        if (_waitForAudioEndCoroutine != null)
        {
            StopCoroutine(_waitForAudioEndCoroutine);
        }
        
        if (_waitForAudioBeginCoroutine != null)
        {
            StopCoroutine(_waitForAudioBeginCoroutine);
        }
        _waitForAudioBeginCoroutine = StartCoroutine(WaitForAudioBegin());
        
        IEnumerator WaitForAudioBegin()
        {
            yield return new WaitForSeconds(_delayAnimationStartTime);
        
            _animator.SetBool("EndTalkLoop", false);
            _waitForAudioEndCoroutine = StartCoroutine(WaitForAudioEnd());
        }
    
        IEnumerator WaitForAudioEnd()
        {
            yield return new WaitForSeconds(_audioClipLength);
        
            _animator.SetBool("EndTalkLoop", true);
        }
    }
    
}
