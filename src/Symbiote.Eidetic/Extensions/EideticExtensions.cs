using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Eidetic.Impl;
using Symbiote.Core.Extensions;

namespace Symbiote.Eidetic.Extensions
{
    public static class MemoryExtensions
    {
        public static void Remember(this string key, Action<IRemembrance> remember)
        {
            var remembrance = new JsonRemembrance(key);
            remember(remembrance);
            remembrance.Store();
        }

        public static T Remember<T>(this string key)
        {
            return new JsonRemembrance(key).Fetch<T>();
        }

        public static void Forget(this string key)
        {
            MemoryMananger.Memory.Remove(key);
        }

        public static IDictionary<string, T> Remember<T>(this IEnumerable<string> keys)
        {
            return MemoryMananger.Memory.Get(keys)
                .ToDictionary(
                x => x.Key,
                x => (x.Value.ToString().FromJson<T>()));
        }
    }
}
