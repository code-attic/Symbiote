using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Impl.Channel;
using Symbiote.Jackalope.Impl.Serialization;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public class ObservableConsumer : 
        DefaultBasicConsumer,
        IQueueObserver
    {
        protected IMessageSerializer Serializer { get; set; }
        protected ConcurrentBag<IObserver<Envelope>> Observers { get; set; }
        protected IChannelProxyFactory ProxyFactory { get; set; }
        protected IChannelProxy Proxy { get; set; }
        
        public string QueueName { get; protected set; }

        public virtual void Dispatch(Envelope message)
        {
            Observers.ForEach(x =>
            {
                x.OnNext(message);
            });
        }

        public virtual void SendCompletion()
        {
            Observers.ForEach(x => x.OnCompleted());
        }

        public event ObserverShutdown OnShutdown;

        public void Start(string queueName)
        {
            QueueName = queueName;
            Proxy = ProxyFactory.GetProxyForQueue(queueName);
            Model = Proxy.Channel;
            Proxy.InitConsumer(this);
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            var message = Serializer.Deserialize(body);
            var envelope = Envelope.Create(Proxy, consumerTag, deliveryTag, redelivered, exchange, routingKey,
                                           properties, message);
            if(message == null)
            {
                
            }
            else
            {
                Dispatch(envelope);   
            }
        }

        public override void HandleBasicCancelOk(string consumerTag)
        {
            Stop();
        }

        public override void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            base.HandleModelShutdown(model, reason);
            Stop();
            if (OnShutdown != null)
                OnShutdown(this);
        }

        public void Stop()
        {
            OnCancel();
            Proxy.Channel.BasicCancel(ConsumerTag);
            SendCompletion();
        }

        public virtual IDisposable Subscribe(IObserver<Envelope> observer)
        {
            var disposable = this as IDisposable;
            Observers.Add(observer);
            return disposable;
        }

        public ObservableConsumer(IChannelProxyFactory proxyFactory, IMessageSerializer serializer)
        {
            ProxyFactory = proxyFactory;
            Serializer = serializer;
            Observers = new ConcurrentBag<IObserver<Envelope>>();
        }

        public void Dispose()
        {
            Proxy.Dispose();
            while (Observers.Count > 0)
            {
                IObserver<Envelope> o;
                Observers.TryTake(out o);
            }
        }
    }
}
