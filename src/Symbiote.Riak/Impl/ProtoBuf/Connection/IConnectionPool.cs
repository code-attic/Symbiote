namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public interface IConnectionPool
    {
        IConnectionHandle Acquire();
        void Release( IProtoBufConnection connection );
    }
}