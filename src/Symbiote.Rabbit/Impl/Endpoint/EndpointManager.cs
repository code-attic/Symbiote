// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using Symbiote.Rabbit.Impl.Server;
using Symbiote.Rabbit.Impl.Subscription;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class EndpointManager : IEndpointManager
    {
        protected IEndpointIndex EndpointIndex { get; set; }
        protected IConnectionManager ConnectionManager { get; set; }
        protected QueueSubscriptionFactory SubscriptionFactory { get; set; }

        public void ConfigureEndpoint( Action<EndpointConfigurator> configurate )
        {
            var configurator = new EndpointConfigurator();
            configurate( configurator );
            var endpoint = configurator.RabbitEndpoint;
            AddEndpoint( endpoint );

            if ( !string.IsNullOrEmpty( endpoint.QueueName ) )
            {
                SubscriptionFactory.CreateSubscription( endpoint, configurator.Subscribe );
            }
        }

        public void AddEndpoint( RabbitEndpoint endpoint )
        {
            endpoint.CreateOnBroker( ConnectionManager );
            EndpointIndex.AddEndpoint( endpoint );
        }

        public EndpointManager(
            IEndpointIndex endpointIndex,
            IConnectionManager connectionManager,
            QueueSubscriptionFactory subscriptionFactory )
        {
            ConnectionManager = connectionManager;
            EndpointIndex = endpointIndex;
            SubscriptionFactory = subscriptionFactory;
        }
    }
}