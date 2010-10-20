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
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Util;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class RabbitQueueListener<TMessage>
        : QueueingBasicConsumer
        where TMessage : class
    {
        protected IChannelProxy Proxy { get; set; }
        protected IDispatcher Dispatch { get; set; }
        protected IMessageSerializer MessageSerializer { get; set; }
        protected int TotalReceived { get; set; }
        protected ConcurrentQueue<RabbitDelivery> DeliveryQueue { get; set; }
        protected bool Running { get; set; }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Dispatch.Count++;
            //Queue.Enqueue(new RabbitDelivery(
            //            consumerTag,
            //            deliveryTag,
            //            redelivered,
            //            exchange,
            //            routingKey,
            //            properties,
            //            body
            //            ));

            Dispatch.Send(
                    RabbitEnvelope<TMessage>.Create(
                        Proxy,
                        consumerTag,
                        deliveryTag,
                        redelivered,
                        exchange,
                        routingKey,
                        properties,
                        MessageSerializer.Deserialize<TMessage>(body)));
        }

        public override void HandleBasicCancelOk(string consumerTag)
        {
            Running = false;
        }

        public override void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            Running = false;
        }

        public void Dequeue()
        {
            while (Running)
            {
                object item = null;
                if (Queue.Dequeue(500, out item))
                {
                    var delivery = item as RabbitDelivery;
                    Dispatch.Send(
                    RabbitEnvelope<TMessage>.Create(
                        Proxy,
                        delivery.ConsumerTag,
                        delivery.DeliveryTag,
                        delivery.Redelivered,
                        delivery.Exchange,
                        delivery.RoutingKey,
                        delivery.Properties,
                        MessageSerializer.Deserialize<TMessage>(delivery.Body)));
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public RabbitQueueListener(IChannelProxy proxy, IDispatcher dispatch)
        {
            Proxy = proxy;
            Dispatch = dispatch;
            MessageSerializer = Assimilate.GetInstanceOf<IMessageSerializer>();
            proxy.InitConsumer(this);
            //DeliveryQueue = new ConcurrentQueue<RabbitDelivery>();
            Running = true;
            //Action dequeue = Dequeue;
            //dequeue.BeginInvoke(null, null);
        }
    }
}