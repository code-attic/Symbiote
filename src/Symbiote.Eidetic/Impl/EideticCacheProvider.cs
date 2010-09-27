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

using Symbiote.Core.Cache;

namespace Symbiote.Eidetic.Impl
{
    public class EideticCacheProvider : ICacheProvider
    {
        protected IRemember _remember;

        public void Store<T>(string key, T value)
        {
            _remember.Store(StoreMode.Set, key, value);
        }

        public T Get<T>(string key)
        {
            return _remember.Get<T>(key);
        }

        public void Remove(string key)
        {
            _remember.Remove(key);
        }

        public EideticCacheProvider(IRemember remember)
        {
            _remember = remember;
        }
    }
}
