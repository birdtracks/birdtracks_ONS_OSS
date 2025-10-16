using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/UI/LoadingScreen")]
    public sealed class LoadingScreen : ServiceAsset
    {
        [SerializeField] private LoadingScreenDialogContainer m_Dialog = default;
        [SerializeField] private float m_DefaultFade = 1f;
        [SerializeField] private float m_JJFade = 0.5f;

        public bool IsLinked
        {
            get { return m_Dialog != null && m_Dialog.Instance != null; }
        }
        public bool IsFadedOut
        {
            get { return m_Dialog == null ? false : m_Dialog.Instance.FadeImage.enabled != false; }
        }

        public Coroutine FadeOut()
        {
            return FadeOut(0f);
        }

        public Coroutine FadeOut(float delay)
        {
            StopAllCoroutines();
            return StartCoroutine(DoFadeOut(delay, false));
        }
        public Coroutine FadeOut(bool showJJ)
        {
            return FadeOut(0f, showJJ);
        }

        public Coroutine FadeOut(float delay, bool showJJ)
        {
            StopAllCoroutines();
            return StartCoroutine(DoFadeOut(delay, showJJ));
        }

        private IEnumerator DoFadeOut(float delay, bool showJJ)
        {
            if (m_Dialog == null || m_Dialog.Instance == null)
            {
                yield break;
            }

            m_Dialog.Instance.FadeImage.enabled = true;
            m_Dialog.Instance.FadeImage.color = new Color(0f, 0f, 0f, 0f);
            m_Dialog.Instance.JJOverlay.color = new Color(0f, 0f, 0f, 1f);

            yield return new WaitForSecondsRealtime(delay);

            float time = 0f;

            while (time < m_DefaultFade)
            {
                time += Time.unscaledDeltaTime;
                m_Dialog.Instance.FadeImage.color = new Color(0f, 0f, 0f, time / m_DefaultFade);
                yield return null;
            }

            m_Dialog.Instance.FadeImage.color = new Color(0f, 0f, 0f, 1f);
            time = 0f;

            if (showJJ)
            {
                m_Dialog.Instance.JJOverlay.enabled = true;
                m_Dialog.Instance.JJAnimator.gameObject.SetActive(true);

                var index = Random.Range(0, 3);
                m_Dialog.Instance.JJAnimator.SetInteger("Index", index);
                m_Dialog.Instance.BookMesh.gameObject.SetActive(index == 0);
            }

            while (time < m_JJFade)
            {
                time += Time.unscaledDeltaTime;
                m_Dialog.Instance.JJOverlay.color = new Color(0f, 0f, 0f, 1f - (time / m_JJFade));
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);

            m_Dialog.Instance.JJOverlay.color = new Color(0f, 0f, 0f, 0f);
        }

        public Coroutine FadeIn()
        {
            return FadeIn(0f);
        }

        public Coroutine FadeIn(float delay)
        {
            StopAllCoroutines();
            return StartCoroutine(DoFadeIn(delay));
        }


        private IEnumerator DoFadeIn(float delay)
        {
            if (m_Dialog == null ||
                m_Dialog.Instance == null)
            {
                yield break;
            }

            m_Dialog.Instance.FadeImage.enabled = true;
            m_Dialog.Instance.JJOverlay.enabled = true;
            m_Dialog.Instance.FadeImage.color = new Color(0f, 0f, 0f, 1f);
            m_Dialog.Instance.JJOverlay.color = new Color(0f, 0f, 0f, 0f);

            float time = 0f;

            while (time < m_JJFade)
            {
                time += Time.unscaledDeltaTime;
                m_Dialog.Instance.JJOverlay.color = new Color(0f, 0f, 0f, (time / m_JJFade));
                yield return null;
            }


            m_Dialog.Instance.JJAnimator.gameObject.SetActive(false);
            m_Dialog.Instance.JJOverlay.enabled = false;
            yield return new WaitForSecondsRealtime(delay);

            time = 0f;

            while (time < m_DefaultFade)
            {
                time += Time.unscaledDeltaTime;
                m_Dialog.Instance.FadeImage.color = new Color(0f, 0f, 0f, 1f - (time / m_DefaultFade));
                yield return null;
            }

            m_Dialog.Instance.FadeImage.color = new Color(0f, 0f, 0f, 0f);
            m_Dialog.Instance.FadeImage.enabled = false;
        }

        internal void FadeOutInstant()
        {
            m_Dialog.Instance.FadeImage.color = new Color(0f, 0f, 0f, 1f);
        }
    }
}