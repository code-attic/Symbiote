using System;

namespace Symbiote.Redis.Impl.Connection
{
    public interface IConnectionHandle
        : IDisposable
    {
        IConnection Connection { get; }
    }
}