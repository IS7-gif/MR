using System;
using ZLinq;

namespace Project.Scripts.Utils.Extensions
{
    public static class ArrayExtension
    {
        public static bool Add<T>(ref T[] array, T item, bool iniq = false)
        {
            if (iniq && array.AsValueEnumerable().Contains(item))
                return false;

            Array.Resize(ref array, array.Length + 1);
            array[^1] = item;

            return true;
        }

        public static bool Remove<T>(ref T[] array, T item, bool all = false)
        {
            var removed = false;
            var count = array.Length;

            for (var i = 0; i < count; )
            {
                var arrayItem = array[i];
                if (item.Equals(arrayItem))
                {
                    removed = true;

                    count--;
                    array[i] = array[count];

                    if (false == all)
                        break;
                }
                else
                    i++;
            }

            if (removed)
                Array.Resize(ref array, count);

            return removed;
        }

        public static bool TryFind<T>(this T[] array, Func<T, bool> predicate, out T result)
        {
            var count = array.Length;
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

        public static void Execute<T>(this T[] array, Action<T> action)
        {
            var count = array.Length;
            for (var i = 0; i < count; i++)
            {
                var arrayItem = array[i];
                action(arrayItem);
            }
        }

        public static void ExecuteBackward<T>(this T[] array, Action<T> action)
        {
            var count = array.Length;
            for (var i = count - 1; i >= 0; i--)
            {
                var arrayItem = array[i];
                action(arrayItem);
            }
        }

        public static void Execute<T>(this T[] array, Action<int, T> action)
        {
            var count = array.Length;
            for (var i = 0; i < count; i++)
            {
                var arrayItem = array[i];
                action(i, arrayItem);
            }
        }

        public static float MinValue<T>(this T[] array, Func<T, float> predicate)
        {
            var count = array.Length;
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

        public static float MaxValue<T>(this T[] array, Func<T, float> predicate)
        {
            var count = array.Length;
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

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return null == array || array.Length == 0;
        }
    }
}