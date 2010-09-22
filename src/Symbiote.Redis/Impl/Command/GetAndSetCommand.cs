using System;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class GetAndSetCommand<TValue>
        : RedisCommand<TValue>
    {
        protected const string VALUE_EXCEEDS_1GB = "Value must not exceed 1 GB";
        protected const string GET_AND_REPLACE = "GETSET {0} {1}\r\n";
        protected string Key { get; set; }
        protected TValue Value { get; set; }

        public TValue GetAndSet(IRedisConnection connection)
        {
            var data = Serialize(Value);
            if (data.Length > 1073741824)
                throw new ArgumentException(VALUE_EXCEEDS_1GB, "value");
            var json = connection.SendExpectData(data, GET_AND_REPLACE.AsFormat(Key, data.Length));
            return Deserialize<TValue>(json);
        }

        public GetAndSetCommand(string key, TValue value)
        {
            Key = key;
            Value = value;
            Command = GetAndSet;
        }
    }
}