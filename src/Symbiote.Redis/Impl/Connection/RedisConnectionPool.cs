using System.Collections.Concurrent;
using System.Collections.Generic;
using Symbiote.Redis.Impl.Config;

namespace Symbiote.Redis.Impl.Connection
{
    public class RedisConnectionPool
    {
        protected RedisConfiguration Configuration { get; set; }
        protected ConcurrentQueue<IRedisConnection> AvailableConnections { get; set; }
        protected HashSet<IRedisConnection> ReservedConnections { get; set; }
        protected IRedisConnectionFactory ConnectionFactory { get; set; }

        public IRedisConnection Acquire()
        {
            IRedisConnection connection = null;
            while(connection == null)
            {
                if(!AvailableConnections.TryDequeue(out connection))
                {
                    if(ReservedConnections.Count < Configuration.ConnectionLimit)
                    {
                        connection = ConnectionFactory.GetConnection();
                    }
                }
            }
            ReservedConnections.Add(connection);
            return connection;
        }

        public void Release(IRedisConnection connection)
        {
            ReservedConnections.Remove(connection);
            AvailableConnections.Enqueue(connection);
        }

        public RedisConnectionPool(RedisConfiguration configuration, IRedisConnectionFactory connectionFactory)
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new ConcurrentQueue<IRedisConnection>();
            ReservedConnections = new HashSet<IRedisConnection>();
        }
    }
}