using System;
using System.Collections;
using Birdtracks.Game.ONS;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class SimpleVoiceDetection : MonoBehaviour
{
    [SerializeField] private float volumeThreshold = 0.02f;
    [SerializeField] private Image _recordingSymbol;
    
    private AudioClip _recordingClip;
    private bool _isRecording = false;
    private float[] _sampleBuffer;
    private int _lastSamplePosition = 0;
    private int _deviceSampleRate;

    private int _nudges = 0;
    private float _timer = 0;

    private Coroutine _listener;

    public event Action OnVoiceEnded;
    public event Action OnFirstNudge;
    public event Action OnSecondNudge;

    private void Start()
    {
        // Check if we already have permission
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            // Request permission
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    public void StartListening(int maxRecordingLength)
    {
        StopListening();
        // Find and configure microphone
        if (Microphone.devices.Length > 0)
        {
            _deviceSampleRate = 44100; // Most common sample rate
            _recordingClip = Microphone.Start(null, true, maxRecordingLength, _deviceSampleRate);
            _sampleBuffer = new float[_deviceSampleRate * maxRecordingLength];
            _lastSamplePosition = 0;
            _isRecording = true;
            _nudges = 0;
            _recordingSymbol.color = Color.green;
            _listener = StartCoroutine(MonitorAudio());
        }
        else
        {
            Debug.LogWarning("No microphone device found!");
        }
    }

    private IEnumerator MonitorAudio()
    {
        bool voiceDetected = false;
        float voiceEndTime = 0;
        float silenceThreshold = 2.0f; // 1 second of silence to end recording
        float nudgeThreshold = 7f;
        _timer = 0f;

        while (_isRecording)
        {
            _timer += Time.deltaTime;
            // Get current position
            int currentPos = Microphone.GetPosition(null);

            if (currentPos < 0 || _lastSamplePosition == currentPos)
            {
                yield return null;
                continue;
            }

            // Get samples
            int sampleCount = 0;
            if (currentPos > _lastSamplePosition)
            {
                // Normal case
                sampleCount = currentPos - _lastSamplePosition;
                _recordingClip.GetData(_sampleBuffer, _lastSamplePosition);
            }
            else
            {
                // Buffer wrapped around
                sampleCount = (_recordingClip.samples - _lastSamplePosition) + currentPos;
                float[] tempBuffer = new float[sampleCount];

                // Get first part (from last position to end)
                _recordingClip.GetData(tempBuffer, _lastSamplePosition);

                // Get second part (from start to current position)
                if (currentPos > 0)
                {
                    float[] secondPartBuffer = new float[currentPos];
                    _recordingClip.GetData(secondPartBuffer, 0);
                    Array.Copy(secondPartBuffer, 0, tempBuffer,
                        _recordingClip.samples - _lastSamplePosition, currentPos);
                }

                Array.Copy(tempBuffer, 0, _sampleBuffer, 0, sampleCount);
            }

            // Check volume
            float volumeLevel = CalculateRMS(_sampleBuffer, sampleCount);

            // Voice detection logic
            if (!voiceDetected && volumeLevel > volumeThreshold)
            {
                _recordingSymbol.color = Color.red;
                voiceDetected = true;
                Debug.Log("Voice detected");
            }
            else if (voiceDetected && volumeLevel < volumeThreshold)
            {
                if (voiceEndTime == 0)
                {
                    voiceEndTime = Time.time;
                }
                else if (Time.time - voiceEndTime > silenceThreshold)
                {
                    Debug.Log($"Voice ended because {Time.time - voiceEndTime} is greater than silenceThreshold of {silenceThreshold}");
                    StopAndSaveRecording();
                    yield break;
                }
            }
            else if (voiceDetected && volumeLevel > volumeThreshold)
            {
                _recordingSymbol.color = Color.red;
                voiceEndTime = 0; // Reset end time if voice continues
            }
            else if (!voiceDetected && _timer > nudgeThreshold)
            {
                if (!TryNudge())
                {
                    Debug.Log("[VAD] No response, moving to next question");
                    NoResponseStop();
                    
                    yield break;
                }

                _timer = 0;
            }

            _lastSamplePosition = currentPos;
            yield return null;
        }
    }

    private bool TryNudge()
    {
        _recordingSymbol.color = Color.black;
        if (_nudges == 0)
        {
            OnFirstNudge?.Invoke();
            _nudges += 1;
            return true;
        }

        if (_nudges == 1)
        {
            OnSecondNudge?.Invoke();
            _nudges += 1;
            return true;
        }

        return false;
    }

    private float CalculateRMS(float[] samples, int count)
    {
        float sum = 0;
        for (int i = 0; i < count; i++)
        {
            sum += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sum / count);
    }

    private void StopAndSaveRecording()
    {
        _isRecording = false;
        Microphone.End(null);
        _recordingSymbol.color = Color.white;
        // Create a new clip with just the recorded data
        AudioClip recordedClip = AudioClip.Create("Recording",
            _recordingClip.samples,
            _recordingClip.channels,
            _recordingClip.frequency,
            false);

        recordedClip.SetData(_sampleBuffer, 0);
        //SaveAudioUtil.SaveToWAV(recordedClip, "Recording");
        OnVoiceEnded?.Invoke();
    }

    private void NoResponseStop()
    {
        _isRecording = false;
        Microphone.End(null);
        _recordingSymbol.color = Color.white;
        OnVoiceEnded?.Invoke();
    }

    public void ResetTimer()
    {
        _timer = 0;
    }
    
    public void StopListening()
    {
        if (_listener != null)
        {
            StopCoroutine(_listener);
            _listener = null;
        }
    
        if (_isRecording)
        {
            _isRecording = false;
            Microphone.End(null);
        }
    }
    
    private void OnDisable()
    {
        // Clean up when the component is disabled
        StopListening();
    }

    private void OnDestroy() 
    {
        // Final cleanup when the object is destroyed
        StopListening();
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        // Re-check permission when app regains focus (in case user changed permissions in settings)
        if (hasFocus && _isRecording && !Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            StopListening();
            // Notify user that mic permission was revoked
        }
    }
}