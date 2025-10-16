using System;
using System.Collections;
using System.Collections.Generic;
using Birdtracks.Game.ONS;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Voice Detector implementation using a small looping microphone buffer
/// and dynamically storing detected speech segments. Overcomes fixed buffer length limits.
/// </summary>
public class DynamicVoiceDetection : MonoBehaviour, IVoiceDetector
{
    [Header("Configuration")] [SerializeField]
    private int micBufferLengthSec = 5; // Internal buffer size

    [SerializeField] private float volumeThreshold = 0.02f;
    [SerializeField] private float silenceThresholdSec = 2.0f;
    [SerializeField] private float nudgeThresholdSec = 5.0f;
    
    // For two-stage detection
    [SerializeField] private float silencePauseThreshold = 0.8f; 
    [SerializeField] private float silenceEndThreshold = 2.0f;
    
    [SerializeField] private float absoluteTimeoutSec = 15.0f; // Maximum listening duration
    
    [Header("UI Feedback (Optional)")] [SerializeField]
    private Image _recordingSymbol;
    
    private bool _inPausedState = false;
    
    private AudioClip _micInputClip; // Small, looping clip
    private bool _isListening = false;
    private bool _isPaused = false;
    private int _lastSamplePosition = 0;
    private int _deviceSampleRate;
    
    private float _runningVolumeAverage = 0.02f; // Start with reasonable default
    private float _adaptiveThreshold;
    private const float ADAPTATION_RATE = 0.05f;

    private List<float> _activeRecordingData = new List<float>();
    private bool _isCapturingSpeech = false;

    private int _nudges = 0;
    private float _noSpeechTimer = 0;
    private float _silenceTimer = 0;
    
    private float _absoluteTimer = 0;
    private float _currentTimeoutSec;

    private Coroutine _listenerCoroutine;

    public bool IsListening => _isListening;
    public bool IsPaused => _isPaused;

    public event Action<AudioClip> OnVoiceRecorded;
    public event Action OnRecordingTimeout;
    public event Action OnFirstNudge;
    public event Action OnSecondNudge;

    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    public void StartListening(float questionTimeoutSec = -1)
    {
        StopListening(); // Ensure clean state

        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("[DynamicBuffer] No microphone device found!");
            return;
        }

        // Use question-specific timeout if provided, otherwise use default
        float effectiveTimeout = questionTimeoutSec > 0 ? questionTimeoutSec : absoluteTimeoutSec;
        // Store the effective timeout for use in MonitorAudio
        _currentTimeoutSec = effectiveTimeout;

        _deviceSampleRate = AudioSettings.outputSampleRate > 0 ? AudioSettings.outputSampleRate : 16000;
        _micInputClip = Microphone.Start(null, true, micBufferLengthSec, _deviceSampleRate);

        if (_micInputClip == null)
        {
            Debug.LogError("[DynamicBuffer] Microphone.Start failed. Check sample rate/device.");
            return;
        }

        while (!(Microphone.GetPosition(null) > 0))
        {
        }

        _activeRecordingData.Clear();
        _lastSamplePosition = 0;
        _isListening = true;
        _isCapturingSpeech = false;
        _nudges = 0;
        _noSpeechTimer = 0;
        _absoluteTimer = 0;
        _silenceTimer = 0;
        //_recordingSymbol.fillAmount = 1;
        if (_recordingSymbol != null) _recordingSymbol.color = Color.green;

        _listenerCoroutine = StartCoroutine(MonitorAudio());
        Debug.Log("[DynamicBuffer] Started Listening...");
    }

    public void StopListening()
    {
        StopListeningInternal(); // Use internal method
        // Clear data just in case it was stopped mid-capture externally
        if (_activeRecordingData.Count > 0)
        {
            Debug.Log("[DynamicBuffer] Listener stopped externally, discarding captured data.");
            _activeRecordingData.Clear();
        }
    }

    public void ResetNudgeTimer()
    {
        // Only reset if we are not already capturing speech
        if (_isListening && !_isCapturingSpeech)
        {
            _noSpeechTimer = 0;
            _nudges = 0; // Also reset nudges?
            //if (_recordingSymbol != null) _recordingSymbol.color = Color.green;
            Debug.Log("[DynamicBuffer] Nudge timer reset.");
        }
    }

    public void PauseListening()
    {
        if (_isListening && !_isPaused)
        {
            _isPaused = true;
            Debug.Log("[DynamicBuffer] Audio monitoring paused (microphone still active).");
        }
    }

    public void ResumeListening()
    {
        if (_isListening && _isPaused)
        {
            _isPaused = false;
            Debug.Log("[DynamicBuffer] Audio monitoring resumed.");
        }
    }

    private IEnumerator MonitorAudio()
    {
        float[] sampleChunk = new float[512];
        // Create a circular buffer to store recent audio samples
        CircularBuffer<float> preActivationBuffer = new CircularBuffer<float>(
            (int)(_deviceSampleRate * 0.2f)); // Store 200ms of pre-speech audio

        while (_isListening && _micInputClip != null)
        {
            // Skip audio processing if paused (e.g., during nudge audio playback)
            if (_isPaused)
            {
                yield return null;
                continue;
            }

            int currentPos = Microphone.GetPosition(null);

            if (currentPos < 0)
            {
                Debug.LogError("[DynamicBuffer] Mic position invalid.");
                yield break;
            }

            if (_lastSamplePosition == currentPos)
            {
                yield return null;
                continue;
            }

            // Calculate samples to read
            int samplesToRead;
            if (currentPos > _lastSamplePosition)
            {
                samplesToRead = currentPos - _lastSamplePosition;
            }
            else
            {
                samplesToRead = (_micInputClip.samples - _lastSamplePosition) + currentPos;
            }

            // Resize buffer if needed
            if (sampleChunk.Length < samplesToRead)
            {
                sampleChunk = new float[samplesToRead];
            }

            // Read the new audio samples more safely
            bool success = GetAudioSegment(_lastSamplePosition, samplesToRead, out float[] segmentBuffer);

            if (!success)
            {
                Debug.LogError("[DynamicBuffer] Failed to read audio data");
                yield return null;
                continue;
            }

            float currentVolume = CalculateRMS(segmentBuffer, segmentBuffer.Length);
            
            _absoluteTimer += (float)samplesToRead / _deviceSampleRate;

            // Check absolute timeout first
            if (_absoluteTimer >= _currentTimeoutSec)
            {
                Debug.Log($"[DynamicBuffer] Absolute timeout reached ({_currentTimeoutSec}s)");
                FinalizeRecording(false); // Force timeout
                yield break;
            }

            // Add current segment to pre-activation buffer
            if (!_isCapturingSpeech)
            {
                preActivationBuffer.Add(segmentBuffer);
            }

            // VAD Logic
            if (_isCapturingSpeech)
            {
                //if (_recordingSymbol != null) _recordingSymbol.color = Color.red;
                _noSpeechTimer = 0;
                _activeRecordingData.AddRange(segmentBuffer); // Add the whole segment read

                // Only adapt threshold when we have speech
                if (currentVolume > _runningVolumeAverage * 0.5f)
                {
                    // Update running average with exponential decay
                    _runningVolumeAverage = (_runningVolumeAverage * (1-ADAPTATION_RATE)) + (currentVolume * ADAPTATION_RATE);
                }
                
                // Set adaptive threshold as percentage of running average
                _adaptiveThreshold = _runningVolumeAverage * 0.3f; // 30% of average speech volume
                
                // Use adaptive threshold for silence detection
                if (currentVolume >= _adaptiveThreshold)
                {
                    _silenceTimer = 0f;
                    //_recordingSymbol.fillAmount = 1f;
        
                    if (_inPausedState)
                    {
                        _inPausedState = false;
                        Debug.Log("[DynamicBuffer] Speech resumed");
                    }
                }
                else
                {
                    _silenceTimer += (float)samplesToRead / _deviceSampleRate;
        
                    // First stage - potential pause
                    if (!_inPausedState && _silenceTimer >= silencePauseThreshold)
                    {
                        _inPausedState = true;
                        Debug.Log("[DynamicBuffer] Speech paused - waiting for continuation");
                    }
                    // Second stage - confirmed end
                    else if (_silenceTimer >= silenceEndThreshold)
                    {
                        Debug.Log("[DynamicBuffer] Speech ended");
                        FinalizeRecording(true);
                        yield break;
                    }

                    // Update visual feedback
                    float normalizedValue = 1f - (_silenceTimer / silenceEndThreshold);
                   // _recordingSymbol.fillAmount = normalizedValue;
                }
            }
            else // Not capturing yet
            {
                if (_recordingSymbol != null) _recordingSymbol.color = Color.green;
                if (currentVolume >= volumeThreshold)
                {
                    Debug.Log("[DynamicBuffer] Voice detected! Starting capture.");
                    _isCapturingSpeech = true;
                    _silenceTimer = 0f;
                    _noSpeechTimer = 0f;

                    // Add pre-activation buffer first to capture speech beginning
                    _activeRecordingData.AddRange(preActivationBuffer.GetAll());
                    // Then add current segment
                    _activeRecordingData.AddRange(segmentBuffer);
                }
                else
                {
                    _noSpeechTimer += (float)samplesToRead / _deviceSampleRate;
                    if (_noSpeechTimer >= nudgeThresholdSec)
                    {
                        if (!TryNudge())
                        {
                            Debug.Log($"[DynamicBuffer] No response within timeout/nudges.");
                            FinalizeRecording(false); // Timeout
                            yield break;
                        }

                        _noSpeechTimer = 0;
                    }
                }
            }

            _lastSamplePosition = currentPos;
            yield return null;
        }

        Debug.Log("[DynamicBuffer] MonitorAudio loop ended.");
    }

    // Helper method to safely get audio data
    private bool GetAudioSegment(int startPos, int samplesToRead, out float[] result)
    {
        try
        {
            List<float> currentChunkData = new List<float>(samplesToRead);
            int remainingSamples = samplesToRead;
            int readPos = startPos;

            while (remainingSamples > 0)
            {
                int readCount = Mathf.Min(remainingSamples, _micInputClip.samples - readPos);
                float[] tempRead = new float[readCount];

                if (!_micInputClip.GetData(tempRead, readPos))
                {
                    Debug.LogWarning($"[DynamicBuffer] GetData failed at position {readPos}");
                    result = new float[0];
                    return false;
                }

                currentChunkData.AddRange(tempRead);
                remainingSamples -= readCount;
                readPos = (readPos + readCount) % _micInputClip.samples;
            }

            result = currentChunkData.ToArray();
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DynamicBuffer] Exception in GetAudioSegment: {e.Message}");
            result = new float[0];
            return false;
        }
    }

    private bool TryNudge()
    {
        if (_recordingSymbol != null) _recordingSymbol.color = Color.black;
        if (_nudges == 0)
        {
            OnFirstNudge?.Invoke();
            _nudges++;
            Debug.Log("[DynamicBuffer] Nudge 1");
            return true;
        }

        if (_nudges == 1)
        {
            OnSecondNudge?.Invoke();
            _nudges++;
            Debug.Log("[DynamicBuffer] Nudge 2");
            return true;
        }

        return false;
    }

    private float CalculateRMS(float[] samples, int count)
    {
        // Same implementation as before
        if (count <= 0) return 0;
        double sum = 0;
        for (int i = 0; i < count; i++)
        {
            sum += (double)samples[i] * samples[i];
        }

        return Mathf.Sqrt((float)(sum / count));
    }

    private void FinalizeRecording(bool success)
    {
        if (!_isListening) return;

        StopListeningInternal(); // Stop mic, coroutine

        if (success && _activeRecordingData.Count > _deviceSampleRate * 0.1) // Check minimum length
        {
            Debug.Log($"[DynamicBuffer] Finalizing successful recording. Samples: {_activeRecordingData.Count}");
            AudioClip finalClip = AudioClip.Create("DynamicBufferRecording",
                _activeRecordingData.Count,
                _micInputClip != null ? _micInputClip.channels : 1, // Handle potential null clip
                _deviceSampleRate,
                false);
            finalClip.SetData(_activeRecordingData.ToArray(), 0);
            OnVoiceRecorded?.Invoke(finalClip);
        }
        else if (success) // Success but too short
        {
            Debug.Log("[DynamicBuffer] Recording too short, discarding.");
            OnRecordingTimeout?.Invoke(); // Treat as timeout?
        }
        else // Timeout occurred
        {
            Debug.Log("[DynamicBuffer] Finalizing recording due to timeout.");
            OnRecordingTimeout?.Invoke();
        }

        _activeRecordingData.Clear(); // Clear data buffer
        if (_micInputClip != null)
        {
            Destroy(_micInputClip); // Destroy the input clip
            _micInputClip = null;
        }
    }

    

    private void StopListeningInternal()
    {
        _isListening = false;
        if (_listenerCoroutine != null)
        {
            StopCoroutine(_listenerCoroutine);
            _listenerCoroutine = null;
        }

        if (Microphone.IsRecording(null))
        {
            Debug.Log("[DynamicBuffer] Ending microphone capture.");
            Microphone.End(null);
        }

        if (_recordingSymbol != null) _recordingSymbol.color = Color.white;
        _isCapturingSpeech = false;

        // Don't clear _activeRecordingData here, FinalizeRecording handles it
        // Don't destroy _micInputClip here, FinalizeRecording handles it
    }

    private void OnDisable()
    {
        StopListeningInternal();
        if (_micInputClip != null) Destroy(_micInputClip);
        _activeRecordingData.Clear();
    }

    private void OnDestroy()
    {
        StopListeningInternal();
        if (_micInputClip != null) Destroy(_micInputClip);
        _activeRecordingData.Clear();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && _isListening && !Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Debug.LogWarning("[DynamicBuffer] Mic permission lost.");
            StopListeningInternal();
            if (_micInputClip != null) Destroy(_micInputClip);
            _activeRecordingData.Clear();
        }
    }
}

// Simple circular buffer implementation
public class CircularBuffer<T>
{
    private T[] _buffer;
    private int _start;
    private int _end;
    private int _count;
    private int _capacity;

    public CircularBuffer(int capacity)
    {
        _buffer = new T[capacity];
        _capacity = capacity;
        _start = 0;
        _end = 0;
        _count = 0;
    }

    public void Add(T item)
    {
        _buffer[_end] = item;
        _end = (_end + 1) % _capacity;

        if (_count == _capacity)
        {
            _start = (_start + 1) % _capacity;
        }
        else
        {
            _count++;
        }
    }

    public void Add(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public T[] GetAll()
    {
        T[] result = new T[_count];
        int index = _start;

        for (int i = 0; i < _count; i++)
        {
            result[i] = _buffer[index];
            index = (index + 1) % _capacity;
        }

        return result;
    }
}