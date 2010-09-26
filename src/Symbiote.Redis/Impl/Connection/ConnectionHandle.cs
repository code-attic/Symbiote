using System;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Redis.Impl.Connection
{
    public class ConnectionHandle
        : IDisposable
    {
        protected IRedisConnectionPool Pool { get; set; }
        public IRedisConnection Connection { get; set; }

        public static ConnectionHandle Acquire()
        {
            var pool = ServiceLocator.Current.GetInstance<IRedisConnectionPool>();
            var connection = pool.Acquire();
            return new ConnectionHandle(pool, connection);
        }

        public ConnectionHandle(IRedisConnectionPool pool, IRedisConnection connection)
        {
            Pool = pool;
            Connection = connection;
        }

        public void Dispose()
        {
            Pool.Release(Connection);
            Pool = null;
            Connection = null;
        }
    }
}