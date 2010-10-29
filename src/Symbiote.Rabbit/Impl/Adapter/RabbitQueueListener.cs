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

using System.Collections.Concurrent;
using System.Threading;
using RabbitMQ.Client;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class RabbitQueueListener<TMessage>
        : QueueingBasicConsumer
    {
        protected IChannelProxy Proxy { get; set; }
        protected IDispatcher Dispatch { get; set; }
        protected RabbitChannelDefinition<TMessage> Definition { get; set; }
        protected int TotalReceived { get; set; }
        protected bool Running { get; set; }

        public override void HandleBasicDeliver(
            string consumerTag, 
            ulong deliveryTag, 
            bool redelivered, 
            string exchange, 
            string routingKey, 
            IBasicProperties properties, 
            byte[] body)
        {
            Dispatch.Count++;
            Dispatch.Send(
                    RabbitEnvelope<TMessage>.Create(
                        Proxy,
                        consumerTag,
                        deliveryTag,
                        redelivered,
                        exchange,
                        routingKey,
                        properties,
                        Definition.IncomingTransform.Reverse<byte[], TMessage>(body)));
        }

        public override void HandleBasicCancelOk(string consumerTag)
        {
            Running = false;
        }

        public override void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            Running = false;
        }

        public RabbitQueueListener(IChannelProxy proxy, IDispatcher dispatch, RabbitChannelDefinition<TMessage> definition)
        {
            Proxy = proxy;
            Dispatch = dispatch;
            Definition = definition;
            proxy.InitConsumer(this);
            Running = true;
        }
    }
}