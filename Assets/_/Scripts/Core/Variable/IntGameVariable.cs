using UnityEngine;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/Variable/Int")]
    public sealed class IntGameVariable : GameVariable<int>
    {
        public override bool TrySetFromString(string value)
        {
            int parsedValue;

            if (!int.TryParse(value, out parsedValue))
            {
                return false;
            }

            SetValue(parsedValue);
            return true;
        }

        protected override bool ValueEquals(int value)
        {
            return Value == value;
        }
    }
}