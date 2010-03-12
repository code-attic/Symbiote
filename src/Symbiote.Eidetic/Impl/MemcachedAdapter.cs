using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Configuration;

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

        public MemcachedAdapter(IMemcachedClientConfiguration configuration)
        {
            _client = new MemcachedClient(configuration);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public bool Remove(string key)
        {
            return _client.Remove(key);
        }

        public object Get(string key)
        {
            return _client.Get(key);
        }

        public T Get<T>(string key)
        {
            return _client.Get<T>(key);
        }

        public long Increment(string key, uint amount)
        {
            return _client.Increment(key, amount);
        }

        public long Decrement(string key, uint amount)
        {
            return _client.Decrement(key, amount);
        }

        public bool Store(StoreMode mode, string key, object value)
        {
            return _client.Store(_modeLookup[mode], key, value);
        }

        public bool Store(StoreMode mode, string key, byte[] value, int offset, int length)
        {
            return _client.Store(_modeLookup[mode], key, value, offset, length);
        }

        public bool Store(StoreMode mode, string key, object value, TimeSpan validFor)
        {
            return _client.Store(_modeLookup[mode], key, value, validFor);
        }

        public bool Store(StoreMode mode, string key, object value, DateTime expiresAt)
        {
            return _client.Store(_modeLookup[mode], key, value, expiresAt);
        }

        public bool Store(StoreMode mode, string key, byte[] value, int offset, int length, TimeSpan validFor)
        {
            return _client.Store(_modeLookup[mode], key, value, offset, length, validFor);
        }

        public bool Store(StoreMode mode, string key, byte[] value, int offset, int length, DateTime expiresAt)
        {
            return _client.Store(_modeLookup[mode], key, value, offset, length, expiresAt);
        }

        public bool Append(string key, byte[] data)
        {
            return _client.Append(key, data);
        }

        public bool Prepend(string key, byte[] data)
        {
            return _client.Prepend(key, data);
        }

        public bool CheckAndSet(string key, object value, ulong cas)
        {
            return _client.CheckAndSet(key, value, cas);
        }

        public bool CheckAndSet(string key, byte[] value, int offset, int length, ulong cas)
        {
            return _client.CheckAndSet(key, value, offset, length, cas);
        }

        public bool CheckAndSet(string key, object value, ulong cas, TimeSpan validFor)
        {
            return _client.CheckAndSet(key, value, cas, validFor);
        }

        public bool CheckAndSet(string key, object value, ulong cas, DateTime expiresAt)
        {
            return _client.CheckAndSet(key, value, cas, expiresAt);
        }

        public bool CheckAndSet(string key, byte[] value, int offset, int length, ulong cas, TimeSpan validFor)
        {
            return _client.CheckAndSet(key, value, offset, length, cas, validFor);
        }

        public bool CheckAndSet(string key, byte[] value, int offset, int length, ulong cas, DateTime expiresAt)
        {
            return _client.CheckAndSet(key, value, offset, length, cas, expiresAt);
        }

        public void FlushAll()
        {
            _client.FlushAll();
        }

        public IDictionary<string, object> Get(IEnumerable<string> keys)
        {
            return _client.Get(keys);
        }

        public IDictionary<string, object> Get(IEnumerable<string> keys, out IDictionary<string, ulong> casValues)
        {
            return _client.Get(keys, out casValues);
        }
    }
}
