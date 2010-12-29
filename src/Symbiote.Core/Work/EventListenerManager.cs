using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Work
{
    public class EventListenerManager : IEventListenerManager
    {
        public ConcurrentDictionary<Type, List<IEventListener>> Listeners { get; set; }

        public EventListenerManager()
        {
            Listeners = new ConcurrentDictionary<Type, List<IEventListener>>();
            WireUpListeners();
        }

        public void PublishEvent(IEvent evnt)
        {
            //TODO: do we need to synchronize this?
            List<IEventListener> listeners;
            if (Listeners.TryGetValue(evnt.GetType(), out listeners))
            {
                listeners.ForEach(a =>
                                      {
                                          try
                                          {
                                              a.OnNext(evnt);
                                          }
                                          catch (Exception exception)
                                          {
                                              a.OnError(exception);
                                          }
                                      });
            }
        }

        private void WireUpListeners()
        {
            var listeners = Assimilate.GetAllInstancesOf<IEventListener>();
            listeners
                .ForEach(listener =>
                             {
                                 var dlist = new List<IEventListener>();
                                 if (Listeners.TryGetValue(listener.EventType, out dlist))
                                 {
                                     if (!dlist.Contains(listener))
                                         dlist.Add(listener);
                                 }
                                 else
                                 {
                                     dlist = new List<IEventListener>() { listener };
                                     Listeners.TryAdd(listener.EventType, dlist);
                                 }
                             });
        }
    }
}