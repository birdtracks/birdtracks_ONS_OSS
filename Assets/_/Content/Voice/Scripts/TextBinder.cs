using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Birdtracks.Game.ONS
{
    
    public class TextBinder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textToBind;

        public void SetTextValue(float value)
        {
            _textToBind.text = value.ToString("F2");
        }
    }
}
