using System;
using System.Collections.Generic;

namespace Project.Scripts.Utils.Extensions
{
    public static class ListExtension
    {
        public static bool TryFind<T>(this List<T> array, Func<T, bool> predicate, out T result)
        {
            var count = array.Count;
            for (var i = 0; i < count; i++)
            {
                var arrayItem = array[i];
                if (predicate(arrayItem))
                {
                    result = arrayItem;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static void Execute<T>(this List<T> array, Action<T> action)
        {
            var count = array.Count;
            for (var i = 0; i < count; i++)
            {
                var arrayItem = array[i];
                action(arrayItem);
            }
        }

        public static void ExecuteBackward<T>(this List<T> array, Action<T> action)
        {
            var count = array.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                var arrayItem = array[i];
                action(arrayItem);
            }
        }

        public static void Execute<T>(this List<T> array, Action<int, T> action)
        {
            var count = array.Count;
            for (var i = 0; i < count; i++)
            {
                var arrayItem = array[i];
                action(i, arrayItem);
            }
        }

        public static float MinValue<T>(this List<T> array, Func<T, float> predicate)
        {
            var count = array.Count;
            if (count == 0) 
                return default;

            var minValue = predicate(array[0]);

            for (var i = 1; i < count; i++)
            {
                var arrayItem = array[i];
                var arrayItemValue = predicate(arrayItem);

                if (arrayItemValue < minValue)
                    minValue = arrayItemValue;
            }

            return minValue;
        }

        public static float MaxValue<T>(this List<T> array, Func<T, float> predicate)
        {
            var count = array.Count;
            if (count == 0) 
                return default;

            var maxValue = predicate(array[0]);

            for (var i = 1; i < count; i++)
            {
                var arrayItem = array[i];
                var arrayItemValue = predicate(arrayItem);

                if (arrayItemValue > maxValue)
                    maxValue = arrayItemValue;
            }

            return maxValue;
        }
    }
}