using UnityEngine;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{
    public sealed class LinkToggle : MonoBehaviour
    {
        [SerializeField] private BoolGameVariable m_Variable = default;
        [SerializeField] private Toggle m_Toggle = default;


        private void OnEnable()
        {
            m_Toggle.isOn = m_Variable.Value;
            m_Toggle.onValueChanged.AddListener(OnValueChangedHandler);
        }

        private void OnDisable()
        {
            m_Toggle.onValueChanged.RemoveListener(OnValueChangedHandler);
        }

        private void OnValueChangedHandler(bool value)
        {
            m_Variable.Value = value;
        }
    }
}