using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class IncrementCommand
        : RedisCommand<int>
    {
        protected const string INCREMENT_BY = "INCRBY {0} {1}\r\n";
        protected const string INCREMENT = "INCR {0}\r\n";
        protected int IncrementBy { get; set; }
        protected string Key { get; set; }

        public int Increment(IRedisConnection connection)
        {
            var command = 
                IncrementBy > 1
                    ? INCREMENT_BY.AsFormat(Key, IncrementBy)
                    : INCREMENT.AsFormat(Key);

            return connection.SendDataExpectInt(null, command);
        }

        public IncrementCommand(int incrementBy, string key)
        {
            IncrementBy = incrementBy;
            Key = key;
            Command = Increment;
        }
    }
}