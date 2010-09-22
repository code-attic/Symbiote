using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class FlushCommand
        : RedisCommand<bool>
    {
        protected const string FLUSH_ALL = "FLUSHALL\r\n";
        protected const string FLUSH_DATABASE = "FLUSHDB\r\n";
        protected bool FlushAll { get; set; }
        
        public bool Flush(IRedisConnection connection)
        {
            connection.SendExpectString(FlushAll ? FLUSH_ALL : FLUSH_DATABASE);
            return true;
        }

        public FlushCommand(bool flushAll)
        {
            FlushAll = flushAll;
            Command = Flush;
        }
    }
}