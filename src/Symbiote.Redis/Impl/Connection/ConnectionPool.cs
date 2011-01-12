using System;
using System.Collections.Concurrent;
using Symbiote.Redis.Impl.Config;

namespace Symbiote.Redis.Impl.Connection
{
    public class ConnectionPool 
        : IConnectionPool
    {
        protected RedisConfiguration Configuration { get; set; }
        protected ConcurrentQueue<IConnection> AvailableConnections { get; set; }
        protected ConcurrentDictionary<IConnection, IConnection> ReservedConnections { get; set; }
        protected IConnectionFactory ConnectionFactory { get; set; }

        public IConnection Acquire()
        {
            IConnection connection = null;
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
            try
            {
                ReservedConnections[connection] = connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return connection;
        }

        public void Release(IConnection connection)
        {
            IConnection removed = null;
            ReservedConnections.TryRemove(connection, out removed);
            AvailableConnections.Enqueue(connection);
        }

        public ConnectionPool(RedisConfiguration configuration, IConnectionFactory connectionFactory)
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new ConcurrentQueue<IConnection>();
            ReservedConnections = new ConcurrentDictionary<IConnection, IConnection>();
        }
    }
}