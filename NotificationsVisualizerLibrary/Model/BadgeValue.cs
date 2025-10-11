using NotificationsVisualizerLibrary.Model.Enums;
using System;

namespace NotificationsVisualizerLibrary.Model
{
    public sealed class BadgeValue
    {
        public int Integer { get; private set; } = 0;

        public BadgeValueType Type { get; private set; } = BadgeValueType.none;

        private BadgeValue(string? value)
        {
            if (value == null)
            {
                return;
            }

            //if they provided a valid integer
            if (int.TryParse(value, out int integer))
            {
                if (integer < 0)
                {
                    integer = 0;
                }

                Type = BadgeValueType.integer;
                Integer = integer;
                return;
            }

            //otherwise if they provided a valid glyph value
            if (Enum.TryParse(value, out BadgeValueType type))
            {
                if (type == BadgeValueType.integer)
                {
                    type = BadgeValueType.none;
                }

                Type = type;
                return;
            }

            //otherwise their value wasn't valid, and we fall back to none
        }

        public override string ToString() => Type == BadgeValueType.none ? "" : Type == BadgeValueType.integer ? Integer <= 0 ? "" : Integer.ToString() : Type.ToString();

        public static BadgeValue Parse(string? str) => new(str);

        public static BadgeValue Default() => new(null);

        /// <summary>
        /// Returns true if there's a badge to be displayed. Otherwise returns false.
        /// </summary>
        /// <returns></returns>
        public bool HasBadge() => Type != BadgeValueType.none && (Type != BadgeValueType.integer || Integer > 0);
    }
}
