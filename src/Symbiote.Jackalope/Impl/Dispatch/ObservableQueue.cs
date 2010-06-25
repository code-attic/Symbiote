using System;
using System.Collections.Concurrent;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Impl.Channel;
using Symbiote.Jackalope.Impl.Serialization;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public class ObservableQueue : IQueueObserver
    {
        protected ConcurrentBag<IObserver<Envelope>> observers { get; set; }
        protected IMessageSerializer messageSerializer { get; set; }
        protected IChannelProxyFactory proxyFactory { get; set; }

        public string QueueName { get; protected set; }

        public bool Running { get; protected set; }

        public virtual void Notify(Envelope message)
        {
            observers.ForEach(x => x.OnNext(message));
        }

        public virtual void SendCompletion()
        {
            observers.ForEach(x => x.OnCompleted());
        }

        public void Start(string queueName)
        {
            QueueName = queueName;
            Running = true;
            
            while (Running)
            {
                using (var proxy = proxyFactory.GetProxyForQueue(queueName))
                {
                    Envelope envelope = null;
                    do
                    {
                        envelope = proxy.Dequeue();
                    } while (envelope == null);
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