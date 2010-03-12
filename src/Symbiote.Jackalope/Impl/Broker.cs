using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{

    public class Broker : 
        IBroker, 
        IDisposable
    {
        private IMessageSerializer _messageSerializer;
        private IChannelProxyFactory _proxyFactory;
        private string _queueName;
        private static ConcurrentDictionary<Type, List<IDispatch>> _dispatchers = new ConcurrentDictionary<Type, List<IDispatch>>();

        protected bool Run { get; set; }

        protected IMessageSerializer Serializer
        {
            get
            {
                _messageSerializer = _messageSerializer ?? ObjectFactory.GetInstance<IMessageSerializer>();
                return _messageSerializer;
            }
        }

        public void Start(string queueName)
        {
            _queueName = queueName;
            Run = true;
            var tasks = new List<Task>();
            var threshold = 50;
            while (Run)
            {
                while(tasks.Count < threshold)
                {
                    var task = Task
                        .Factory
                        .StartNew(Dispatch);
                    tasks.Add(task);
                }

                tasks.RemoveAll(x => x.Status == TaskStatus.RanToCompletion);
            }
        }

        protected void Dispatch()
        {
            IChannelProxy proxy = null;
            BasicDeliverEventArgs result = null;
            QueueingBasicConsumer consumer = null;
            try
            {
                proxy = _proxyFactory.GetProxyForQueue(_queueName);
                consumer = proxy.GetConsumer();
                result = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                if(result != null)
                {
                    var payload = Serializer.Deserialize(result.Body);
                    var dispatchers = GetDispatchers(payload);
                    dispatchers.AsParallel().ForEach(x => x.Dispatch(payload, proxy, result));
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

        private List<IDispatch> GetDispatchers(object payload)
        {
            var payloadType = payload.GetType();
            List<IDispatch> dispatchers = null;
            if(!_dispatchers.TryGetValue(payloadType, out dispatchers))
            {
                dispatchers = ObjectFactory.GetAllInstances<IDispatch>().Where(x => x.CanHandle(payload)).ToList();
                _dispatchers.TryAdd(payloadType,
                                    dispatchers);
            }
            return dispatchers;
        }

        public void Stop()
        {
            Run = false;
            Console.WriteLine("Killing broker...");
        }

        public Broker(IChannelProxyFactory proxyFactory)
        {
            _proxyFactory = proxyFactory;
        }

        public void Dispose()
        {
            _dispatchers.Clear();
            Stop();
        }
    }
}
