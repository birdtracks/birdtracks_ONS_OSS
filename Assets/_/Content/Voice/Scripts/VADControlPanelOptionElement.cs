using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BirdTracks.Game.Tangrams
{
    public sealed class VADControlPanelOptionElement : MonoBehaviour
    {
        public TextMeshProUGUI NameText = default;
        public TextMeshProUGUI ErrorText = default;
        public GameObject ErrorContent = default;
        public GameObject SuccessContent = default;
        public TextMeshProUGUI DescriptionText = default;
        private Action<VADControlPanelOptionElement> _callback;
        private bool _isError;
        public VADGameSettings Settings = new VADGameSettings();


        public void Initialize(
            Action<VADControlPanelOptionElement> onClickHandler,
            string storyName,
            string sceneName,
            string language)
        {
            _isError = false;
            _callback = onClickHandler;

            ErrorContent.SetActive(false);
            SuccessContent.SetActive(true);

            NameText.text = name;
            
            var m_StoryData = new VADData();
            m_StoryData.Language = language;
            m_StoryData.SceneName = sceneName;
            m_StoryData.StoryName = storyName;
            Settings.StoryData.Add(m_StoryData);

        }

        public void SetError()
        {
            _isError = true;
        }

        public void OnClicked_Button()
        {
            Debug.Log("Tangram Control Panel Option Clicked");

            if (_isError)
            {
                return;
            }
            
            _callback?.Invoke(this);
        }
    }
}
