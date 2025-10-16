using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SweetEngine.Routine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/UI/PlaySessionService")]
    public sealed class PlaySessionService : ServiceAsset
    {
        private static PlaySessionService _Instance;
        private Queue<PlaySessionItem> _Queue = new Queue<PlaySessionItem>();
        public LoadingScreen LoadingScreen;
        private UnityMessageManager MessageManager;


        public static PlaySessionService Instance
        {
            get { return _Instance; }
        }

        public int IsSubmittingLocks { get; set; }

        public bool IsSubmittingResult => IsSubmittingLocks > 0;


        protected override void OnEnable()
        {
            base.OnEnable();
            _Instance = this;
        }

        protected override Task Initialize()
        {
            MessageManager = Host.gameObject.AddComponent<UnityMessageManager>();
            return base.Initialize();
        }

        public void QueueSessionItems(List<PlaySessionItem> items)
        {
            _Queue.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                _Queue.Enqueue(items[i]);
            }
        }

        public void LoadNextGame()
        {
            CoroutineHost.HostCoroutine(LoadNextGameRoutine());
        }

        private IEnumerator LoadNextGameRoutine()
        {
            if (_Queue.Count > 0)
            {
                var item = _Queue.Dequeue();
                var loadRoutine = item.LoadCallback();
                yield return loadRoutine;
            }
            else
            {
                Debug.Log("Fading out");
                yield return LoadingScreen.FadeOut();

                // while (IsSubmittingResult)
                // {
                //     yield return null;
                // }
                Debug.Log("Unloading");

                SceneManager.LoadScene("Empty");
                Resources.UnloadUnusedAssets();
                GC.Collect();
#if !UNITY_EDITOR
                Debug.Log("Sending message to flutter");
                MessageManager.SendMessageToFlutter("GAME_COMPLETE");
#endif
            }
        }

        public void SendSessionDataToFlutter(string message)
        {
            Debug.Log("[FLUTTER] Sending game data: " + message);
#if !UNITY_EDITOR
            string simpleJson = "{\"message\": \"Hello from Unity\"}";
            MessageManager.SendMessageToFlutter(simpleJson);
            MessageManager.SendMessageToFlutter(message);
#endif
        }
    }
}
