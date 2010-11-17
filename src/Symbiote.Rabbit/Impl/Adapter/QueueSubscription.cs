/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class QueueSubscription :
        ISubscription
    {
        protected IChannelProxyFactory ProxyFactory { get; set; }
        protected IChannelProxy CurrentProxy { get; set; }
        protected RabbitQueueListener Listener { get; set; }
        protected RabbitEndpoint Endpoint { get; set; }
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
            CurrentProxy = ProxyFactory.GetProxyForQueue(Endpoint);
            Listener = new RabbitQueueListener(CurrentProxy, Dispatcher, Endpoint);
        }

        public void Stop()
        {
            CurrentProxy.Dispose();
            Listener = null;
        }

        public QueueSubscription(
            IChannelProxyFactory proxyFactory,
            IDispatcher dispatcher,
            RabbitEndpoint endpoint)
        {
            ProxyFactory = proxyFactory;
            Dispatcher = dispatcher;
            Endpoint = endpoint;
        }
    }

    public class QueueSubscription<TMessage> :
        ISubscription
    {
        protected IChannelProxyFactory ProxyFactory { get; set; }
        protected IChannelProxy CurrentProxy { get; set; }
        protected RabbitQueueListener<TMessage> Listener { get; set; }
        protected RabbitEndpoint Endpoint { get; set; }
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
            CurrentProxy = ProxyFactory.GetProxyForQueue(Endpoint);
            Listener = new RabbitQueueListener<TMessage>(CurrentProxy, Dispatcher, Endpoint);
        }

        public void Stop()
        {
            CurrentProxy.Dispose();
            Listener = null;
        }

        public QueueSubscription(
            IChannelProxyFactory proxyFactory, 
            IDispatcher dispatcher,
            RabbitEndpoint endpoint)
        {
            ProxyFactory = proxyFactory;
            Dispatcher = dispatcher;
            Endpoint = endpoint;
        }
    }
}
