using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public class DispatchingObserver : IDispatchMessages
    {
        private static ConcurrentDictionary<Type, List<IDispatch>> dispatchers { get; set; }

        public void OnNext(Envelope message)
        {
            dispatchers[message.MessageType].ForEach(x => x.Dispatch(message));
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }

        private static void WireUpDispatchers()
        {
            if (dispatchers.Count == 0)
            {
                ObjectFactory.GetAllInstances<IDispatch>()
                    .ForEach(x => x.Handles.ForEach(y =>
                            {
                                List<IDispatch> dispatchersForType = null;
                                if (dispatchers.TryGetValue(y, out dispatchersForType))
                                {
                                    dispatchersForType.Add(x);
                                }
                                else
                                {
                                    dispatchersForType = new List<IDispatch>() { x };
                                    dispatchers.TryAdd(y, dispatchersForType);
                                }
                            }));
            }
        }

        public DispatchingObserver()
        {
            dispatchers = new ConcurrentDictionary<Type, List<IDispatch>>();
            WireUpDispatchers();
        }
    }
}