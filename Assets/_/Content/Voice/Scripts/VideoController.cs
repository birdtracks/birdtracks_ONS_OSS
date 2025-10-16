using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace BirdTracks.Game.ONS
{
    public class VideoController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;

        private Coroutine _LoadFollowUp;

        private VideoClip _activeClip;
        void Start()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        public void PlayVideo(bool loop)
        {
            _videoPlayer.clip = _activeClip;
            _videoPlayer.isLooping = loop;
            _videoPlayer.Play();
            
        }

        IEnumerator FollowUpClip(VideoClip fuc, bool loop)
        {
            _activeClip = fuc;
            yield return new WaitForSeconds((float)fuc.length);
            
            PlayVideo(loop);
        }

        public void PauseVideo()
        {
            _videoPlayer.Pause();
        }

        public void StopVideo()
        {
            _videoPlayer.Stop();
        }

        public void SetActiveClip(VideoClip clip)
        {
            if (_videoPlayer.isPlaying)
            {
                StopVideo();
            }

            _activeClip = clip;
            
        }
        
        public void SetActiveClipAndPlay(VideoClip clip, bool loop)
        {
            if (_videoPlayer.isPlaying)
            {
                _videoPlayer.Stop();
            }

            _activeClip = clip;
            PlayVideo(loop);
            
        }
        
        public void SetActiveClipAndPlay(VideoClip clip, VideoClip followUpClip, bool loop)
        {
            if (_videoPlayer.isPlaying)
            {
                _videoPlayer.Stop();
            }

            _activeClip = clip;
            PlayVideo(loop);
            _LoadFollowUp = StartCoroutine(FollowUpClip(followUpClip, loop));

        }
    }
}