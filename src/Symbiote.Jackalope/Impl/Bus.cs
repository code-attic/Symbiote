using System;
using System.Collections;
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

        public void AddEndPoint(Action<IEndPoint> endpointConfiguration)
        {
            var endpoint = new BusEndPoint();
            endpointConfiguration(endpoint);
            _endpointManager.AddEndpoint(endpoint);
        }

        public void Get(string queueName, Action<object, IResponse> messageHandler)
        {
            using(var proxy = _channelFactory.GetProxyForQueue(queueName))
            {
                try
                {
                    var result = proxy.GetNext();
                    messageHandler(result.Item1, result.Item2);
                }
                catch (Exception e)
                {
                    "An exception occurred while attempting to retrieve a message from queue {0}: \r\n\t {1}"
                        .ToError<IBus>(queueName, e);
                    throw;
                }
            }
        }

        public Tuple<object, IResponse> Get(string queueName)
        {
            var proxy = _channelFactory.GetProxyForQueue(queueName);
            try
            {
                return proxy.GetNext();
            }
            catch (Exception e)
            {
                "An exception occurred while attempting to retrieve a message from queue {0}: \r\n\t {1}"
                    .ToError<IBus>(queueName, e);
                throw;
            }
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