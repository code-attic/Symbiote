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
        protected ConcurrentDictionary<string, IObserver<Envelope>> Observers { get; set; }
        protected IChannelProxyFactory ProxyFactory { get; set; }
        protected IChannelProxy Proxy { get; set; }
        protected Hollywood.Agency<Envelope> Agency { get; set; }
        
        public string QueueName { get; protected set; }

        public virtual void Dispatch(string id, Envelope message)
        {
            Observers[id].OnNext(message);
        }

        public virtual void SendCompletion()
        {
            Observers.Values.ForEach(x => x.OnCompleted());
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
                Observers.Keys.ForEach(x => Agency.SendTo(x, envelope));
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
            Observers.TryAdd((Observers.Count+1).ToString(), observer);
            return disposable;
        }

        public ObservableConsumer(IChannelProxyFactory proxyFactory, IMessageSerializer serializer)
        {
            ProxyFactory = proxyFactory;
            Serializer = serializer;
            Observers = new ConcurrentDictionary<string, IObserver<Envelope>>();
            Agency = new Hollywood.Agency<Envelope>(Dispatch);
        }

        public void Dispose()
        {
            Proxy.Dispose();
            while (Observers.Count > 0)
            {
                Observers.Clear();
            }
        }
    }
}
