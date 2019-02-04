using System;

namespace Chess.Common.Helpers
{
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum target)
        {
            var type = target.GetType();
            var fieldInfo = type.GetField(target.ToString());
            var attrs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];
            return attrs.Length > 0 ? attrs[0].StringValue : null;
        }
    }
}
