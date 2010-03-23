using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StructureMap;
using Symbiote.Core.Extensions;
using Enumerable = System.Linq.Enumerable;
using IEnumerableExtenders = Symbiote.Core.Extensions.IEnumerableExtenders;

namespace Symbiote.Jackalope.Impl
{
    public abstract class BaseBroker :
        IBroker,
        IDisposable
    {
        private IMessageSerializer _messageSerializer;
        private IChannelProxyFactory _proxyFactory;
        private static ConcurrentDictionary<Type, List<IDispatch>> _dispatchers = new ConcurrentDictionary<Type, List<IDispatch>>();
        protected string _queueName;

        protected IMessageSerializer Serializer { get { return _messageSerializer; } }
        protected IChannelProxyFactory ProxyFactory { get { return _proxyFactory; } }

        public abstract void Start(string queueName);
        public abstract void Stop();

        protected virtual void Dispatch()
        {
            IChannelProxy proxy = null;
            BasicDeliverEventArgs result = null;
            QueueingBasicConsumer consumer = null;
            try
            {
                proxy = ProxyFactory.GetProxyForQueue(_queueName);
                consumer = proxy.GetConsumer();
                result = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                if (result != null)
                {
                    var payload = Serializer.Deserialize(result.Body);
                    var dispatchers = GetDispatchersForPayload(payload);
                    IEnumerableExtenders.ForEach<IDispatch>(dispatchers.AsParallel(), x => x.Dispatch(payload, proxy, result));
                }
            }
            catch (Exception e)
            {
                "An exception occurred while attempting to dispatch a message from the exchange named {0} with routing key {1} \r\n\t {2}"
                    .ToError<IBus>(result.Exchange, result.RoutingKey, e);
                proxy.Reject(result.DeliveryTag, true);
            }
            finally
            {
                proxy.Dispose();
            }
        }

        protected virtual List<IDispatch> GetDispatchersForPayload(object payload)
        {
            var payloadType = payload.GetType();
            List<IDispatch> dispatchers = null;
            if (!_dispatchers.TryGetValue(payloadType, out dispatchers))
            {
                dispatchers = GetAllDispatchers(payload);
                _dispatchers.TryAdd(payloadType, dispatchers);
            }
            return dispatchers;
        }

        protected virtual List<IDispatch> GetAllDispatchers(object payload)
        {
            return ObjectFactory.GetAllInstances<IDispatch>().Where(x => x.CanHandle(payload)).ToList();
        }

        protected BaseBroker(IChannelProxyFactory proxyFactory, IMessageSerializer messageSerializer)
        {
            _proxyFactory = proxyFactory;
            _messageSerializer = messageSerializer;
        }

        public void Dispose()
        {
            _messageSerializer = null;
            _proxyFactory = null;
            Stop();
        }
    }
}