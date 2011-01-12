using System.Collections.Generic;
using System.Threading;
using Symbiote.Riak.Config;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class LockingConnectionPool
        : IConnectionPool
    {
        protected IRiakConfiguration Configuration { get; set; }
        protected ReaderWriterLockSlim AvailableLock { get; set; }
        protected ReaderWriterLockSlim ReservedLock { get; set; }
        protected Queue<IProtoBufConnection> AvailableConnections { get; set; }
        protected HashSet<IProtoBufConnection> ReservedConnections { get; set; }
        protected IConnectionFactory ConnectionFactory { get; set; }

        public IConnectionHandle Acquire()
        {
            IProtoBufConnection connection = null;
            do
            {
                AvailableLock.EnterUpgradeableReadLock();
                if (connection == null && AvailableConnections.Count > 0)
                {
                    AvailableLock.EnterWriteLock();
                    connection = AvailableConnections.Dequeue();
                    AvailableLock.ExitWriteLock();
                }
                AvailableLock.ExitUpgradeableReadLock();
                if (connection != null)
                {
                    ReservedLock.EnterWriteLock();
                    ReservedConnections.Add(connection);
                    ReservedLock.ExitWriteLock();
                }
            } while (connection == null);
            return new PooledConnectionHandle( this, connection );
        }

        public void Release( IProtoBufConnection connection )
        {
            ReservedLock.EnterWriteLock();
            AvailableLock.EnterWriteLock();
            ReservedConnections.Remove(connection);
            AvailableConnections.Enqueue(connection);
            AvailableLock.ExitWriteLock();
            ReservedLock.ExitWriteLock();
        }

        public LockingConnectionPool( IRiakConfiguration configuration, IConnectionFactory connectionFactory )
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableLock = new ReaderWriterLockSlim();
            ReservedLock = new ReaderWriterLockSlim();
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new Queue<IProtoBufConnection>();
            ReservedConnections = new HashSet<IProtoBufConnection>();
            while (AvailableConnections.Count < Configuration.ConnectionLimit)
            {
                AvailableConnections.Enqueue(ConnectionFactory.GetConnection());
            }
        }
    }
}