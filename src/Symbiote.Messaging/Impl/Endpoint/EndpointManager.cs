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
using Symbiote.Messaging.Impl.Subscriptions;

namespace Symbiote.Messaging.Impl.Endpoint
{
    public class EndpointManager : IEndpointManager
    {
        protected IEndpointIndex EndpointIndex { get; set; }
        protected NamedPipeSubscriptionFactory SubscriptionFactory { get; set; }

        public void ConfigureEndpoint( Action<EndpointConfigurator> configurate )
        {
            var configurator = new EndpointConfigurator();
            configurate( configurator );
            var endpoint = configurator.NamedPipeEndpoint;
            AddEndpoint( endpoint );

            if ( !string.IsNullOrEmpty( endpoint.PipeName ) )
            {
                SubscriptionFactory.CreateSubscription( endpoint, configurator.Subscribe );
            }
        }

        public void AddEndpoint( NamedPipeEndpoint endpoint )
        {
            EndpointIndex.AddEndpoint( endpoint );
        }

        public EndpointManager(
            IEndpointIndex endpointIndex,
            NamedPipeSubscriptionFactory subscriptionFactory )
        {
            EndpointIndex = endpointIndex;
            SubscriptionFactory = subscriptionFactory;
        }
    }
}