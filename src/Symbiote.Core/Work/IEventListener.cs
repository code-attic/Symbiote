using System;

namespace Symbiote.Core.Work
{
    public interface IEventListener : IObserver<IEvent>
    {
        Type EventType { get; }
    }
}
