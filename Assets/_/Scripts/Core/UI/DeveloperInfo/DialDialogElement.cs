using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{
    public sealed class DialDialogElement : MonoBehaviour
    {
        public int DefaultValue = 2;
        public int Min = 1;
        public int Max = 10;
        public TextMeshProUGUI NumberText;
        private int _value;


        public int Value
        {
            get { return _value; }
        }


        public event UnityAction OnValueChanged;


        private void Awake()
        {
            SetValue(DefaultValue);
        }

        public void NextClicked()
        {
            SetValue(_value + 1);
        }

        public void PreviousClicked()
        {
            SetValue(_value - 1);
        }

        public void SetValue(int value)
        {
            _value = Mathf.Clamp(value, Min, Max);
            NumberText.text = _value.ToString();
            OnValueChanged?.Invoke();
        } 
        public void CheckValueRange()
        {
            _value = Mathf.Clamp(_value, Min, Max);
            NumberText.text = _value.ToString();
        }

        public void SetValueWithoutNotify(int value)
        {
            _value = Mathf.Clamp(value, Min, Max);
            NumberText.text = _value.ToString();
        }
    }
}