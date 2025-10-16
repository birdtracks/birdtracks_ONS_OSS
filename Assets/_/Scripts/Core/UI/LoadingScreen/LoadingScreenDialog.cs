using System;
using UnityEngine;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{
    public sealed class LoadingScreenDialog : MonoBehaviour
    {
        public Image FadeImage;
        public Image JJOverlay;
        public Animator JJAnimator;
        public Renderer BookMesh;

        private void Awake()
        {
            FadeImage.enabled = false;
            JJOverlay.enabled = false;
            JJAnimator.gameObject.SetActive(false);
            
            Application.targetFrameRate = 60;
        }
    }
}