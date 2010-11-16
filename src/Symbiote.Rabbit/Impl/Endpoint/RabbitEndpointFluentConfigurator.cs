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
    public class RabbitEndpointFluentConfigurator
    {
        public RabbitEndpoint Endpoint { get; set; }
        public bool Subscribe { get; set; }

        public RabbitEndpointFluentConfigurator AutoDelete()
        {
            Endpoint.AutoDelete = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator Broker(string broker)
        {
            Endpoint.Broker = broker;
            return this;
        }

        public RabbitEndpointFluentConfigurator CreateResponseChannel()
        {
            Endpoint.NeedsResponseChannel = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator Durable()
        {
            Endpoint.Durable = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator ExchangeName(string name)
        {
            Endpoint.ExchangeName = name;
            return this;
        }

        public RabbitEndpointFluentConfigurator Exclusive()
        {
            Endpoint.Exclusive = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator NoWait()
        {
            Endpoint.NoWait = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator NoAck()
        {
            Endpoint.NoAck = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator QueueName(string queueName)
        {
            Endpoint.QueueName = queueName;
            return this;
        }

        public RabbitEndpointFluentConfigurator RoutingKeys(params string[] routingKeys)
        {
            Endpoint.RoutingKeys = new List<string>(routingKeys);
            return this;
        }

        public RabbitEndpointFluentConfigurator Passive()
        {
            Endpoint.Passive = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator PersistentDelivery()
        {
            Endpoint.PersistentDelivery = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator StartSubscription()
        {
            Subscribe = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator UseTransactions()
        {
            Endpoint.UseTransactions = true;
            return this;
        }

        public RabbitEndpointFluentConfigurator()
        {
            Endpoint = new RabbitEndpoint();
        }
    }

    
}