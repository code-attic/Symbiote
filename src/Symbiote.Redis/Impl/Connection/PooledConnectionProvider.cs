namespace Symbiote.Redis.Impl.Connection
{
    public class PooledConnectionProvider
        : IConnectionProvider
    {
        public IConnectionPool Pool { get; protected set; }

        public IConnectionHandle Acquire()
        {
            var connection = Pool.Acquire();
            return new PooledConnectionHandle(Pool, connection);
        }

        public PooledConnectionProvider( IConnectionPool pool )
        {
            Pool = pool;
        }
    }
}