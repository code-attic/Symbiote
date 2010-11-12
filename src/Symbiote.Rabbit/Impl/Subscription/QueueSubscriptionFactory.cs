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
using Symbiote.Rabbit.Impl.Adapter;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Subscription
{
    public class QueueSubscriptionFactory
    {
        protected ISubscriptionManager Subscriptions { get; set; }
        protected IDispatcher Dispatcher { get; set; }
        protected IChannelProxyFactory ProxyFactory { get; set; }
        
        public void CreateSubscription(RabbitChannelDefinition definition, bool start)
        {
            var queueSubscription = new QueueSubscription(ProxyFactory, Dispatcher, definition);
            queueSubscription.Name = definition.Queue;
            if (start)
                Subscriptions.AddAndStartSubscription(queueSubscription);
            else
                Subscriptions.AddSubscription(queueSubscription);
        }

        public void CreateSubscription<TMessage>(RabbitChannelDefinition<TMessage> definition, bool start)
        {
            var queueSubscription = new QueueSubscription<TMessage>(ProxyFactory, Dispatcher, definition);
            queueSubscription.Name = definition.Queue;
            if (start)
                Subscriptions.AddAndStartSubscription(queueSubscription);
            else
                Subscriptions.AddSubscription(queueSubscription);
        }

        public QueueSubscriptionFactory( ISubscriptionManager subscriptions, IDispatcher dispatcher, IChannelProxyFactory proxyFactory )
        {
            Subscriptions = subscriptions;
            Dispatcher = dispatcher;
            ProxyFactory = proxyFactory;
        }
    }
}
