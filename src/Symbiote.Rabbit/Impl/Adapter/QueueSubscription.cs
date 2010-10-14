using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class QueueSubscription : 
        DefaultBasicConsumer,
        ISubscription
    {
        protected IChannelProxy Proxy { get; set; }
        protected IObserver<IEnvelope> Observer { get; set; }
        public string Name { get; set; }
        public bool Started { get; private set; }
        public bool Starting { get; private set; }
        public bool Stopped { get; private set; }
        public bool Stopping { get; private set; }

        public void Dispose()
        {
            
        }

        public void Start()
        {
            
        }

        
        public void Stop()
        {
            
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            
            if(Observer != null)
                Observer.OnNext()
        }

        public IDisposable Subscribe(IObserver<IEnvelope> observer)
        {
            Observer = observer;
            return new ObserverToken();
        }

        public QueueSubscription(IChannelProxy proxy)
        {
            Proxy = proxy;
        }
    }

    public class RabbitEnvelope
    {
        
    }
}
