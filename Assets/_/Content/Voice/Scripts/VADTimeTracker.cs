using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BirdTracks.Game.Core;
using BirdTracks.Game.ONS;
using JetBrains.Annotations;
using UnityEngine;

namespace Birdtracks.Game.ONS
{
    public class VADTimeTracker : MonoBehaviour
    {
        private IVoiceActivityDetection _vad;
        public List<TimeStamp> SessionTimeStamps = new List<TimeStamp>();
        private int stampCounter = 0;

        public void Init(IVoiceActivityDetection vad)
        {
            _vad = vad;
            _vad.OnVoiceDetected += TimeAtVoiceDetected;
            _vad.OnVoiceEnded += TimeAtVoiceEnded;
            _vad.OnTimeUpdate += UpdateTime;
        }

        private void TimeAtVoiceDetected(bool obj)
        {
            stampCounter++;
            var ts = new TimeStamp
            {
                stampDescription = "voice detected " + stampCounter,
                voiceTimeStamp = TimestampUtility.GetTimeSinceSceneLoaded().ToString("F")
            };
            SessionTimeStamps.Add(ts);
            
        }

        private void TimeAtVoiceEnded()
        {
            var ts = new TimeStamp
            {
                stampDescription = "voice ended" + stampCounter,
                voiceTimeStamp = TimestampUtility.GetTimeSinceSceneLoaded().ToString("F")
            };
            SessionTimeStamps.Add(ts);
        }

        // Update is called once per frame
        private void UpdateTime()
        {
            TimestampUtility.UpdateTime();
        }

        public string CompileTimeStampReport()
        {
            Debug.Log("stamp count = " + SessionTimeStamps.Count);
            string report = JsonUtility.ToJson(new TimeStampListWrapper(){ timeStampListWrapper = SessionTimeStamps}, true);
            Debug.Log(report);
            return report;
        }

        public void SaveTimeStampLog()
        {
            try
            {
                string report = CompileTimeStampReport();
                string date = DateTime.Today.Date.ToString("yyyyMMdd");
                string fileName = $"TimeStampLog_{date}.json";
                string filePath = Path.Combine(Application.persistentDataPath, fileName);
                File.WriteAllText(filePath, report);
                Debug.Log($"File saved successfully at: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save file: {ex.Message}");
            }
        }
        private void OnDisable()
        {
            if (_vad == null) return;
            _vad.OnVoiceDetected -= TimeAtVoiceDetected;
            _vad.OnVoiceEnded -= TimeAtVoiceEnded;
            _vad.OnTimeUpdate -= UpdateTime;
        }
    }

    [Serializable]
    public class TimeStampListWrapper
    {
        public List<TimeStamp> timeStampListWrapper = new List<TimeStamp>();
    }

    [Serializable]
    public class TimeStamp
    {
        public string stampDescription;
        public string voiceTimeStamp;
    }
}