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

using System.Collections.Generic;

namespace Symbiote.Hibernate.Impl
{
    public class InMemoryContext : ISessionContext
    {
        private Dictionary<string, object> _hash = new Dictionary<string, object>();

        public bool Contains(string key)
        {
            return _hash.ContainsKey(key);
        }

        public void Set(string key, object value)
        {
            _hash[key] = value;
        }

        public object Get(string key)
        {
            object value = null;
            _hash.TryGetValue(key, out value);
            return value;
        }
    }
}