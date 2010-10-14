using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class QueueSubscription :
        ISubscription
    {
        protected IChannelProxyFactory ProxyFactory { get; set; }
        protected IChannelProxy CurrentProxy { get; set; }
        protected RabbitQueueListener Listener { get; set; }
        protected IDispatcher Dispatcher { get; set; }
        public string Name { get; set; }
        public bool Started { get; private set; }
        public bool Starting { get; private set; }
        public bool Stopped { get; private set; }
        public bool Stopping { get; private set; }

        public void Dispose()
        {
            CurrentProxy.Dispose();
            Listener = null;
            ProxyFactory = null;
        }

        public void Start()
        {
            CurrentProxy = ProxyFactory.GetProxyForQueue(Name);
            Listener = new RabbitQueueListener(CurrentProxy, Dispatcher);
        }

        
        public void Stop()
        {
            CurrentProxy.Dispose();
            Listener = null;
        }

        public QueueSubscription(IChannelProxyFactory proxyFactory, IDispatcher dispatcher)
        {
            ProxyFactory = proxyFactory;
            Dispatcher = dispatcher;
        }
    }
}
