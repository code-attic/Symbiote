namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class PooledConnectionProvider
        : IConnectionProvider
    {
        protected IConnectionPool Pool { get; set; }

        public IConnectionHandle Acquire()
        {
            return Pool.Acquire();
        }

        public PooledConnectionProvider( IConnectionPool pool )
        {
            Pool = pool;
        }
    }
}