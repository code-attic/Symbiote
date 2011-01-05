using System;

namespace Symbiote.Core.Impl.UnitOfWork
{
    public interface IEventListener : IObserver<IEvent>
    {
        Type EventType { get; }
    }
}
