using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class Broker : BaseBroker
    {
        protected bool Run { get; set; }

        public override void Start(string queueName)
        {
            _queueName = queueName;
            Run = true;
            var threshold = 5;
            while (Run)
            {
                Dispatch();
            }
        }

        protected override void Dispatch()
        {
            IChannelProxy proxy = null;
            BasicDeliverEventArgs result = null;
            QueueingBasicConsumer consumer = null;
            try
            {
                proxy = proxyFactory.GetProxyForQueue(_queueName);
                consumer = proxy.GetConsumer();
                result = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                if (result != null)
                {
                    var payload = messageSerialzier.Deserialize(result.Body);
                    var dispatchers = GetDispatchersForPayload(payload);
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

        public override void Stop()
        {
            Run = false;
            Console.WriteLine("Killing broker...");
        }

        public Broker(IChannelProxyFactory proxyFactory, IMessageSerializer messageSerializer) : base(proxyFactory, messageSerializer)
        {
        }
    }
}
