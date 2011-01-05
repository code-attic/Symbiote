using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Extensions
{
    public static class ConcurentExtensions
    {
        public static Tuple<bool, TValue> TryGet<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value = default(TValue);
            var found = dictionary.TryGetValue(key, out value);
            return Tuple.Create(found, value);
        }

        public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value = default(TValue);
            var found = dictionary.TryGetValue(key, out value);
            return value;
        }
    }
}
