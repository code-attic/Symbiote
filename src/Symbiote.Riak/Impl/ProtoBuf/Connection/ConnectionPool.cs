using System;
using System.Collections.Concurrent;
using Symbiote.Riak.Config;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class ConnectionPool
        : IConnectionPool
    {
        protected IRiakConfiguration Configuration { get; set; }
        protected ConcurrentQueue<IProtoBufConnection> AvailableConnections { get; set; }
        protected ConcurrentDictionary<IProtoBufConnection, IProtoBufConnection> ReservedConnections { get; set; }
        protected IConnectionFactory ConnectionFactory { get; set; }

        public IConnectionHandle Acquire()
        {
            IProtoBufConnection connection = null;
            while (connection == null)
            {
                if (!AvailableConnections.TryDequeue(out connection))
                {
                    if (ReservedConnections.Count < Configuration.ConnectionLimit)
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
            return new PooledConnectionHandle( this, connection );
        }

        public void Release( IProtoBufConnection connection )
        {
            IProtoBufConnection removed = null;
            ReservedConnections.TryRemove(connection, out removed);
            AvailableConnections.Enqueue(connection);
        }

        public ConnectionPool( IRiakConfiguration configuration, IConnectionFactory connectionFactory )
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new ConcurrentQueue<IProtoBufConnection>();
            ReservedConnections = new ConcurrentDictionary<IProtoBufConnection, IProtoBufConnection>();
        }
    }
}