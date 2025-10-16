using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for voice detection components.
/// Defines the contract for starting, stopping, and receiving events related to voice activity.
/// </summary>
public interface IVoiceDetector
{
    /// <summary>
    /// Event triggered when speech is successfully detected, recorded, and silence is met.
    /// The AudioClip contains the recorded speech segment.
    /// </summary>
    event Action<AudioClip> OnVoiceRecorded;

    /// <summary>
    /// Event triggered when listening stops due to timeout (no speech detected after nudges).
    /// </summary>
    event Action OnRecordingTimeout;

    /// <summary>
    /// Event triggered when the first nudge threshold is met without detecting speech.
    /// </summary>
    event Action OnFirstNudge;

    /// <summary>
    /// Event triggered when the second nudge threshold is met without detecting speech.
    /// </summary>
    event Action OnSecondNudge;

    /// <summary>
    /// Starts the voice detection process.
    /// Implementations will handle microphone initialization and monitoring.
    /// </summary>
    void StartListening(float questionTimeoutSec = -1);

    /// <summary>
    /// Manually stops the voice detection process.
    /// Cleans up resources like the microphone and coroutines.
    /// </summary>
    void StopListening();

    /// <summary>
    /// Resets the internal timer used for triggering nudges if no speech is detected.
    /// Useful if an external event (like UI feedback) should reset this timer.
    /// </summary>
    void ResetNudgeTimer();

    /// <summary>
    /// Pauses audio monitoring while keeping the microphone active.
    /// Used to prevent system audio (like nudge prompts) from being recorded.
    /// </summary>
    void PauseListening();

    /// <summary>
    /// Resumes audio monitoring after being paused.
    /// </summary>
    void ResumeListening();

    /// <summary>
    /// Gets a value indicating whether the detector is currently listening/monitoring.
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    /// Gets a value indicating whether the detector is currently paused.
    /// </summary>
    bool IsPaused { get; }
}
