using UnityEngine;

namespace BirdTracks.Game.Core
{
    public abstract class GameVariable : ScriptableObject
    {
        public event GameVariableChangedHandler ValueChanged;


        public string Name { get { return name; } }


        public abstract void SetValueFromObject(object value);

        public void SetFromString(string value)
        {
            TrySetFromString(value);
        }

        public abstract bool TrySetFromString(string value);

        public abstract object GetValue();

        public string GetAsString()
        {
            return GetValue().ToString();
        }

        protected void InvokeValueChanged()
        {
            ValueChanged?.Invoke(this);
        }
    }


    public delegate void GameVariableChangedHandler(GameVariable variable);


    public abstract class GameVariable<T> : GameVariable
    {
        [SerializeField] private T m_Value;


        public T Value
        {
            get { return m_Value; }
            set { SetValue(value); }
        }


        private void OnValidate()
        {
            InvokeValueChanged();
        }

        public void SetValue(T value)
        {
            if (ValueEquals(value))
            {
                return;
            }

            m_Value = value;
            InvokeValueChanged();
        }

        public override void SetValueFromObject(object value)
        {
            SetValue((T)value);
        }

        public override object GetValue()
        {
            return Value;
        }

        protected abstract bool ValueEquals(T value);
    }
}