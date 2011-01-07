/* 
Copyright 2008-2010 Alex Robson & Jim Cowart

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
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Impl.UnitOfWork
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IList<IObserver<IEvent>> _subscribers = new List<IObserver<IEvent>>();
        private IEventListenerManager _manager;

        public EventPublisher()
        {
            
        }

        public EventPublisher(IEventListenerManager manager)
        {
            _manager = manager;
        }

        public IDisposable Subscribe( IObserver<IEvent> listener )
        {
            _subscribers.Add(listener);
            return new EventSubscriptionToken(listener, _subscribers);
        }

        public void PublishEvents( IEnumerable<IEvent> events )
        {
            events
                .ForEach(evnt =>
                             {
                                 _subscribers
                                     .ForEach(x =>
                                                  {
                                                      try
                                                      {
                                                          x.OnNext(evnt);
                                                      }
                                                      catch (Exception exception)
                                                      {
                                                          try
                                                          {
                                                              x.OnError(exception);
                                                          }
                                                          // ReSharper disable EmptyGeneralCatchClause
                                                          catch
                                                          // ReSharper restore EmptyGeneralCatchClause
                                                          {
                                                              // Well, this is awkward.  We tried to tell your listener about an event,
                                                              // but he blew up.  Then we tried to tell your listener about an exception
                                                              // and he gave us the proverbial finger. So, here we are with all the
                                                              // existential silence of an empty catch block. Selah.
                                                          }
                                                          
                                                      }
                                                  });
                                 if (_manager != null)
                                 {
                                     _manager.PublishEvent(evnt);
                                 }
                             });
        }
    }
}
