using System.Collections.Generic;

namespace Symbiote.Jackalope.Config
{
    public interface IAmqpConfigurationProvider
    {
        IList<IAmqpServerConfiguration> Servers { get; set; }
    }
}