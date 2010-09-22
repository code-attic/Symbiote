using System;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class CheckAndSetCommand<TValue>
        : RedisCommand<bool>
    {
        protected const string VALUE_EXCEEDS_1GB = "Value must not exceed 1 GB";
        private const string CHECK_AND_SET = "SETNX {0} {1}\r\n";
        protected string Key { get; set; }
        protected TValue Value { get; set; }

        public bool CheckAndSet(IRedisConnection connection)
        {
            var data = Serialize(Value);
            if (data.Length > 1073741824)
                throw new ArgumentException(VALUE_EXCEEDS_1GB, "value");
            return connection.SendDataExpectInt(data, CHECK_AND_SET.AsFormat(Key, data.Length)) > 0 ? true : false;
        }

        public CheckAndSetCommand(string key, TValue value)
        {
            Key = key;
            Value = value;
            Command = CheckAndSet;
        }
    }
}