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
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;
using Symbiote.Eidetic.Extensions;

namespace Symbiote.Eidetic.Impl
{
    public class JsonRemembrance : IRemembrance
    {
        private string _key = "";
        private string _value;
        private TimeSpan _time;
        private DateTime _until;

        public IRemembrance Is<T>(T value)
        {
            _value = value.ToJson();
            return this;
        }

        public IRemembrance For(TimeSpan time)
        {
            _time = time;
            return this;
        }

        public IRemembrance Until(DateTime expiration)
        {
            _until = expiration;
            return this;
        }

        internal T Fetch<T>()
        {
            var json = MemoryMananger.Memory.Get<string>(_key);
            return json == null 
                ? default(T) : json.FromJson<T>();
        }

        internal void Store()
        {
            var noLimit = _time == default(TimeSpan);
            var noExpiration = _until == default(DateTime);

            if (noLimit && noExpiration)
            {
                MemoryMananger.Memory.Store(StoreMode.Set, _key, _value);
            }
            else if (noLimit)
            {
                MemoryMananger.Memory.Store(StoreMode.Set, _key, _value, _until);
            }
            else if (noExpiration)
            {
                MemoryMananger.Memory.Store(StoreMode.Set, _key, _value, _time);
            }
        }

        public JsonRemembrance(string key)
        {
            _key = key;
        }
    }
}
