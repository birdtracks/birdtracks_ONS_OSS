using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{
    public sealed class MetaPlaySessionElement : MonoBehaviour
    {
        [SerializeField] private Graphic m_Button = default;
        [SerializeField] private Color m_NormalColor = default;
        [SerializeField] private Color m_SelectedColor = default;
        [SerializeField] private TextMeshProUGUI m_NameText = default;
        [SerializeField] private TextMeshProUGUI m_DescriptionText = default;
        private Action<MetaPlaySessionElement> _clickedCallback;
        
        
        public PlaySessionItem Item { get; private set; }
        
        
        public void Initialize(PlaySessionItem item, Action<MetaPlaySessionElement> clickedCallback)
        {
            Item = item;
            m_NameText.text = item.GameName;
            m_DescriptionText.text = item.Description;
            _clickedCallback = clickedCallback;
        }

        public void Select()
        {
            m_Button.color = m_SelectedColor;
        }

        public void Deselect()
        {
            m_Button.color = m_NormalColor;
        }

        public void OnPointerClick()
        {
            _clickedCallback?.Invoke(this);
        }
    }
}