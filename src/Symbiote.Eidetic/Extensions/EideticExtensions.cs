/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Serialization;
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
