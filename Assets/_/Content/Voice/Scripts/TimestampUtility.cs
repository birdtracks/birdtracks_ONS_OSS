using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public static class TimestampUtility
    {
        private static float _timeSinceGameStarted = 0; //time since this games scene loaded
        
        /// <summary>
        /// Get time in seconds since the Unity app was opened
        /// </summary>
        /// <returns></returns>
        public static float GetTimeSinceAppOpened()
        {
            return Time.unscaledTime;
        }

        /// <summary>
        /// Get the time in seconds since the scene was loaded - only for non-additive scenes
        /// </summary>
        /// <returns></returns>
        public static float GetTimeSinceSceneLoaded()
        {
            return Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// Get the time since this specific game was started. Returned value will be different depending on if requested before or after time is updated in the frame
        /// </summary>
        /// <returns></returns>
        public static float GetTimeSinceGameStarted()
        {
            return _timeSinceGameStarted;
        }

        public static void UpdateTime()
        {
            _timeSinceGameStarted += Time.deltaTime;
        }
    }
}