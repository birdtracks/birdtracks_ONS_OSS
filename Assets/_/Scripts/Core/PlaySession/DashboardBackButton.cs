using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdTracks.Game.Core
{
    public class DashboardBackButton : MonoBehaviour
    {
        private UnityMessageManager MessageManager;

        private void Awake()
        {
            MessageManager = gameObject.AddComponent<UnityMessageManager>();
        }

        public void Do()
        {
            SceneManager.LoadScene("Empty");
            Resources.UnloadUnusedAssets();
            GC.Collect();
            MessageManager.SendMessageToFlutter("GAME_COMPLETE");
        }
    }
}