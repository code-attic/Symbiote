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
using StructureMap;
using Symbiote.Jackalope.Impl.Channel;
using Symbiote.Jackalope.Impl.Dispatch;
using Symbiote.Jackalope.Impl.Endpoint;
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

        public IQueueStreamCollection QueueMessageStreams { get { return _subscriptionManager; } }

        public void AddEndPoint(Action<IEndPoint> endpointConfiguration)
        {
            var endpoint = new BusEndPoint();
            endpointConfiguration(endpoint);
            _endpointManager.AddEndpoint(endpoint);
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
                using (var proxy = _channelFactory.GetProxyForExchange(exchangeName))
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

        public Bus(IChannelProxyFactory channelFactory, IEndpointManager endpointManager, ISubscriptionManager subscriptionManager)
        {
            _channelFactory = channelFactory;
            _endpointManager = endpointManager;
            _subscriptionManager = subscriptionManager;
        }

        public void Dispose()
        {

        }
    }
}