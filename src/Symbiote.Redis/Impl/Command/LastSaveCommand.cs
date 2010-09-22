using System;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class LastSaveCommand
        : RedisCommand<DateTime>
    {
        protected const long EPOCH = 621355968000000000L;
        protected const string LAST_SAVE = "LASTSAVE\r\n";
        
        public DateTime GetLast(IRedisConnection connection)
        {
            var last = connection.SendDataExpectInt(null, LAST_SAVE);
            return new DateTime(EPOCH).AddSeconds(last);
        }

        public LastSaveCommand()
        {
            Command = GetLast;
        }
    }
}