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

namespace Symbiote.Qpid.Impl.Endpoint
{
    public class QpidEndpointFluentConfigurator
    {
        public QpidEndpoint Endpoint { get; set; }

        public QpidEndpointFluentConfigurator Broker(string broker)
        {
            Endpoint.Broker = broker;
            return this;
        }

        public QpidEndpointFluentConfigurator QueueName(string queueName)
        {
            Endpoint.QueueName = queueName;
            return this;
        }

        public QpidEndpointFluentConfigurator RoutingKeys(params string[] routingKeys)
        {
            Endpoint.RoutingKeys = new List<string>(routingKeys);
            return this;
        }

        public QpidEndpointFluentConfigurator Direct(string exchangeName)
        {
            Endpoint.ExchangeName = exchangeName;
            Endpoint.ExchangeType = ExchangeType.direct;
            return this;
        }

        public QpidEndpointFluentConfigurator Fanout(string exchangeName)
        {
            Endpoint.ExchangeName = exchangeName;
            Endpoint.ExchangeType = ExchangeType.fanout;
            return this;
        }

        public QpidEndpointFluentConfigurator Topic(string exchangeName)
        {
            Endpoint.ExchangeName = exchangeName;
            Endpoint.ExchangeType = ExchangeType.topic;
            return this;
        }

        public QpidEndpointFluentConfigurator Durable()
        {
            Endpoint.Durable = true;
            return this;
        }

        public QpidEndpointFluentConfigurator Exclusive()
        {
            Endpoint.Exclusive = true;
            return this;
        }

        public QpidEndpointFluentConfigurator Passive()
        {
            Endpoint.Passive = true;
            return this;
        }

        public QpidEndpointFluentConfigurator AutoDelete()
        {
            Endpoint.AutoDelete = true;
            return this;
        }

        public QpidEndpointFluentConfigurator Immediate()
        {
            Endpoint.ImmediateDelivery = true;
            return this;
        }

        public QpidEndpointFluentConfigurator Internal()
        {
            Endpoint.Internal = true;
            return this;
        }

        public QpidEndpointFluentConfigurator LoadBalanced()
        {
            Endpoint.LoadBalance = true;
            return this;
        }

        public QpidEndpointFluentConfigurator Mandatory()
        {
            Endpoint.MandatoryDelivery = true;
            return this;
        }

        public QpidEndpointFluentConfigurator NoWait()
        {
            Endpoint.NoWait = true;
            return this;
        }

        public QpidEndpointFluentConfigurator NoAck()
        {
            Endpoint.NoAck = true;
            return this;
        }

        public QpidEndpointFluentConfigurator PersistentDelivery()
        {
            Endpoint.PersistentDelivery = true;
            return this;
        }

        public QpidEndpointFluentConfigurator()
        {
            Endpoint = new QpidEndpoint();
        }
    }
}