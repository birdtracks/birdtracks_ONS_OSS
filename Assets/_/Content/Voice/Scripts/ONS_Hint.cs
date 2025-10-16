using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Birdtracks.Game.ONS
{
    public class ONS_Hint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _hintText;

        [SerializeField] private Button _exitButton;

        public void SetHintText(string text)
        {
            _hintText.text = text;
        }

        public Button GetHintExitButton()
        {
            return _exitButton;
        }
    }
}
