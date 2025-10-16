using UnityEngine;

namespace BirdTracks.Game.Core
{
    [CreateAssetMenu(menuName = "Game/Variable/Color")]
    public sealed class ColorGameVariable : GameVariable<Color>
    {
        public override bool TrySetFromString(string value)
        {
            Color parsedValue;

            if (!ColorUtility.TryParseHtmlString(value, out parsedValue))
            {
                return false;
            }

            SetValue(parsedValue);
            return true;
        }

        protected override bool ValueEquals(Color value)
        {
            return Value == value;
        }
    }
}