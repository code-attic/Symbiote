using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using Symbiote.Jackalope.Impl;
using Symbiote.Jackalope.Impl.Endpoint;
using Symbiote.Jackalope.Impl.Routes;

namespace Symbiote.Jackalope
{
    public interface IBus : IDisposable
    {
        IQueueStreamCollection QueueStreams { get; }
        void AddEndPoint(Action<IEndPoint> endpointConfiguration);
        void AutoRouteFromSource<T>(IObservable<T> source)
            where T : class;
        void BindQueue(string queueName, string exchangeName, params string[] routingKeys);
        bool ClearQueue(string queueName);
        void DefineRouteFor<T>(Action<IDefineRoute<T>> routeDefinition);
        void DestroyQueue(string queueName);
        void DestroyExchange(string exchangeName);
        void Subscribe(string queueName);
        void Send<T>(T body)
            where T : class;
        void Send<T>(string exchangeName, T body)
            where T : class;
        void Send<T>(string exchangeName, T body, string routingKey)
            where T : class;
        void Unsubscribe(string queueName);
    }
}