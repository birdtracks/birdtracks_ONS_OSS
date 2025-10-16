using UnityEngine;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/Variable/Float")]
    public sealed class FloatGameVariable : GameVariable<float>
    {
        public override bool TrySetFromString(string value)
        {
            float parsedValue;

            if (!float.TryParse(value, out parsedValue))
            {
                return false;
            }

            SetValue(parsedValue);
            return true;
        }

        protected override bool ValueEquals(float value)
        {
            return Value == value;
        }
    }
}