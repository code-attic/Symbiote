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