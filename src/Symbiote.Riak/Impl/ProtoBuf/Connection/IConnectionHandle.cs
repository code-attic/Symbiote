using System;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public interface IConnectionHandle
        : IDisposable
    {
        IProtoBufConnection Connection { get; }
    }
}