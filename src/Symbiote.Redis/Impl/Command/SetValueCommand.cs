using System;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class SetValueCommand<TValue>
        : RedisCommand<bool>
    {
        protected const string VALUE_EXCEEDS_1GB = "Value must not exceed 1 GB";
        protected const string SET_VALUE = "SET {0} {1}\r\n";
        protected string Key { get; set; }
        protected TValue Value { get; set; }

        public bool Set(IRedisConnection connection)
        {
            var data = Serialize(Value);
            if (data.Length > 1073741824)
                throw new ArgumentException(VALUE_EXCEEDS_1GB, "value");
            return connection.SendExpectSuccess(data, SET_VALUE.AsFormat(Key, data.Length));
        }

        public SetValueCommand(string key, TValue value)
        {
            Key = key;
            Value = value;
            Command = Set;
        }
    }
}