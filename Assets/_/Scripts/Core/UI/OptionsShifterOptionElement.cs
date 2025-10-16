using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{
    public sealed class OptionsShifterOptionElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public TextMeshProUGUI m_LabelText = default;
        [SerializeField] private Button m_Button = default;
        [SerializeField] private Color m_SelectedColor = default;
        private ColorBlock _normalBlock;

        public System.Object Value { get; set; }

        public int SortIndex { get; set; }

        public event UnityAction<OptionsShifterOptionElement> OnOptionSelected;


        private void Awake()
        {
            _normalBlock = m_Button.colors;
        }

        private void OnDestroy()
        {
            OnOptionSelected = null;
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

        public void Initialize(string text, System.Object value, int sortIndex = 0)
        {
            m_LabelText.text = text;
            Value = value;
            SortIndex = sortIndex;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnOptionSelected?.Invoke(this);
        }
    }
}