namespace Sundae
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class EnumParser
    {
        internal static T ToEnum<T>(this string text)
        {
            if (string.IsNullOrEmpty(text) && IsNullable<T>())
                return default(T);

            var normalized = Normalize(text);
            var values = Values<T>();

            return values.ContainsKey(normalized) ? values[normalized] : default(T);
        }

        private static bool IsNullable<T>() => Nullable.GetUnderlyingType(typeof(T)) != null;

        private static string Normalize(string text) => text.Trim().ToLower().Replace("-", string.Empty);

        private static IDictionary<string, T> Values<T>() =>
            Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(v => Normalize(v.ToString()), v => v);
    }
}