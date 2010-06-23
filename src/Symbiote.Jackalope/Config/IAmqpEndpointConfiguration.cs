using System.Collections;
using System.Collections.Generic;

namespace Symbiote.Jackalope.Config
{
    public interface IAmqpEndpointConfiguration
    {
        string QueueName { get; set; }
        List<string> RoutingKeys { get; set; }
        ExchangeType ExchangeType { get; set; }
        string ExchangeName { get; set; }
        string ExchangeTypeName { get; }
        bool Durable { get; set; }
        bool Passive { get; set; }
        bool AutoDelete { get; set; }
        bool Internal { get; set; }
        bool NoWait { get; set; }
        bool NoAck { get; set; }
        IDictionary Arguments { get; set; }
        bool Exclusive { get; set; }
        bool PersistentDelivery { get; set; }
    }
}