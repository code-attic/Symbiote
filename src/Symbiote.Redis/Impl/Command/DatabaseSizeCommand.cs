using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class DatabaseSizeCommand
        : RedisCommand<int>
    {
        protected const string DBSIZE = "DBSIZE\r\n";

        public int GetSize(IRedisConnection connection)
        {
            return connection.SendDataExpectInt(null, DBSIZE);
        }

        public DatabaseSizeCommand()
        {
            Command = GetSize;
        }
    }
}