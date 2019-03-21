using System.Collections.Generic;
using System.Linq;

namespace Halforbit.ApiClient
{
    public static class ReadOnlyCollectionExtensions
    {
        public static IReadOnlyList<TValue> With<TValue>(
            this IReadOnlyList<TValue> source,
            TValue value)
        {
            var count = source.Count;

            var r = new List<TValue>(count + 1);

            r.AddRange(source);

            r.Add(value);

            return r;
        }

        public static IReadOnlyList<TValue> WithFirst<TValue>(
            this IReadOnlyList<TValue> source,
            TValue value)
        {
            var count = source.Count;

            var r = new List<TValue>(count + 1);

            r.Add(value);

            r.AddRange(source);
            
            return r;
        }

        public static IReadOnlyDictionary<TKey, TValue> With<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> source,
            TKey key,
            TValue value)
        {
            var d = source.ToDictionary(kv => kv.Key, kv => kv.Value);

            d[key] = value;

            return d;
        }

        public static IReadOnlyDictionary<string, string> With(
            this IReadOnlyDictionary<string, string> source,
            object values)
        {
            var d = source.ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach(var property in values.GetType().GetProperties())
            {
                d[property.Name] = property.GetValue(values).ToString();
            }

            return d;
        }

        public static IReadOnlyDictionary<TKey, TValue> With<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> source,
            params (TKey Key, TValue Value)[] values)
        {
            var d = source.ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach(var kv in values)
            {
                d[kv.Key] = kv.Value;
            }

            return d;
        }

        public static IReadOnlyDictionary<TKey, TValue> With<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> source,
            IReadOnlyDictionary<TKey, TValue> values)
        {
            var d = source.ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var kv in values)
            {
                d[kv.Key] = kv.Value;
            }

            return d;
        }

        public static IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(
            this (TKey Key, TValue Value)[] values)
        {
            return values.ToDictionary(
                kv => kv.Key,
                kv => kv.Value);
        }
    }
}
