using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Symbiote.Redis.Impl.Config;
using System.Linq;

namespace Symbiote.Redis.Impl.Connection
{
    public interface IRedisConnectionPool
    {
        IRedisConnection Acquire();
        void Release(IRedisConnection connection);
    }

    public class LockingRedisConnectionPool
        : IRedisConnectionPool
    {
        protected RedisConfiguration Configuration { get; set; }
        protected ReaderWriterLockSlim AvailableLock { get; set; }
        protected ReaderWriterLockSlim ReservedLock { get; set; }
        protected Queue<IRedisConnection> AvailableConnections { get; set; }
        protected HashSet<IRedisConnection> ReservedConnections { get; set; }
        protected IRedisConnectionFactory ConnectionFactory { get; set; }

        public IRedisConnection Acquire()
        {
            IRedisConnection connection = null;
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
            return connection;
        }

        public void Release(IRedisConnection connection)
        {
            ReservedLock.EnterWriteLock();
            AvailableLock.EnterWriteLock();
            ReservedConnections.Remove(connection);
            AvailableConnections.Enqueue(connection);
            AvailableLock.ExitWriteLock();
            ReservedLock.ExitWriteLock();
        }

        public LockingRedisConnectionPool(RedisConfiguration configuration, IRedisConnectionFactory connectionFactory)
        {
            AvailableLock = new ReaderWriterLockSlim();
            ReservedLock = new ReaderWriterLockSlim();
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new Queue<IRedisConnection>();
            ReservedConnections = new HashSet<IRedisConnection>();
            while(AvailableConnections.Count < Configuration.ConnectionLimit)
            {
                AvailableConnections.Enqueue(ConnectionFactory.GetConnection());
            }
        }
    }

    public class RedisConnectionPool 
        : IRedisConnectionPool
    {
        protected RedisConfiguration Configuration { get; set; }
        protected ConcurrentQueue<IRedisConnection> AvailableConnections { get; set; }
        protected ConcurrentDictionary<IRedisConnection, IRedisConnection> ReservedConnections { get; set; }
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

        public void Release(IRedisConnection connection)
        {
            IRedisConnection removed = null;
            ReservedConnections.TryRemove(connection, out removed);
            AvailableConnections.Enqueue(connection);
        }

        public RedisConnectionPool(RedisConfiguration configuration, IRedisConnectionFactory connectionFactory)
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new ConcurrentQueue<IRedisConnection>();
            ReservedConnections = new ConcurrentDictionary<IRedisConnection, IRedisConnection>();
        }
    }
}