using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class SetDatabaseCommand
        : RedisCommand<bool>
    {
        private const string SET_DATABASE = "SELECT {0}\r\n";
        protected int DatabaseIndex { get; set; }

        public bool SetInstance(IRedisConnection connection)
        {
            return connection.SendExpectSuccess(null, SET_DATABASE.AsFormat(DatabaseIndex));
        }

        public SetDatabaseCommand(int databaseIndex)
        {
            DatabaseIndex = databaseIndex;
            Command = SetInstance;
        }
    }
}