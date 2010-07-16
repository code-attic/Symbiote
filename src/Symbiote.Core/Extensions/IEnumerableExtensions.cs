using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Extensions
{
    public static class IEnumerableExtenders
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> nestedCollection)
        {
            return nestedCollection.SelectMany(c => c);
        }

        public static Dictionary<K, V> ZipToDictionary<K, V>(this IEnumerable<K> keys, IEnumerable<V> values)
        {
            var keyList = new List<K>();
            return keys.Take(values.Count()).ToDictionary(
                key =>
                {
                    keyList.Add(key);
                    return key;
                },
                key => values.ElementAt(keyList.Count - 1));
        }

        public static List<Tuple<K, V>> Zip<K, V>(this IEnumerable<K> collection1, IEnumerable<V> collection2)
        {
            var index = 0;
            return collection1
                    .Take(collection2.Count())
                    .Select<K, Tuple<K, V>>(value1 => Tuple.Create(value1, collection2.ElementAt(index++)))
                    .ToList();
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                return;

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var t in enumerable)
            {
                action(t);
            }
        }

        public static string BuildString(this IEnumerable<string> source, string delimiter)
        {
            return String.Join(delimiter, source);
        }

        public static IEnumerable<IEnumerable<T>> UniquePermutations<T>(this IEnumerable<T> enumerable)
        {
            var count = enumerable.Count();
            int total = (int)(Math.Pow(2, count)) - 1;
            var permutations = (from m in Enumerable.Range(1, 1 << count)
                                select
                                    from i in Enumerable.Range(0, count)
                                    where (m & (1 << i)) != 0
                                    select enumerable.Skip(i).Take(1).First());
            return
                permutations
                    .Take(total)
                    .Select(x => x).ToList();
        }

        public static T TakeRandomly<T>(this IEnumerable<T> enumerable)
        {
            var rnd = new Random((int) DateTime.Now.TimeOfDay.TotalSeconds);
            var index = rnd.Next(0, enumerable.Count());
            return enumerable.Skip(index).Take(1).First();
        }
    }
}
