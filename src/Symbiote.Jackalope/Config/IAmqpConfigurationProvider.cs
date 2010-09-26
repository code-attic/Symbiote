using System.Collections.Generic;

namespace Symbiote.Jackalope.Config
{
    public interface IAmqpConfigurationProvider
    {
        IDictionary<string, IBroker> Brokers { get; set; }
    }
}