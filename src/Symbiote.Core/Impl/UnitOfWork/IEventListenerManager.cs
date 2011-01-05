using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Symbiote.Core.Impl.UnitOfWork
{
    public interface IEventListenerManager
    {
        ConcurrentDictionary<Type, List<IEventListener>> Listeners { get; set; }
        void PublishEvent(IEvent evnt);
    }
}