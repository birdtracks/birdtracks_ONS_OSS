using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BirdTracks.Game.Tangrams
{
    public class VADOptionShifterOptionElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public TextMeshProUGUI m_LabelText = default;
        [SerializeField] private Button m_Button = default;
        [SerializeField] private Color m_SelectedColor = default;
        [SerializeField] public string m_SceneName;
        private UnityAction<VADOptionShifterOptionElement> _onSelect;
        private ColorBlock _normalBlock;
        private bool _storeSceneName;

        public System.Object Value { get; set; }


        private void Awake()
        {
            _normalBlock = m_Button.colors;
        }

        private void OnDestroy()
        {
            _onSelect = null;
        }

        public void Select()
        {
            ColorBlock c = m_Button.colors;
            c.normalColor = m_SelectedColor;
            c.highlightedColor = m_SelectedColor;
            m_Button.colors = c;
            m_Button.enabled = false;
            m_Button.enabled = true;
        }

        public void Deselect()
        {
            ColorBlock c = m_Button.colors;
            m_Button.colors = _normalBlock;
            m_Button.enabled = false;
            m_Button.enabled = true;
        }

        public void Initialize(string text, System.Object value, UnityAction<VADOptionShifterOptionElement> onSelectCallback, bool storeSceneName, string sceneName)
        {
            m_LabelText.text = text;
            Value = value;
            
            m_SceneName = sceneName;
            _onSelect = onSelectCallback;

            if (storeSceneName)
            {
                _storeSceneName = storeSceneName;
            }
            
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _onSelect?.Invoke(this);
        }
    }
}
