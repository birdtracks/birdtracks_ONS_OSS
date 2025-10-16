using UnityEngine;
using System;
using TMPro;
using System.Threading.Tasks;

namespace BirdTracks.Game.Core
{
    public class ErrorDialog : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_HeadingText = default;
        [SerializeField] private TextMeshProUGUI m_DescriptionText = default;
        private Canvas _canvas;
        private Action _callback;


        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }

        public void OnOkayClicked_Button()
        {
            _canvas.enabled = false;

            if (_callback != null)
            {
                _callback.Invoke();
                _callback = null;
            }
        }

        public void Show(string message)
        {
            Show("Error", message);
        }

        public void Show(Exception e)
        {
            Show(e.Message);
        }

        public void Show(string heading, string message)
        {
            _canvas.enabled = true;
            m_HeadingText.text = heading;
            m_DescriptionText.text = message;
        }

        internal void OverrideOkayClickedCallback(Action callback)
        {
            _callback = callback;
        }
    }
}
