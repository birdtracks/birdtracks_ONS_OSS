using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace Birdtracks.Game.ONS.MuddyPaws
{

    public class VideoManager : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private VideoPlayer _player;
        [SerializeField] private string videoFileName;

        private void OnEnable()
        {
            Debug.Log("prepare video");

            StartCoroutine(PrepareAndPlayVideo());
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            PlayVideo();
        }

        private void PlayVideo()
        {
            if(_player.isPrepared)
                _player.Play();
            else
            {
                Debug.Log("[VideoPlayer] not prepared yet");
            }
        }
        
        IEnumerator PrepareAndPlayVideo()
        {
            string streamingPath = Path.Combine(Application.streamingAssetsPath, videoFileName);
            string persistentPath = Path.Combine(Application.persistentDataPath, videoFileName);
        
            // First, try direct loading (works if video is properly in StreamingAssets)
            _player.url = streamingPath;
            _player.Prepare();
        
            // Wait a bit to see if it works
            yield return new WaitForSeconds(0.5f);
        
            // If it failed (which it likely will with AAR), copy to persistent storage
            if (!_player.isPrepared)
            {
                Debug.Log("[VideoPlayer] Direct loading failed, copying video to persistent storage...");
            
                if (!File.Exists(persistentPath))
                {
                    UnityWebRequest www = UnityWebRequest.Get(streamingPath);
                    yield return www.SendWebRequest();
                
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        File.WriteAllBytes(persistentPath, www.downloadHandler.data);
                        Debug.Log("[VideoPlayer] Video copied successfully");
                    }
                    else
                    {
                        Debug.LogError($"[VideoPlayer] Failed to copy video: {www.error}");
                        yield break;
                    }
                }
            
                // Load from persistent storage
                _player.url = persistentPath;
                _player.Prepare();
            }
        }
    }
}
