using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class DecrementCommand
        : RedisCommand<int>
    {
        protected const string DECREMENT = "DECR {0}\r\n";
        protected const string DECREMENT_BY = "DECRBY {0} {1}\r\n";
        protected int IncrementBy { get; set; }
        protected string Key { get; set; }

        public int Increment(IRedisConnection connection)
        {
            var command =
                IncrementBy > 1
                    ? DECREMENT_BY.AsFormat(Key, IncrementBy)
                    : DECREMENT.AsFormat(Key);

            return connection.SendDataExpectInt(null, command);
        }

        public DecrementCommand(int incrementBy, string key)
        {
            IncrementBy = incrementBy;
            Key = key;
            Command = Increment;
        }
    }
}