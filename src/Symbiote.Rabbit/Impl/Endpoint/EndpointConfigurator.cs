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

using System.Collections.Generic;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class EndpointConfigurator
    {
        public RabbitEndpoint RabbitEndpoint { get; set; }
        public bool Subscribe { get; set; }

        public EndpointConfigurator AutoDelete()
        {
            RabbitEndpoint.AutoDelete = true;
            return this;
        }

        public EndpointConfigurator Broker(string broker)
        {
            RabbitEndpoint.Broker = broker;
            return this;
        }

        public EndpointConfigurator CreateResponseChannel()
        {
            RabbitEndpoint.NeedsResponseChannel = true;
            return this;
        }

        public EndpointConfigurator Durable()
        {
            RabbitEndpoint.Durable = true;
            return this;
        }

        public EndpointConfigurator ExchangeName(string name)
        {
            RabbitEndpoint.ExchangeName = name;
            return this;
        }

        public EndpointConfigurator Exclusive()
        {
            RabbitEndpoint.Exclusive = true;
            return this;
        }

        public EndpointConfigurator NoWait()
        {
            RabbitEndpoint.NoWait = true;
            return this;
        }

        public EndpointConfigurator NoAck()
        {
            RabbitEndpoint.NoAck = true;
            return this;
        }

        public EndpointConfigurator QueueName(string queueName)
        {
            RabbitEndpoint.QueueName = queueName;
            return this;
        }

        public EndpointConfigurator RoutingKeys(params string[] routingKeys)
        {
            RabbitEndpoint.RoutingKeys = new List<string>(routingKeys);
            return this;
        }

        public EndpointConfigurator Passive()
        {
            RabbitEndpoint.Passive = true;
            return this;
        }

        public EndpointConfigurator SerializeBy<TSerializer>()
        {
            RabbitEndpoint.SerializerType = typeof(TSerializer);
            return this;
        }

        public EndpointConfigurator StartSubscription()
        {
            Subscribe = true;
            return this;
        }

        public EndpointConfigurator UseTransactions()
        {
            RabbitEndpoint.Transactional = true;
            return this;
        }

        public EndpointConfigurator()
        {
            RabbitEndpoint = new RabbitEndpoint();
        }
    }

    
}