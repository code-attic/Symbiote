using System.Collections;
using System.Collections.Generic;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;
using Symbiote.Relax;

namespace Symbiote.Telepathy
{
    public class JackalopeEndpointConfiguration : DefaultCouchDocument, IAmqpEndpointConfiguration
    {
        public string QueueName { get; set; }
        public List<string> RoutingKeys { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public string ExchangeName { get; set; }
        public string ExchangeTypeName { get; private set; }
        public bool Durable { get; set; }
        public bool Passive { get; set; }
        public bool AutoDelete { get; set; }
        public bool Internal { get; set; }
        public bool NoWait { get; set; }
        public bool NoAck { get; set; }
        public IDictionary Arguments { get; set; }
        public bool Exclusive { get; set; }
        public bool PersistentDelivery { get; set; }
    }
}