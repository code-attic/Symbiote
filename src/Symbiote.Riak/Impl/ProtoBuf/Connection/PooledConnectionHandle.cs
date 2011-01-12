using System;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class PooledConnectionHandle
        : IConnectionHandle
    {
        protected IConnectionPool Pool { get; set; }
        public IProtoBufConnection Connection { get; set; }
        
        public PooledConnectionHandle( IConnectionPool pool, IProtoBufConnection connection )
        {
            Pool = pool;
            Connection = connection;
        }

        public void Dispose()
        {   
            Pool.Release( Connection );
            Pool = null;
            Connection = null;
        }
    }
}