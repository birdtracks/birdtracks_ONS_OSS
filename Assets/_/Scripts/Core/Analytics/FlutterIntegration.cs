using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace BirdTracks.Game.Core
{
    public class FlutterIntegration : MonoBehaviour
    {
        [SerializeField] private FlutterInitArgs m_TestInitArgs;
        public LoadingScreen LoadingScreen;
        public Canvas DebugCanvas;

#if UNITY_EDITOR
        private void Awake()
        {
            StartCoroutine(Init(JsonUtility.ToJson(m_TestInitArgs)));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                StartCoroutine(Init(JsonUtility.ToJson(m_TestInitArgs)));
            }
        }
#endif

        public IEnumerator Init(string json)
        {
            var initArgs = JsonUtility.FromJson<FlutterInitArgs>(json);

            TrackosaurusAPI.Initialize(initArgs);
            DebugCanvas.enabled = true;

            while (!LoadingScreen.IsLinked)
            {
                yield return null;
            }

            if (LoadingScreen.IsFadedOut)
            {
                LoadingScreen.FadeIn();
            }
        }
    }
}
