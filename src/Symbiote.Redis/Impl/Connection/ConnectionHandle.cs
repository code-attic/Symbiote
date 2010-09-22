using System;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Redis.Impl.Connection
{
    public class ConnectionHandle
        : IDisposable
    {
        protected RedisConnectionPool Pool { get; set; }
        public IRedisConnection Connection { get; set; }

        public static ConnectionHandle Acquire()
        {
            var pool = ServiceLocator.Current.GetInstance<RedisConnectionPool>();
            var connection = pool.Acquire();
            return new ConnectionHandle(pool, connection);
        }

        public ConnectionHandle(RedisConnectionPool pool, IRedisConnection connection)
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