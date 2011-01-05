using System;
using Symbiote.Core.Impl.Futures;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INode
    {
        void Publish<T>( T message );
        void Publish<T>( T message, Action<IEnvelope> modifyEnvelope);
        Future<R> Request<T, R>( T message );
        Future<R> Request<T, R>(T message, Action<IEnvelope> modifyEnvelope);
    }
}