using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using Symbiote.Jackalope.Impl;

namespace Symbiote.Jackalope
{
    public interface IBus : IDisposable
    {
        IQueueStreamCollection QueueMessageStreams { get; }
        void AddEndPoint(Action<IEndPoint> endpointConfiguration);
        void BindQueue(string queueName, string exchangeName, params string[] routingKeys);
        bool ClearQueue(string queueName);
        void DestroyQueue(string queueName);
        void DestroyExchange(string exchangeName);
        void Subscribe(string queueName);
        void Send<T>(string exchangeName, T body)
            where T : class;
        void Send<T>(string exchangeName, T body, string routingKey)
            where T : class;
        void Unsubscribe(string queueName);
    }
}