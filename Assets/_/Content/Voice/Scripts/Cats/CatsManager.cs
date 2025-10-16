using System.Collections;
using BirdTracks.Game.Core;
using Birdtracks.Game.ONS;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CatsManager : MonoBehaviour, INotificationReceiver
{
    [Header("Scene Dependencies")]
    [SerializeField] private PlayableDirector m_PlayableDirector;
    [SerializeField] private LoadingScreen m_LoadingScreen = default;
    [Tooltip("Assign the GameObject containing the IVoiceDetector implementation")]
    [SerializeField] private GameObject voiceDetectorProvider;
    [SerializeField] private Animator TortoiseAnimator;
    [SerializeField] private CanvasGroup ThoughtCanvasGroup;
    [SerializeField] private ONS_CharacterLipSync _lipSync;
    [SerializeField] private AudioSource _audioSource;

    [Header("Game Logic")]
    [SerializeField] private GameEvent gameEvent;
    [SerializeField] private CatsQuestionData QuestionData;
    [SerializeField] private float additionalDelayTime = 0;
    [SerializeField] private BoolGameVariable _isSeswati;
        
    private IVoiceDetector SVD;
    private SpriteSheetAnimator _spriteSheetAnimator;
    private QuestionData _currentQuestionData;

    private Coroutine _delayListenCoroutine;
    private Coroutine _delayTimerResetCoroutine;
    private Coroutine _delayNextQuestionCoroutine;

    private int _promptCount = 0;
    
    private Language _language;
    
    private void OnEnable()
    {
        m_LoadingScreen.FadeIn();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        Setup();
        
        _language = _isSeswati.Value ? Language.Seswati : Language.English;
        
        m_PlayableDirector.Play();
    }

    private void Setup()
    {
        _audioSource = GetComponent<AudioSource>();

        // Get the IVoiceDetector implementation
        if (voiceDetectorProvider != null)
        {
            SVD = voiceDetectorProvider.GetComponent<IVoiceDetector>();
            if (SVD == null)
            {
                Debug.LogError(
                    $"The assigned voiceDetectorProvider '{voiceDetectorProvider.name}' does not have a component implementing IVoiceDetector!",
                    voiceDetectorProvider);
            }
        }
        else
        {
            // Fallback: Try to get it from the same GameObject
            SVD = GetComponent<IVoiceDetector>();
            if (SVD == null)
            {
                Debug.LogError(
                    "No IVoiceDetector component found on this GameObject, and no voiceDetectorProvider assigned! Voice detection will not work.",
                    this.gameObject);
            }
        }

        if (SVD != null)
        {
            SVD.OnVoiceRecorded += HandleVoiceRecorded; // Changed event name and handler signature
            SVD.OnRecordingTimeout += HandleRecordingTimeout; // Added timeout handler
            SVD.OnFirstNudge += HandleFirstNudge;
            SVD.OnSecondNudge += HandleSecondNudge;
        }

        foreach (var q in QuestionData.CatsQuestionList)
        {
            q.Completed = false;
        }

    }

    private bool StartQuestions()
    {
        foreach (var q in QuestionData.CatsQuestionList)
        {
            if (!q.Completed)
            {
                _promptCount = 0;
                _currentQuestionData = q;
                ShowCanvas();
                SetQuestionAudio();
                return true;
            }
        }

        return false;
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        StartQuestions();
    }

    private void HandleFirstNudge()
    {
        if (TortoiseAnimator != null)
        {
            Debug.Log("Handling First Nudge (Triggering Animation)");
            TortoiseAnimator.SetTrigger("Prompt");
        }
    }

    private void HandleSecondNudge()
    {
        if (TortoiseAnimator != null)
        {
            Debug.Log("Handling Second Nudge (Triggering Animation & Audio)");
            TortoiseAnimator.SetTrigger("Prompt");
            // Play the nudge audio prompt *after* the animation trigger potentially
            SetQuestionNudgeAudio();
        }
    }

    private void SetQuestionAudio()
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("Cannot play question audio: AudioSource or AudioClip missing.");
            // If audio fails, maybe start listening immediately? Or after a short default delay?
            StartListeningAfterDelay(1.0f);
            return;
        }
        AudioClip currentClip = GetQuestionClip();
        _audioSource.clip = currentClip;
        _audioSource.Play();
        _lipSync?.PlayLipSync(currentClip.length);

        StartListeningAfterDelay(currentClip.length + additionalDelayTime);
    }
    
    private AudioClip GetQuestionClip()
    {
        return _language == Language.English
            ? _currentQuestionData.ENGQuestionAudioClip
            : _currentQuestionData.SSWQuestionAudioClip;
    }

    private void SetQuestionNudgeAudio()
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("Cannot play nudge audio: AudioSource or PromptClip missing.");
            // If nudge audio fails, maybe reset timer immediately?
            ResetVADNudgeTimerAfterDelay(0.1f);
            return;
        }

        // Pause VAD to prevent nudge audio from being recorded
        if (SVD != null && SVD.IsListening)
        {
            SVD.PauseListening();
            Debug.Log("VAD paused during nudge audio playback");
        }

        AudioClip promptClip = GetPromptClip();
        _audioSource.clip = promptClip;
        _audioSource.Play();
        _lipSync?.PlayLipSync(promptClip.length);

        // Reset the VAD's nudge timer *after* the nudge audio finishes playing
        ResetVADNudgeTimerAfterDelay(promptClip.length + additionalDelayTime);
    }
    
    private AudioClip GetPromptClip()
    {
        return _language == Language.English
            ? _currentQuestionData.ENGPromptQuestionAudioClip
            : _currentQuestionData.SSWPromptQuestionAudioClip;
    }

    // Handler for successful voice recording
    private void HandleVoiceRecorded(AudioClip recordedClip)
    {
        Debug.Log($"Handling Voice Recorded. Clip Length: {recordedClip?.length ?? 0}s");
        
        SubmitClip(recordedClip);
        
        MarkQuestionCompleteAndProceed();
    }
    
    private void SubmitClip(AudioClip finalClip)
    {
        if (finalClip == null)
        {
            Debug.LogWarning("Cannot submit a null audio clip");
            return;
        }
        string sceneName = SceneManager.GetActiveScene().name;
        
        //save to device
        SaveAudioUtil.SaveToWAV(finalClip, $"{sceneName}", out string filePath);

        VoiceGameData gameData = new VoiceGameData()
        {
            filePath = filePath,
            gameName = sceneName
        };
        
        //save to cloud (via flutter)
        SaveAudioUtil.AddGameData(gameData);
    }

    private void SubmitNoClip()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        VoiceGameData gameData = new VoiceGameData()
        {
            question = _currentQuestionData.MainQuestionSubtitle,
            filePath = "No speed detected",
            gameName = sceneName
        };
        SaveAudioUtil.AddGameData(gameData);
    }

    // Handler for when VAD stops due to timeout (no speech)
    private void HandleRecordingTimeout()
    {
        Debug.Log("Handling Recording Timeout (No speech detected). Moving on.");
        SubmitNoClip();
        MarkQuestionCompleteAndProceed();
    }

    // Common logic after an answer OR timeout
    private void MarkQuestionCompleteAndProceed()
    {
        if (_currentQuestionData != null)
        {
            _currentQuestionData.Completed = true;
        }

        if (TortoiseAnimator != null)
        {
            TortoiseAnimator.SetTrigger("Answer");
        }

        HideRecorderSymbolAndPause();
    }
    
    private void HideRecorderSymbolAndPause()
    {
        if (ThoughtCanvasGroup != null)
        {
            ThoughtCanvasGroup.alpha = 0; // Hide instantly or fade? Let CrossFade handle visual
        }

        // Stop previous delay coroutine if it exists
        if (_delayNextQuestionCoroutine != null)
        {
            StopCoroutine(_delayNextQuestionCoroutine);
        }
        // Start delay before checking for the next question
        _delayNextQuestionCoroutine = StartCoroutine(PauseBeforeNextQuestion(2.0f)); // Pause duration
    }

    private void StartListeningAfterDelay(float delay)
    {
        // Stop previous delay coroutine if it exists
        if (_delayListenCoroutine != null)
        {
            StopCoroutine(_delayListenCoroutine);
        }

        _delayListenCoroutine = StartCoroutine(DelayAndStartListening(delay));
    }

    private void ResetVADNudgeTimerAfterDelay(float delay)
    {
        // Stop previous delay coroutine if it exists
        if (_delayTimerResetCoroutine != null)
        {
            StopCoroutine(_delayTimerResetCoroutine);
        }

        _delayTimerResetCoroutine = StartCoroutine(DelayAndResetNudgeTimer(delay));
    }
    
    private bool ShowNextQuestionOrEnd()
    {
        if (QuestionData == null || QuestionData.CatsQuestionList == null) return false;

        // Find the next incomplete question
        _currentQuestionData = null;
        foreach (var q in QuestionData.CatsQuestionList)
        {
            if (q != null && !q.Completed)
            {
                _currentQuestionData = q;
                break;
            }
        }

        if (_currentQuestionData != null)
        {
            _promptCount = 0;
            ShowCanvas();
            SetQuestionAudio();
            return true;
        }
        
        Debug.Log("All questions completed.");

        SubmitComplete();
        
        gameEvent?.Invoke();
        return false;
        
    }

    private void SubmitComplete()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        
        SaveAudioUtil.SaveAllData(sceneName);
    }

    private void ShowCanvas()
    {
        if (ThoughtCanvasGroup != null)
        {
            ThoughtCanvasGroup.alpha = 1;
        }
    }

    IEnumerator PauseBeforeNextQuestion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _delayNextQuestionCoroutine = null; // Clear coroutine reference
        ShowNextQuestionOrEnd(); // Attempt to show the next question
    }

    IEnumerator DelayAndStartListening(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime); // Use Realtime if timescale might be 0
        _delayListenCoroutine = null; // Clear coroutine reference

        if (TortoiseAnimator != null)
        {
            TortoiseAnimator.SetTrigger("QuestionComplete"); // Indicate question asking is done
        }

        // Start VAD Listening using the interface
        if (SVD != null)
        {
            Debug.Log("Starting VAD Listening...");
            SVD.StartListening(_currentQuestionData.maxListeningTime); // No parameter needed for the interface method
        }
        else
        {
            Debug.LogError("Cannot start listening - IVoiceDetector (SVD) is null!");
        }
    }

    IEnumerator DelayAndResetNudgeTimer(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        _delayTimerResetCoroutine = null; // Clear coroutine reference

        if (TortoiseAnimator != null)
        {
            TortoiseAnimator.SetTrigger("QuestionComplete"); // Maybe a different trigger for nudge complete?
        }

        // Resume VAD and reset nudge timer
        if (SVD != null)
        {
            if (SVD.IsPaused)
            {
                SVD.ResumeListening();
                Debug.Log("VAD resumed after nudge audio");
            }

            Debug.Log("Resetting VAD Nudge Timer...");
            SVD.ResetNudgeTimer(); // Use the interface method
        }
        else
        {
            Debug.LogError("Cannot reset nudge timer - IVoiceDetector (SVD) is null!");
        }
    }

    private void OnDisable()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        // Unsubscribe from events IF SVD is valid
        if (SVD != null)
        {
            SVD.OnVoiceRecorded -= HandleVoiceRecorded;
            SVD.OnRecordingTimeout -= HandleRecordingTimeout;
            SVD.OnFirstNudge -= HandleFirstNudge;
            SVD.OnSecondNudge -= HandleSecondNudge;

            // Ensure listening is stopped when disabled
            if (SVD.IsListening)
            {
                SVD.StopListening();
            }
        }

        // Stop any running coroutines associated with this manager
        StopAllCoroutines(); // Or stop specific ones if needed elsewhere
        _delayListenCoroutine = null;
        _delayTimerResetCoroutine = null;
        _delayNextQuestionCoroutine = null;
    }
}


