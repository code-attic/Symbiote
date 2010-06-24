using System;
using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl
{
    public interface IChannelProxy : IDisposable
    {
        IModel Channel { get; }
        void Acknowledge(ulong tag, bool multiple);
        QueueingBasicConsumer GetConsumer();
        string QueueName { get; }
        Envelope Dequeue();
        void Send<T>(T body, string routingKey)
            where T : class;
        void Reply<T>(PublicationAddress address, IBasicProperties properties, T response)
            where T : class;
        void Reject(ulong tag, bool requeue);
    }
}