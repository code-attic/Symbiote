/* 
Copyright 2008-2010 Alex Robson

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
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public class DispatchingObserver : IDispatchMessages
    {
        private static ConcurrentDictionary<Type, List<IDispatch>> Dispatchers { get; set; }
        private static Hollywood.Agency<Envelope> Agency { get; set; }

        public void OnNext(Envelope message)
        {
            Agency.SendTo(message.CorrelationId ,message);
        }

        public void DispatchToHandlers(string id, Envelope message)
        {
            List<IDispatch> dispatchersList;
            if (Dispatchers.TryGetValue(message.MessageType, out dispatchersList))
            {
                dispatchersList.ForEach(x => x.Dispatch(message));
            }
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }

        private static void WireUpDispatchers()
        {
            if (Dispatchers.Count == 0)
            {
                ServiceLocator.Current.GetAllInstances<IDispatch>()
                    .ForEach(x => x.Handles.ForEach(y =>
                            {
                                List<IDispatch> dispatchersForType = null;
                                if (Dispatchers.TryGetValue(y, out dispatchersForType))
                                {
                                    dispatchersForType.Add(x);
                                }
                                else
                                {
                                    dispatchersForType = new List<IDispatch>() { x };
                                    Dispatchers.TryAdd(y, dispatchersForType);
                                }
                            }));
            }
        }

        public DispatchingObserver()
        {
            Dispatchers = new ConcurrentDictionary<Type, List<IDispatch>>();
            Agency = new Hollywood.Agency<Envelope>(DispatchToHandlers);
            WireUpDispatchers();
        }
    }
}