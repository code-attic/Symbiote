using Symbiote.Redis.Impl.Config;

namespace Symbiote.Redis.Impl.Connection
{
    public class RedisConnectionFactory
        : IRedisConnectionFactory
    {
        public RedisConfiguration Configuration { get; set; }

        public IRedisConnection GetConnection()
        {
            return new RedisConnection(Configuration);
        }

        public RedisConnectionFactory(RedisConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}