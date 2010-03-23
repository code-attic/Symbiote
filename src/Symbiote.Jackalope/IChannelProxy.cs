using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Impl;
using Symbiote.Jackalope.Impl;

namespace Symbiote.Jackalope
{
    public interface IChannelProxy : IDisposable
    {
        IModel Channel { get; }
        void Acknowledge(ulong tag, bool multiple);
        QueueingBasicConsumer GetConsumer();
        void Send<T>(T body, string routingKey)
            where T : class;
        void Reply<T>(PublicationAddress address, IBasicProperties properties, T response)
            where T : class;
        void Reject(ulong tag, bool requeue);
        BasicGetResult Get();
        Tuple<object, IResponse> GetNext();
    }
}