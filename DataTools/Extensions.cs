using System;
using System.Collections.Generic;

namespace DataTools
{
    public static class Extensions
    {
        private static int? ToInt(object s)
        {
            if (s == null || "".Equals(s))
                return null;
            return int.Parse(s.ToString());
        }

        private static long? ToLong(object s)
        {
            if (s == null || "".Equals(s))
                return null;
            return long.Parse(s.ToString());
        }

        private static float? ToFloat(object s)
        {
            if (s == null || "".Equals(s))
                return null;
            return float.Parse(s.ToString());
        }

        private static string ToString(object s)
        {
            if (s == null)
                return null;
            return s.ToString();
        }

        public static float GetFloat<TKey>(this Dictionary<TKey, object> dictionary, TKey key, float defaultValue = 0)
        {
            return ToFloat(dictionary.GetValueOrDefault(key)) ?? defaultValue;
        }

        public static int GetInt<TKey>(this Dictionary<TKey, object> dictionary, TKey key, int defaultValue = 0)
        {
            return ToInt(dictionary.GetValueOrDefault(key)) ?? defaultValue;
        }

        public static long GetLong<TKey>(this Dictionary<TKey, object> dictionary, TKey key, long defaultValue = 0)
        {
            return ToLong(dictionary.GetValueOrDefault(key)) ?? defaultValue;
        }

        public static string GetString<TKey>(this Dictionary<TKey, object> dictionary, TKey key, string defaultValue = null)
        {
            return ToString(dictionary.GetValueOrDefault(key)) ?? defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            TValue result;
            if (key != null && dictionary.TryGetValue(key, out result))
                return result;
            else
                return defaultValue;
        }

        public static TResult Map<T, TResult>(this T input, Func<T, TResult> map)
        {
            if (input != null)
                return map(input);
            else
                return default(TResult);
        }

        public static TResult Map<T, TResult>(this T? input, Func<T, TResult> map)
            where T : struct
        {
            if (input != null)
                return map(input.Value);
            else
                return default(TResult);
        }
    }
}
