using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Wcf.Client;

namespace Symbiote.Wcf
{
    public class WcfDependencies : IDefineDependencies
    {
        public Action<DependencyConfigurator> Dependencies()
        {
            return container =>
                       {
                           container.For<IServiceMetadataCache>().Use<InMemoryMetadataCache>();
                           container.For( typeof( IService<> ) ).Use( typeof( ServiceClient<> ) );
                           container.For( typeof( IServiceClientConfigurationStrategy<> ) ).Use(
                               typeof( DefaultConfigurationStrategy<> ) );
                       };
        }
    }
}