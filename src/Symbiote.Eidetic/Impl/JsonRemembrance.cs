using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
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
