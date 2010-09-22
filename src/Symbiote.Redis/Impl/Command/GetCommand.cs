using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class GetCommand<TValue>
        : RedisCommand<TValue>
    {
        protected const string GET = "GET {0}\r\n";
        protected string Key { get; set; }

        public TValue Get(IRedisConnection connection)
        {
            var data = connection.SendExpectData(null, GET.AsFormat(Key));
            return Deserialize<TValue>(data);
        }

        public GetCommand(string key)
        {
            Key = key;
            Command = Get;
        }
    }
}