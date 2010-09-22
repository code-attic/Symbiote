using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class RemoveCommand
        : RedisCommand<bool>
    {
        private const string REMOVE = "DEL {0}\r\n";
        protected List<string> Keys { get; set; }

        public bool Remove(IRedisConnection connection)
        {
            var count = connection.SendDataExpectInt(null, REMOVE.AsFormat(string.Join(" ", Keys)));
            return count == Keys.Count;
        }

        public RemoveCommand(string key)
        {
            Keys = new List<string>(new [] {key});
            Command = Remove;
        }

        public RemoveCommand(IEnumerable<string> keys)
        {
            Keys = new List<string>(keys);
            Command = Remove;
        }
    }
}