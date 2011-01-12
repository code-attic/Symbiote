namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public interface IConnectionProvider
    {
        IConnectionHandle Acquire();
    }
}