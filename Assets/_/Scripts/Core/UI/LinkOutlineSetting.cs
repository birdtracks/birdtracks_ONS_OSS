using UnityEngine;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{
    public sealed class LinkOutlineSetting : MonoBehaviour
    {
        [SerializeField] private IntGameVariable m_Variable = default;
        [SerializeField] private Toggle m_Toggle = default;


        private void OnEnable()
        {
            m_Toggle.isOn = m_Variable.Value > 0;
            m_Toggle.onValueChanged.AddListener(OnValueChangedHandler);
        }

        private void OnDisable()
        {;
            m_Toggle.onValueChanged.RemoveListener(OnValueChangedHandler);
        }

        private void OnValueChangedHandler(bool value)
        {
            m_Variable.Value = value == false ? 0 : 1;
        }
    }
}