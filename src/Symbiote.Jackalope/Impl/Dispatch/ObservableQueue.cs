using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class ObservableQueue : IQueueObserver
    {
        private static ConcurrentDictionary<Type, List<IDispatch>> dispatchers { get; set; }
        protected ConcurrentBag<IObserver<Envelope>> observers { get; set; }
        protected IMessageSerializer messageSerializer { get; set; }
        protected IChannelProxyFactory proxyFactory { get; set; }

        public string QueueName { get; protected set; }

        public bool Running { get; protected set; }

        public virtual void Notify(Envelope notification)
        {
            //observers.AsParallel().ForAll(x => x.OnNext(notification));
            //dispatchers[notification.MessageType].AsParallel().ForAll(x => x.Dispatch(notification));

            observers.ForEach(x => x.OnNext(notification));
            dispatchers[notification.MessageType].ForEach(x => x.Dispatch(notification));
        }

        public virtual void SendCompletion()
        {
            observers.ForEach(x => x.OnCompleted());
        }

        public void Start(string queueName)
        {
            QueueName = queueName;
            Running = true;
            while(Running)
            {
                using(var proxy = proxyFactory.GetProxyForQueue(queueName))
                {
                    var envelope = proxy.Dequeue();
                    if(envelope != null)
                        Notify(envelope);
                }
            }
        }

        public void Stop()
        {
            Running = false;
            SendCompletion();
        }

        public virtual IDisposable Subscribe(IObserver<Envelope> observer)
        {
            var disposable = this as IDisposable;

            observers.Add(observer);
            
            return disposable;
        }

        public ObservableQueue(IMessageSerializer messageSerializer, IChannelProxyFactory proxyFactory)
        {
            this.messageSerializer = messageSerializer;
            this.proxyFactory = proxyFactory;
            observers = new ConcurrentBag<IObserver<Envelope>>();
            dispatchers = new ConcurrentDictionary<Type, List<IDispatch>>();
            WireUpDispatchers();
        }

        private static void WireUpDispatchers()
        {
            if(dispatchers.Count == 0)
            {
                IEnumerableExtenders.ForEach<IDispatch>(ObjectFactory.GetAllInstances<IDispatch>(), x => IEnumerableExtenders.ForEach<Type>(x.Handles, y =>
                                                        {
                                                            List<IDispatch> dispatchersForType = null;
                                                            if(dispatchers.TryGetValue(y, out dispatchersForType))
                                                            {
                                                                dispatchersForType.Add(x);
                                                            }
                                                            else
                                                            {
                                                                dispatchersForType = new List<IDispatch>() {x};
                                                                dispatchers.TryAdd(y, dispatchersForType);
                                                            }
                                                        }));
            }
        }

        public void Dispose()
        {
            while (observers.Count > 0)
            {
                IObserver<Envelope> o;
                observers.TryTake(out o);
            }
        }
    }
}