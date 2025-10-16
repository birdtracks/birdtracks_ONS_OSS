using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ONS_JJAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    
    private float _delayAnimationStartTime;
    private float _audioClipLength;
    private AudioClip _audioClip;
    private Animator _animator;

    private Coroutine _waitForAudioEndCoroutine;
    private Coroutine _waitForAudioBeginCoroutine;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool("EndTalkLoop", true);
    }
    
    public void Set(float delayTime, AudioClip clip, Animator animator)
    {
        _delayAnimationStartTime = delayTime;
        _audioClipLength = clip.length;
        _audioClip = clip;
        _animator = animator;
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
        
        if (_audioSource == null)
        {
            _audioSource = _animator.GetComponent<AudioSource>();
        }
        
        if (_audioSource != null)
        {
            
            _audioSource.clip = _audioClip;
            _waitForAudioBeginCoroutine = StartCoroutine(WaitForAudioBegin());
        }
        else
        {
            Debug.LogWarning("No AudioSource found on the Animator's GameObject.");
        }
    }
    
    IEnumerator WaitForAudioBegin()
    {
        yield return new WaitForSeconds(_delayAnimationStartTime);
        
        _audioSource.Play();
        _animator.SetBool("EndTalkLoop", false);
        _waitForAudioEndCoroutine = StartCoroutine(WaitForAudioEnd());
    }
    
    IEnumerator WaitForAudioEnd()
    {
        yield return new WaitForSeconds(_audioClipLength);
        
        _audioSource.Stop();
        _animator.SetBool("EndTalkLoop", true);
    }
}
