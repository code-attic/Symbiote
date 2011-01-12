using System;

namespace Symbiote.Redis.Impl.Connection
{
    public class PooledConnectionHandle :
        IConnectionHandle
    {
        protected IConnectionPool Pool { get; set; }
        public IConnection Connection { get; set; }

        public PooledConnectionHandle(IConnectionPool pool, IConnection connection)
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