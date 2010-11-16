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
using System.Collections;
using System.Collections.Generic;
using RabbitMQ.Client;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class RabbitEndpoint
    {
        public IDictionary Arguments { get; set; }
        public bool AutoDelete { get; set; }
        public string Broker { get; set; }
        public bool CreatedOnBroker { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public string ExchangeName { get; set; }
        public bool NeedsResponseChannel { get; set; }
        public bool NoAck { get; set; }
        public bool NoWait { get; set; }
        public bool Passive { get; set; }
        public bool PersistentDelivery { get; set; }
        public string QueueName { get; set; }
        public List<string> RoutingKeys { get; set; }
        public bool UseTransactions { get; set; }

        public void BindQueue(IModel channel)
        {
            if (RoutingKeys.Count == 0)
                RoutingKeys = new List<string>(new[] { "" });

            RoutingKeys
                .ForEach( x => channel.QueueBind( QueueName, ExchangeName, x, false, null ) );
        }

        public void BuildQueue(IModel channel)
        {
            channel.QueueDeclare(
                QueueName,
                Passive,
                Durable,
                Exclusive,
                AutoDelete,
                NoWait,
                Arguments);
        }

        public void CreateOnBroker(IConnectionManager manager)
        {
            if (!CreatedOnBroker)
            {
                var connection = manager.GetConnection(Broker);
                using (var channel = connection.CreateModel())
                {
                    BuildQueue(channel);
                    BindQueue(channel);
                    CreatedOnBroker = true;
                }
            }
        }

        public RabbitEndpoint()
        {
            Broker = "default";
            ExchangeName = "";
            RoutingKeys = new List<string>();
        }
    }
}
