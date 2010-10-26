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
using RabbitMQ.Client;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Rabbit.Impl.Adapter;

namespace Symbiote.Rabbit.Impl.Channels
{
    public interface IChannelProxy : IDisposable
    {
        IModel Channel { get; }
        IMessageSerializer Serializer { get; }
        void Acknowledge(ulong tag, bool multiple);
        QueueingBasicConsumer GetConsumer();
        void InitConsumer(IBasicConsumer consumer);
        string QueueName { get; }
        bool Closed { get; }
        void Send<T>(T body, string routingKey)
            where T : class;
        void Send<T>(T body, string routingKey, string correlationId)
            where T : class;
        void Reply<T>(PublicationAddress address, IBasicProperties properties, T response)
            where T : class;
        void Reject(ulong tag, bool requeue);
    }
}