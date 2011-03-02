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

using System.Collections;
using System.Collections.Generic;

namespace Symbiote.Jackalope.Config
{
    public class AmqpEndpointConfiguration : IAmqpEndpointConfiguration
    {
        public string QueueName { get; set; }
        public List<string> RoutingKeys { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public string Broker { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeTypeName { get { return ExchangeType.ToString(); } }
        public bool Durable { get; set; }
        public bool Passive { get; set; }
        public bool AutoDelete { get; set; }
        public bool Internal { get; set; }
        public bool NoWait { get; set; }
        public bool NoAck { get; set; }
        public IDictionary Arguments { get; set; }
        public bool Exclusive { get; set; }
        public bool PersistentDelivery { get; set; }
        public bool ImmediateDelivery { get; set; }
        public bool LoadBalance { get; set; }
        public bool MandatoryDelivery { get; set; }

        public AmqpEndpointConfiguration()
        {
            Broker = "default";
            ExchangeName = "";
            RoutingKeys = new List<string>();
            NoWait = false;
        }
    }
}