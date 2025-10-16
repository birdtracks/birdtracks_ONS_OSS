using System;
using System.Collections;
using System.Collections.Generic;
using Birdtracks.Game.ONS;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

/// <summary>
/// Voice Detector implementation using a fixed-size buffer via Microphone.Start(lengthSec).
/// Stops recording after a period of silence following detected speech, or after timeout.
/// Note: If speech exceeds maxRecordingLengthSec, the beginning will be overwritten.
/// </summary>
public class FixedBufferVoiceDetection : MonoBehaviour, IVoiceDetector
{
    [Header("Configuration")] [SerializeField]
    private int maxRecordingLengthSec = 30; // Configurable max buffer size

    [SerializeField] private float volumeThreshold = 0.02f;
    [SerializeField] private float silenceThresholdSec = 2.0f;
    [SerializeField] private float nudgeThresholdSec = 5.0f;

    [Header("UI Feedback (Optional)")] [SerializeField]
    private Image _recordingSymbol;
    
    public event Action<AudioClip> OnVoiceRecorded;
    public event Action OnRecordingTimeout;
    public event Action OnFirstNudge;
    public event Action OnSecondNudge;
    
    private AudioClip _recordingClip; // The fixed-size clip
    private bool _isListening = false;
    private float[] _sampleBuffer;
    private int _lastSamplePosition = 0;
    private int _actualSpeechEndSample = 0; // Track where silence started
    private int _deviceSampleRate;

    private int _nudges = 0;
    private float _timer = 0; // Used for both silence and nudge timing

    private Coroutine _listenerCoroutine;

    public void ResumeListening()
    {
        throw new NotImplementedException();
    }

    public bool IsListening => _isListening;
    public bool IsPaused { get; }

    private void Start()
    {
        // Permission check remains the same
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
            Debug.LogWarning("[FixedBuffer] No microphone device found!");
            return;
        }

        _deviceSampleRate = AudioSettings.outputSampleRate > 0 ? AudioSettings.outputSampleRate : 44100;
        _recordingClip =
            Microphone.Start(null, true, maxRecordingLengthSec, _deviceSampleRate); // Use configured length

        if (_recordingClip == null)
        {
            Debug.LogError("[FixedBuffer] Microphone.Start failed. Check sample rate/device.");
            return;
        }

        // Allocate buffer for the entire fixed clip length
        _sampleBuffer = new float[_recordingClip.samples];

        while (!(Microphone.GetPosition(null) > 0))
        {
        } // Wait for mic to start

        _lastSamplePosition = 0;
        _actualSpeechEndSample = 0; // Reset end marker
        _isListening = true;
        _nudges = 0;
        _timer = 0; // Reset general timer

        if (_recordingSymbol != null) _recordingSymbol.color = Color.green;

        _listenerCoroutine = StartCoroutine(MonitorAudio());
        Debug.Log("[FixedBuffer] Started Listening...");
    }

    public void StopListening()
    {
        StopListeningInternal(); // Use internal method for cleanup
    }

    public void ResetNudgeTimer()
    {
        // Reset the main timer if it's currently being used for nudge timing
        // (This assumes _timer is used for nudging when no voice detected yet)
        if (!_isListening || Microphone.GetPosition(null) == 0) // Basic check if we are in nudge phase
        {
            _timer = 0;
            _nudges = 0; // Reset nudges as well? Decide based on desired behavior
            if (_recordingSymbol != null) _recordingSymbol.color = Color.green;
            Debug.Log("[FixedBuffer] Nudge timer reset.");
        }
    }

    public void PauseListening()
    {
        throw new NotImplementedException();
    }

    private IEnumerator MonitorAudio()
    {
        bool voiceDetected = false;
        float voiceEndTime = 0; // Reintroduce for silence timing clarity
        _timer = 0f; // Reset timer specifically for this monitoring session


        while (_isListening && _recordingClip != null)
        {
            _timer += Time.deltaTime; // General purpose timer increment

            int currentPos = Microphone.GetPosition(null);

            if (currentPos < 0 || _lastSamplePosition == currentPos)
            {
                yield return null;
                continue;
            }

            // Read Samples
            // This part reads data into _sampleBuffer but we need to be careful
            // We calculate volume on the *new* segment only for detection
            int samplesToRead;
            int readStartPos = _lastSamplePosition;
            float[] segmentBuffer;

            if (currentPos > _lastSamplePosition)
            {
                samplesToRead = currentPos - _lastSamplePosition;
                segmentBuffer = new float[samplesToRead];
                _recordingClip.GetData(segmentBuffer, _lastSamplePosition); // Read only the new segment
                // Optionally copy to the main buffer if needed later, but maybe not necessary
                // Array.Copy(segmentBuffer, 0, _sampleBuffer, _lastSamplePosition, samplesToRead);
            }
            else // Wrap around
            {
                samplesToRead = (_recordingClip.samples - _lastSamplePosition) + currentPos;
                segmentBuffer = new float[samplesToRead];

                // Read first part (end of clip)
                int firstPartLength = _recordingClip.samples - _lastSamplePosition;
                float[] firstPart = new float[firstPartLength];
                _recordingClip.GetData(firstPart, _lastSamplePosition);
                Array.Copy(firstPart, 0, segmentBuffer, 0, firstPartLength);

                // Read second part (start of clip)
                if (currentPos > 0)
                {
                    float[] secondPart = new float[currentPos];
                    _recordingClip.GetData(secondPart, 0);
                    Array.Copy(secondPart, 0, segmentBuffer, firstPartLength, currentPos);
                }
                // Optionally copy to main buffer if needed later
                // Array.Copy(segmentBuffer, 0, _sampleBuffer, 0, samplesToRead); // This logic needs care if _sampleBuffer is used
            }


            // Check volume of the new segment
            float volumeLevel = CalculateRMS(segmentBuffer, samplesToRead);

            //VAD Logic
            if (!voiceDetected && volumeLevel > volumeThreshold)
            {
                if (_recordingSymbol != null) _recordingSymbol.color = Color.red;
                voiceDetected = true;
                _timer = 0; // Reset timer to use for silence detection now
                voiceEndTime = 0;
                Debug.Log("[FixedBuffer] Voice detected");
            }
            else if (voiceDetected && volumeLevel < volumeThreshold)
            {
                if (voiceEndTime == 0)
                {
                    voiceEndTime = Time.time; // Mark start of potential silence
                    _actualSpeechEndSample = currentPos; // Mark the sample where silence began
                }
                // Use Time.time difference for silence duration check
                else if (Time.time - voiceEndTime > silenceThresholdSec)
                {
                    Debug.Log($"[FixedBuffer] Voice ended (silence). Duration: {Time.time - voiceEndTime}s");
                    StopAndFinalizeRecording(true); // Finalize successfully
                    yield break;
                }
            }
            else if (voiceDetected && volumeLevel > volumeThreshold)
            {
                if (_recordingSymbol != null) _recordingSymbol.color = Color.red;
                voiceEndTime = 0; // Reset silence timer if voice continues
                _actualSpeechEndSample = 0; // Reset end marker
            }
            else if (!voiceDetected && _timer > nudgeThresholdSec) // Check nudge timeout ONLY if voice not yet detected
            {
                if (!TryNudge())
                {
                    Debug.Log("[FixedBuffer] No response within timeout/nudges.");
                    StopAndFinalizeRecording(false); // Finalize due to timeout
                    yield break;
                }

                _timer = 0; // Reset timer after successful nudge
            }

            _lastSamplePosition = currentPos;
            yield return null;
        }

        Debug.Log("[FixedBuffer] MonitorAudio loop ended.");
    }

    private bool TryNudge()
    {
        if (_recordingSymbol != null) _recordingSymbol.color = Color.black;
        if (_nudges == 0)
        {
            OnFirstNudge?.Invoke();
            _nudges++;
            Debug.Log("[FixedBuffer] Nudge 1");
            return true;
        }

        if (_nudges == 1)
        {
            OnSecondNudge?.Invoke();
            _nudges++;
            Debug.Log("[FixedBuffer] Nudge 2");
            return true;
        }

        return false;
    }

    private float CalculateRMS(float[] samples, int count)
    {
        if (count <= 0) return 0;
        double sum = 0;
        for (int i = 0; i < count; i++)
        {
            sum += (double)samples[i] * samples[i];
        }

        return Mathf.Sqrt((float)(sum / count));
    }

    // Modified to create and pass AudioClip
    private void StopAndFinalizeRecording(bool success)
    {
        if (!_isListening) return; // Already stopped

        int finalSamplePos = Microphone.GetPosition(null); // Get final position before stopping
        if (success && _actualSpeechEndSample == 0)
        {
            // If success but silence never started, use current pos
            _actualSpeechEndSample = finalSamplePos;
        }

        StopListeningInternal(); // Stop mic, coroutine

        if (success)
        {
            Debug.Log($"[FixedBuffer] Finalizing successful recording. End sample approx: {_actualSpeechEndSample}");

            // --- Create AudioClip ---
            // This is tricky with the fixed buffer. We *should* try to capture only the relevant part.
            // A simple approach: Copy data up to _actualSpeechEndSample (or currentPos if needed).
            // This doesn't handle wrap-around perfectly for *extraction* but is better than nothing.

            // Get all data from the fixed clip first.
            _recordingClip.GetData(_sampleBuffer, 0);

            // Determine the number of samples to include in the final clip.
            // This needs careful calculation if wrap-around occurred *during* the speech segment.
            // For simplicity, let's just take samples up to the end point. Might include initial silence.
            // A truly robust solution would need to track the *start* of speech too.
            int samplesInFinalClip = _actualSpeechEndSample > 0 ? _actualSpeechEndSample : finalSamplePos;
            if (samplesInFinalClip <= 0) samplesInFinalClip = _sampleBuffer.Length; // Fallback

            if (samplesInFinalClip > _deviceSampleRate * 0.1) // Basic check for minimum length
            {
                AudioClip recordedClip = AudioClip.Create("FixedBufferRecording",
                    samplesInFinalClip,
                    _recordingClip.channels,
                    _recordingClip.frequency,
                    false);

                recordedClip.SetData(_sampleBuffer, 0); // Sets data starting from beginning of _sampleBuffer
                OnVoiceRecorded?.Invoke(recordedClip); // Pass the created clip
            }
            else
            {
                Debug.Log("[FixedBuffer] Recording deemed too short after processing, discarding.");
                OnRecordingTimeout?.Invoke(); // Treat very short recordings as timeout? Or new event?
            }
        }
        else // Timeout occurred
        {
            Debug.Log("[FixedBuffer] Finalizing recording due to timeout.");
            OnRecordingTimeout?.Invoke();
        }

        // Clean up the main recording clip used for input
        if (_recordingClip != null)
        {
            Destroy(_recordingClip);
            _recordingClip = null;
        }

        _sampleBuffer = null; // Release buffer memory
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
            Debug.Log("[FixedBuffer] Ending microphone capture.");
            Microphone.End(null);
        }

        if (_recordingSymbol != null) _recordingSymbol.color = Color.white;

        // Don't destroy _recordingClip here yet if needed by StopAndFinalizeRecording
    }

    private void OnDisable()
    {
        StopListeningInternal();
    }

    private void OnDestroy()
    {
        StopListeningInternal();
        if (_recordingClip != null) Destroy(_recordingClip);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && _isListening && !Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Debug.LogWarning("[FixedBuffer] Mic permission lost.");
            StopListeningInternal();
            if (_recordingClip != null) Destroy(_recordingClip); // Clean up clip too
        }
    }
}