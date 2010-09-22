using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class SaveDatabaseCommand
        : RedisCommand<bool>
    {
        protected const string SAVE = "SAVE\r\n";
        protected const string SAVE_ASYNC = "BGSAVE\r\n";
        public bool Synchronous { get; set; }

        public bool Save(IRedisConnection connection)
        {
            connection.SendExpectString(Synchronous ? SAVE : SAVE_ASYNC);
            return true;
        }

        public SaveDatabaseCommand(bool synchronously)
        {
            Synchronous = synchronously;
            Command = Save;
        }
    }
}