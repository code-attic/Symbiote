using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;
using StructureMap;
using Symbiote.Jackalope.Control;

namespace Symbiote.Jackalope.Impl
{
    public class Bus : IBus
    {
        private IChannelProxyFactory _channelFactory;
        private IEndpointManager _endpointManager;
        private ISubscriptionManager _subscriptionManager;

        public bool ClearQueue(string queueName)
        {
            var purged = _channelFactory
                .GetProxyForQueue(queueName)
                .Channel
                .QueuePurge(queueName, true);
            return purged > 0;
        }

        public void DestroyExchange(string exchangeName)
        {
            _channelFactory
                .GetProxyForExchange(exchangeName)
                .Channel
                .ExchangeDelete(exchangeName, false, true);
        }

        public void DestroyQueue(string queueName)
        {
            _channelFactory
                .GetProxyForQueue(queueName)
                .Channel
                .QueueDelete(queueName, false, false, true);
        }

        public void AddEndPoint(Action<IEndPoint> endpointConfiguration)
        {
            var endpoint = new BusEndPoint();
            endpointConfiguration(endpoint);
            _endpointManager.AddEndpoint(endpoint);
        }

        public void Send<T>(string exchangeName, T body)
            where T : class
        {
            Send(exchangeName, body, "");
        }

        public void Send<T>(string exchangeName, T body, string routingKey) where T : class
        {
            using (var proxy = _channelFactory.GetProxyForExchange(exchangeName))
            {
                proxy.Send(body, routingKey);
            }
        }

        public void Subscribe(string queueName, AsyncCallback onBrokerDeath)
        {
            _subscriptionManager.AddSubscription(queueName, 1).Start();
        }

        public void Subscribe(string queueName, int brokers, AsyncCallback onBrokerDeath)            
        {
            _subscriptionManager.AddSubscription(queueName, brokers).Start();
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