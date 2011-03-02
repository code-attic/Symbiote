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
    public class RabbitDelivery
    {
        public string ConsumerTag { get; set; }
        public ulong DeliveryTag { get; set; }
        public bool Redelivered { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public IBasicProperties Properties { get; set; }
        public byte[] Body { get; set; }

        public RabbitDelivery(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            ConsumerTag = consumerTag;
            DeliveryTag = deliveryTag;
            Redelivered = redelivered;
            Exchange = exchange;
            RoutingKey = routingKey;
            Properties = properties;
            Body = body;
        }
    }

    public class RabbitQueueListener
        : QueueingBasicConsumer
    {
        protected IChannelProxy Proxy { get; set; }
        protected IDispatcher Dispatch { get; set; }
        protected IMessageSerializer MessageSerializer { get; set; }
        protected int TotalReceived { get; set; }
        protected ConcurrentQueue<RabbitDelivery> DeliveryQueue { get; set; }
        protected bool Running { get; set; }
        protected Action<string, ulong, bool, string, string, IBasicProperties, byte[]> BasicDeliverAction { get; set; }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Dispatch.Count++;
            //Queue.Enqueue(body);
            Queue.Enqueue(new RabbitDelivery(
                        consumerTag,
                        deliveryTag,
                        redelivered,
                        exchange,
                        routingKey,
                        properties,
                        body
                        ));
            //BasicDeliverAction.BeginInvoke(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body, null, null);
        }

        public void HandleFoRealz(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Dispatch.Send(
                    RabbitEnvelope.Create(
                        Proxy,
                        consumerTag,
                        deliveryTag,
                        redelivered,
                        exchange,
                        routingKey,
                        properties,
                        MessageSerializer.Deserialize(body)));
            //DeliveryQueue.Enqueue(
            //    new RabbitDelivery(
            //            consumerTag,
            //            deliveryTag,
            //            redelivered,
            //            exchange,
            //            routingKey,
            //            properties,
            //            body
            //            ));
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
            while(Running)
            {
                object item = null;
                if(Queue.Dequeue(500, out item))
                {
                    var delivery = item as RabbitDelivery;
                    Dispatch.Send(
                    RabbitEnvelope.Create(
                        Proxy,
                        delivery.ConsumerTag,
                        delivery.DeliveryTag,
                        delivery.Redelivered,
                        delivery.Exchange,
                        delivery.RoutingKey,
                        delivery.Properties,
                        MessageSerializer.Deserialize(delivery.Body)));    
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
            DeliveryQueue = new ConcurrentQueue<RabbitDelivery>();
            Running = true;
            BasicDeliverAction = HandleFoRealz;
            Action dequeue = Dequeue;
            dequeue.BeginInvoke(null, null);
        }
    }
}