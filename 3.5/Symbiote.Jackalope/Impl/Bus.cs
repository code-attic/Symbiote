using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Impl;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;
using StructureMap;

namespace Symbiote.Jackalope.Impl
{
    public class Bus : IBus
    {
        private IChannelProxyFactory _channelFactory;
        private ISubscriptionManager _subscriptionManager;
        private Dictionary<Type, IDispatch> _functionalDispatchers 
            = new Dictionary<Type, IDispatch>();
        private IEndpointManager _endpointManager;
        private ReaderWriterLockSlim _functionalDispatcherLock = new ReaderWriterLockSlim();

        public void AddEndPoint(Action<IEndPoint> endpointConfiguration)
        {
            var endpoint = new BusEndPoint();
            endpointConfiguration(endpoint);
            _endpointManager.AddEndpoint(endpoint);
        }

        public IBus AddProcessor<TMessage>(Func<TMessage, IMessageDelivery, object> processor) where TMessage : class
        {
            var actionDispatcher = new ActionDispatcher<TMessage>(processor);
            _functionalDispatcherLock.EnterWriteLock();
            _functionalDispatchers[typeof(TMessage)] = actionDispatcher;
            _functionalDispatcherLock.ExitWriteLock();
            return this;
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
        
        public Envelope GetNextMessage(string queueName)
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

        public Envelope GetNextMessage(string queueName, int miliseconds)
        {
            var proxy = _channelFactory.GetProxyForQueue(queueName);
            try
            {
                return proxy.GetNext(miliseconds);
            }
            catch (Exception e)
            {
                "An exception occurred while attempting to retrieve a message from queue {0}: \r\n\t {1}"
                    .ToError<IBus>(queueName, e);
                throw;
            }
        }

        public object ProcessNextMessage(string queueName)
        {
            var proxy = _channelFactory.GetProxyForQueue(queueName);
            try
            {
                using(var envelope = proxy.GetNext())
                {
                    _functionalDispatcherLock.EnterReadLock();
                    var dispatcher = _functionalDispatchers[envelope.Message.GetType()];
                    _functionalDispatcherLock.ExitReadLock();
                    var result = dispatcher.Dispatch(envelope);
                    envelope.MessageDelivery.Acknowledge();
                    return result;
                }
            }
            catch (Exception e)
            {
                "An exception occurred while attempting to retrieve a message from queue {0}: \r\n\t {1}"
                    .ToError<IBus>(queueName, e);
                throw;
            }
        }

        public object ProcessNextMessage(string queueName, int miliseconds)
        {
            var proxy = _channelFactory.GetProxyForQueue(queueName);
            try
            {
                using (var envelope = proxy.GetNext(miliseconds))
                {
                    _functionalDispatcherLock.EnterReadLock();
                    var dispatcher = _functionalDispatchers[envelope.Message.GetType()];
                    _functionalDispatcherLock.ExitReadLock();
                    var result = dispatcher.Dispatch(envelope);
                    envelope.MessageDelivery.Acknowledge();
                    return result;
                }
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