using System;
using System.Collections.Generic;

namespace DataTools
{
    public static class Extensions
    {
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
