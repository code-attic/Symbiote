using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public ExchangeType ExchangeType { get; set; }
        public string ExchangeTypeName { get { return ExchangeType.ToString(); } }
        public bool Internal { get; set; }
        public bool ImmediateDelivery { get; set; }
        public bool LoadBalance { get; set; }
        public bool NeedsResponseChannel { get; set; }
        public bool NoAck { get; set; }
        public bool NoWait { get; set; }
        public bool MandatoryDelivery { get; set; }
        public bool Passive { get; set; }
        public bool PersistentDelivery { get; set; }
        public string QueueName { get; set; }
        public List<string> RoutingKeys { get; set; }
        public bool UseTransactions { get; set; }

        public RabbitEndpoint()
        {
            Broker = "default";
            ExchangeName = "";
            RoutingKeys = new List<string>();
            NoWait = false;
        }
    }
}
