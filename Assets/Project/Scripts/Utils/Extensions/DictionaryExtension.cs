using System;
using System.Collections.Generic;

namespace Project.Scripts.Utils.Extensions
{
    public static class DictionaryExtension
    {
        public static bool IsNullOrEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            return null == dict || dict.Count == 0;
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
        {
            if (dict.TryGetValue(key, out var value))
                return value;

            value = factory(key);
            dict[key] = value;
            
            return value;
        }

        public static int RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<TKey, TValue, bool> predicate)
        {
            var keysToRemove = new List<TKey>();

            foreach (var (key, value) in dict)
            {
                if (predicate(key, value))
                    keysToRemove.Add(key);
            }

            for (var i = 0; i < keysToRemove.Count; i++)
                dict.Remove(keysToRemove[i]);

            return keysToRemove.Count;
        }

        public static void Execute<TKey, TValue>(this Dictionary<TKey, TValue> dict, Action<TKey, TValue> action)
        {
            foreach (var (key, value) in dict)
                action(key, value);
        }

        public static bool TryFind<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<TKey, TValue, bool> predicate, out TKey key, out TValue value)
        {
            foreach (var (k, v) in dict)
            {
                if (predicate(k, v))
                {
                    key = k;
                    value = v;
                    return true;
                }
            }

            key = default;
            value = default;
            
            return false;
        }
    }
}