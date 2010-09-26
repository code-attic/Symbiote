using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Impl;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;
using Symbiote.Jackalope.Impl.Channel;
using Symbiote.Jackalope.Impl.Dispatch;
using Symbiote.Jackalope.Impl.Endpoint;
using Symbiote.Jackalope.Impl.Routes;
using Symbiote.Jackalope.Impl.Subscriptions;

namespace Symbiote.Jackalope.Impl
{
    public class Bus : IBus
    {
        private IChannelProxyFactory _channelFactory;
        private ISubscriptionManager _subscriptionManager;
        private ConcurrentDictionary<Type, IDispatch> _functionalDispatchers 
            = new ConcurrentDictionary<Type, IDispatch>();
        private IEndpointManager _endpointManager;
        private IRouteManager _routeManager;

        public IQueueStreamCollection QueueStreams { get { return _subscriptionManager; } }

        public void AddEndPoint(Action<IEndPoint> endpointConfiguration)
        {
            var endpoint = new BusEndPoint();
            endpointConfiguration(endpoint);
            _endpointManager.AddEndpoint(endpoint);
        }

        public void AutoRouteFromSource<T>(IObservable<T> source)
            where T : class
        {
            source.Subscribe(Send);
        }

        public void BindQueue(string queueName, string exchangeName, params string[] routingKeys)
        {
            _endpointManager.BindQueue(exchangeName, queueName, routingKeys);
        }

        public bool ClearQueue(string queueName)
        {
            using (var proxy = _channelFactory.GetProxyForQueue(queueName))
            {
                var purged = proxy
                    .Channel
                .QueuePurge(queueName, true);
                return purged > 0;
            }
        }

        public void DefineRouteFor<T>(Action<IDefineRoute<T>> routeDefinition)
        {
            _routeManager.AddRoute(routeDefinition);
        }

        public void DestroyExchange(string exchangeName)
        {
            using(var proxy = _channelFactory.GetProxyForExchange(exchangeName))
            {
                proxy
                    .Channel
                    .ExchangeDelete(exchangeName, false, true);
            }
        }

        public void DestroyQueue(string queueName)
        {
            using (var proxy = _channelFactory.GetProxyForQueue(queueName))
            {
                proxy
                    .Channel
                    .QueueDelete(queueName, false, false, true);
            }
        }

        public void Send<T>(T body)
            where T : class
        {
            if(body != default(T))
            {
                _routeManager
                    .GetRoutesForMessage(body)
                    .ForEach(x =>
                                 {
                                     if(string.IsNullOrEmpty(x.Item2))
                                     {
                                         Send(x.Item1, body);
                                     }
                                     else
                                     {
                                         Send(x.Item1, body, x.Item2);
                                     }
                                 });
            }
        }
        
        public void Send<T>(string exchangeName, T body)
            where T : class
        {
            if(body != default(T))
                Send(exchangeName, body, "");
        }

        public void Send<T>(string exchangeName, T body, string routingKey) where T : class
        {
            if (body != default(T))
            {
                var correlated = body as ICorrelate;
                var correlationId = correlated == null ? null : correlated.CorrelationId;

                using (var proxy = correlationId == null 
                    ? _channelFactory.GetProxyForExchange(exchangeName)
                    : _channelFactory.GetProxyForExchange(exchangeName, correlationId))
                {
                    proxy.Send(body, routingKey);
                }
            }
        }

        public void Subscribe(string queueName)
        {
            _subscriptionManager.AddSubscription(queueName).Start();
        }

        public void Unsubscribe(string queueName)
        {
            _subscriptionManager.StopSubscription(queueName);
        }

        protected void CloseConnection()
        {

        }

        public Bus(
            IChannelProxyFactory channelFactory, 
            IEndpointManager endpointManager, 
            ISubscriptionManager subscriptionManager,
            IRouteManager routeManager)
        {
            _channelFactory = channelFactory;
            _endpointManager = endpointManager;
            _subscriptionManager = subscriptionManager;
            _routeManager = routeManager;
        }

        public void Dispose()
        {

        }
    }
}