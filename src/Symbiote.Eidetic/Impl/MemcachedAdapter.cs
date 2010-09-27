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
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace Symbiote.Eidetic.Impl
{
    public class MemcachedAdapter : IRemember
    {
        protected MemcachedClient _client;

        protected Dictionary<StoreMode, Enyim.Caching.Memcached.StoreMode> _modeLookup =
            new Dictionary<StoreMode, Enyim.Caching.Memcached.StoreMode>()
            {
                {StoreMode.Add, Enyim.Caching.Memcached.StoreMode.Add},
                {StoreMode.Replace, Enyim.Caching.Memcached.StoreMode.Replace},
                {StoreMode.Set, Enyim.Caching.Memcached.StoreMode.Set},
            };

        public MemcachedAdapter(MemcachedClient client)
        {
            _client = client;
        }

        public void Dispose()
        {
            
        }

        public object Get(string key)
        {
            return _client.Get(key);
        }

        public T Get<T>(string key)
        {
            return _client.Get<T>(key);
        }

        public bool TryGet(string key, out object value)
        {
            return _client.TryGet(key, out value);
        }

        public CasResult<object> GetWithCas(string key)
        {
            return _client.GetWithCas(key);
        }

        public CasResult<T> GetWithCas<T>(string key)
        {
            return _client.GetWithCas<T>(key);
        }

        public bool TryGetWithCas(string key, out CasResult<object> value)
        {
            return _client.TryGetWithCas(key, out value);
        }

        public bool Store(StoreMode mode, string key, object value)
        {
            return _client.Store(_modeLookup[mode], key, value);
        }

        public bool Store(StoreMode mode, string key, object value, TimeSpan validFor)
        {
            return _client.Store(_modeLookup[mode], key, value, validFor);
        }

        public bool Store(StoreMode mode, string key, object value, DateTime expiresAt)
        {
            return _client.Store(_modeLookup[mode], key, value, expiresAt);
        }

        public CasResult<bool> CheckAndSet(StoreMode mode, string key, object value)
        {
            return _client.Cas(_modeLookup[mode], key, value);
        }

        public CasResult<bool> CheckAndSet(StoreMode mode, string key, object value, ulong cas)
        {
            return _client.Cas(_modeLookup[mode], key, value, cas);
        }

        public CasResult<bool> CheckAndSet(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas)
        {
            return _client.Cas(_modeLookup[mode], key, value, validFor, cas);
        }

        public CasResult<bool> CheckAndSet(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas)
        {
            return _client.Cas(_modeLookup[mode], key, value, expiresAt, cas);
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta)
        {
            return _client.Increment(key, defaultValue, delta);
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            return _client.Increment(key, defaultValue, delta, validFor);
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            return _client.Increment(key, defaultValue, delta, expiresAt);
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            return _client.Increment(key, defaultValue, delta, cas);
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas)
        {
            return _client.Increment(key, defaultValue, delta, validFor, cas);
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas)
        {
            return _client.Increment(key, defaultValue, delta, expiresAt, cas);
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta)
        {
            return _client.Decrement(key, defaultValue, delta);
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            return _client.Decrement(key, defaultValue, delta, validFor);
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            return _client.Decrement(key, defaultValue, delta, expiresAt);
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            return _client.Decrement(key, defaultValue, delta, cas);
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas)
        {
            return _client.Decrement(key, defaultValue, delta, validFor, cas);
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas)
        {
            return _client.Decrement(key, defaultValue, delta, expiresAt, cas);
        }

        public bool Append(string key, ArraySegment<byte> data)
        {
            return _client.Append(key, data);
        }

        public bool Prepend(string key, ArraySegment<byte> data)
        {
            return _client.Prepend(key, data);
        }

        public CasResult<bool> Append(string key, ulong cas, ArraySegment<byte> data)
        {
            return _client.Append(key, cas, data);
        }

        public CasResult<bool> Prepend(string key, ulong cas, ArraySegment<byte> data)
        {
            return _client.Prepend(key, cas, data);
        }

        public void FlushAll()
        {
            _client.FlushAll();
        }

        public ServerStats Stats()
        {
            return _client.Stats();
        }

        public bool Remove(string key)
        {
            return _client.Remove(key);
        }

        public IDictionary<string, object> Get(IEnumerable<string> keys)
        {
            return _client.Get(keys);
        }

        public IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys)
        {
            return _client.GetWithCas(keys);
        }

        public IDictionary<string, T> PerformMultiGet<T>(IEnumerable<string> keys, Func<IMultiGetOperation, KeyValuePair<string, CacheItem>, T> collector)
        {
            return _client.PerformMultiGet(keys, collector);
        }
    }
}
