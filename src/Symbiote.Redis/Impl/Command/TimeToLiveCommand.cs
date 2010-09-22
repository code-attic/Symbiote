using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class TimeToLiveCommand
        : RedisCommand<int>
    {
        protected const string TTL = "TTL {0}\r\n";
        public string Key { get; set; }

        public int GetTime(IRedisConnection connection)
        {
            return connection.SendDataExpectInt(null, TTL.AsFormat(Key));
        }

        public TimeToLiveCommand(string key)
        {
            Key = key;
            Command = GetTime;
        }
    }
}