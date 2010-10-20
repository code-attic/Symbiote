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

using System;
using System.Collections.Concurrent;
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;
using Symbiote.Jackalope.Impl.Channel;
using Symbiote.Jackalope.Impl.Dispatch;
using Symbiote.Jackalope.Impl.Endpoint;
using Symbiote.Jackalope.Impl.Router;
using Symbiote.Jackalope.Impl.Routes;
using Symbiote.Jackalope.Impl.Subscriptions;

namespace Symbiote.Jackalope.Impl
{
    public class Bus : 
        IBus,
        IQueueStreamCollection
    {
        private IChannelProxyFactory _channelFactory;
        private ISubscriptionManager _subscriptionManager;
        private ConcurrentDictionary<string, IChannelProxy> SendProxies { get; set; }
        private IEndpointManager _endpointManager;
        private IRouteManager _routeManager;
        private static string tempId = Guid.NewGuid().ToString();

        public IQueueStreamCollection QueueStreams { get { return this; } }

        public IObservable<Envelope> this[string queueName]
        {
            get
            {
                var endpoint = _endpointManager.GetEndpointByQueue(queueName);
                var effectiveQueue = queueName;
                if (endpoint.EndpointConfiguration.LoadBalance)
                {
                    effectiveQueue = "{0}.{1}".AsFormat(queueName, GetClientId());
                    _subscriptionManager.EnsureSubscriptionIsRunning(queueName);    
                }

                return Observable.Merge(_subscriptionManager
                                     .EnsureSubscriptionIsRunning(queueName)
                                     .Select(x => x.MessageStream));
            }
        }

        public IAmqpEndpointConfiguration AddEndPoint(Action<IEndPoint> endpointConfiguration)
        {
            var endpoint = new BusEndPoint();
            endpointConfiguration(endpoint);
            _endpointManager.AddEndpoint(endpoint);
            return endpoint.EndpointConfiguration;
        }

        public void AutoRouteFromSource<T>(IObservable<T> source)
            where T : class
        {
            source.Subscribe(Send);
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

        public void DefineRouteFor<T>(Action<IDefineRoute<T>> routeDefinition)
        {
            _routeManager.AddRoute(routeDefinition);
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

        public void Send<T>(T body)
            where T : class
        {
            if(body != default(T))
            {
                var routesForMessage = _routeManager
                    .GetRoutesForMessage(body);

                if (routesForMessage.Count() == 0)
                    throw new NoRouteDefinedException("No route has been defined for message type {0}.".AsFormat(typeof(T)));

                routesForMessage
                    .ForEach(x =>
                                 {
                                     if(string.IsNullOrEmpty(x.Item2))
                                     {
                                         Send(x.Item1, body);
                                     }
                                     else
                                     {
                                         Send(x.Item1, body, x.Item2);
                                     }
                                 });
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
                var correlated = body as ICorrelate;
                var correlationId = correlated == null ? null : correlated.CorrelationId;

                using (var proxy = correlationId == null 
                    ? _channelFactory.GetProxyForExchange(exchangeName)
                    : _channelFactory.GetProxyForExchange(exchangeName, correlationId))
                {
                    proxy.Send(body, routingKey);
                }
            }
        }

        public void Subscribe(string queueName)
        {
            var endpoint = _endpointManager.GetEndpointByQueue(queueName);
            var subscribeTo = queueName;
            if(endpoint.EndpointConfiguration.LoadBalance)
            {
                subscribeTo = LoadBalanceSubscription(queueName);
            }
            DirectSubscription(subscribeTo);
        }

        protected string LoadBalanceSubscription(string queueName)
        {
            var alias = "{0}.{1}".AsFormat(queueName, GetClientId());
            var endpoint = AddEndPoint(x => x
                .Exchange(alias, ExchangeType.direct)
                .QueueName(alias)
                .AutoDelete()
                .Immediate()
                .Exclusive()
                .Mandatory()
                .RoutingKeys(queueName));
            Send(new SubscriberOnline(endpoint, queueName, alias));
            return alias;
        }

        protected string GetClientId()
        {
            return tempId;
        }

        protected void DirectSubscription(string queueName)
        {
            _subscriptionManager.AddSubscription(queueName);
        }

        public void Unsubscribe(string queueName)
        {
            _subscriptionManager.StopSubscription(queueName);
        }

        protected void CloseConnection()
        {

        }

        public Bus(
            IChannelProxyFactory channelFactory, 
            IEndpointManager endpointManager, 
            ISubscriptionManager subscriptionManager,
            IRouteManager routeManager)
        {
            _channelFactory = channelFactory;
            _endpointManager = endpointManager;
            _subscriptionManager = subscriptionManager;
            _routeManager = routeManager;
        }

        public void Dispose()
        {

        }
    }
}