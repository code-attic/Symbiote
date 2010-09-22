using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class ShutdownCommand
        : RedisCommand<bool>
    {
        protected const string SHUTDOWN = "SHUTDOWN\r\n";
        public bool Shutdown(IRedisConnection connection)
        {
            connection.SendExpectString(SHUTDOWN);
            return true;
        }

        public ShutdownCommand()
        {
            Command = Shutdown;
        }
    }
}