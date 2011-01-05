/* 
Copyright 2008-2010 Jim Cowart

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Impl.UnitOfWork
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