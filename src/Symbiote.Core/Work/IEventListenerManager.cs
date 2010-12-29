using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Symbiote.Core.Work
{
    public interface IEventListenerManager
    {
        ConcurrentDictionary<Type, List<IEventListener>> Listeners { get; set; }
        void PublishEvent(IEvent evnt);
    }
}