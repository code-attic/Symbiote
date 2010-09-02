using System;
using RabbitMQ.Client;
using Symbiote.Jackalope.Impl.Serialization;

namespace Symbiote.Jackalope.Impl.Channel
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
        Envelope Dequeue();
        void Send<T>(T body, string routingKey)
            where T : class;
        void Reply<T>(PublicationAddress address, IBasicProperties properties, T response)
            where T : class;
        void Reject(ulong tag, bool requeue);
    }
}