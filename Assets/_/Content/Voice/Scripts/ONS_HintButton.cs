using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Birdtracks.Game.ONS
{
    public class ONS_HintButton : MonoBehaviour,IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private int _id;
        [SerializeField] private VADSettingsUI _hintUI;
        [SerializeField] private Color _textHoverColor;

        private Color _originalColor;
        private TextMeshProUGUI _text;
        public void OnPointerDown(PointerEventData eventData)
        {
            _hintUI.OpenHint(_id);
        }
        
        

        public void SetHintUI(VADSettingsUI _hint)
        {
            _hintUI = _hint;
        }

        public void SetID(int id)
        {
            _id = id;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_text == null)
            {
                _text = GetComponent<TextMeshProUGUI>();
                _originalColor = _text.color;
            }
            
            _text.color = _textHoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            _text.color = _originalColor;
        }
    }
}
