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
using org.apache.qpid.transport;

namespace Symbiote.Qpid.Impl.Endpoint
{
    public class QpidEndpoint
    {
        public IDictionary Arguments { get; set; }
        public bool AutoDelete { get; set; }
        public string Broker { get; set; }
        public bool CreatedOnBroker { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public string ExchangeName { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public string ExchangeTypeName { get { return ExchangeType.ToString(); } }
        public bool Internal { get; set; }
        public bool ImmediateDelivery { get; set; }
        public bool LoadBalance { get; set; }
        public bool NoAck { get; set; }
        public bool NoWait { get; set; }
        public bool MandatoryDelivery { get; set; }
        public bool Passive { get; set; }
        public bool PersistentDelivery { get; set; }
        public string QueueName { get; set; }
        public List<string> RoutingKeys { get; set; }
        public Option[] Options
        {
            get
            {
                var optionList = new List<Option>();

                if (AutoDelete) optionList.Add(Option.AUTO_DELETE);
                if (Durable) optionList.Add(Option.DURABLE);
                if (Exclusive) optionList.Add(Option.EXCLUSIVE);
                //if (Internal) optionList.Add(Option.);
                if (ImmediateDelivery) optionList.Add(Option.IMMEDIATE);
                //if (NoAck) optionList.Add(Option.);
                //if (NoWait) optionList.Add(Option.);

                return optionList.ToArray();
            }
        }

        public QpidEndpoint()
        {
            Broker = "default";
            ExchangeName = "";
            RoutingKeys = new List<string>();
            NoWait = false;
        }
    }
}
