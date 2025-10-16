using UnityEngine;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/Variable/Bool")]
    public sealed class BoolGameVariable : GameVariable<bool>
    {
        public override bool TrySetFromString(string value)
        {
            bool parsedValue;

            if (!bool.TryParse(value, out parsedValue))
            {
                return false;
            }

            SetValue(parsedValue);
            return true;
        }

        protected override bool ValueEquals(bool value)
        {
            return Value == value;
        }
    }
}