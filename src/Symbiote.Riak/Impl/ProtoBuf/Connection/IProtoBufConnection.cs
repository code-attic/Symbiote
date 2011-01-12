using System;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public interface IProtoBufConnection
        : IDisposable
    {
        object Send<T>( T command );
    }
}
