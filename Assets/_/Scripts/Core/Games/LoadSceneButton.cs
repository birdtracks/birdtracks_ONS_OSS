using System;
using System.Collections;
using System.Threading.Tasks;
using SweetEngine.Routine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdTracks.Game.Core
{

    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] public string m_SceneName = default;
        [SerializeField] private LoadingScreen m_LoadingScreen = default;
        [SerializeField] private CanvasReference m_DebugPanel = default;
        [SerializeField] private bool m_EnableDebugPanel = false;
        [SerializeField] private bool m_OverrideLoadScreenFadeIn = false;


        public void Button_OnClick()
        {
            CoroutineHost.HostCoroutine(OnClick());
        }


        public Coroutine Load(Action onSceneReady = null)
        {
            return CoroutineHost.HostCoroutine(OnClick(onSceneReady));
        }

        private IEnumerator OnClick(Action onSceneReady = null)
        {
            yield return m_LoadingScreen.FadeOut();
            OnBeforeLoadScene();
            yield return SceneManager.LoadSceneAsync(m_SceneName);
            onSceneReady?.Invoke();
            OnAfterLoadScene();
            m_DebugPanel.Instance.enabled = m_EnableDebugPanel;

            if (!m_OverrideLoadScreenFadeIn)
            {
                yield return m_LoadingScreen.FadeIn();
            }
        }

        protected virtual void OnBeforeLoadScene()
        {

        }
        protected virtual void OnAfterLoadScene()
        {
            
        }

        public void LoadInstantFade()
        {
            CoroutineHost.HostCoroutine(OnClickInstantFade());
        }

        private IEnumerator OnClickInstantFade(Action onSceneReady = null)
        {
            Debug.Log("OnClickInstant");
            m_LoadingScreen.FadeOutInstant();
            OnBeforeLoadScene();
            yield return SceneManager.LoadSceneAsync(m_SceneName);
            onSceneReady?.Invoke();
            OnAfterLoadScene();
            m_DebugPanel.Instance.enabled = m_EnableDebugPanel;

            if (!m_OverrideLoadScreenFadeIn)
            {
                yield return m_LoadingScreen.FadeIn();
            }
        }

        public void SetEnableDebugPanel (bool state)
        {
            m_EnableDebugPanel = state;
        }
    }
}