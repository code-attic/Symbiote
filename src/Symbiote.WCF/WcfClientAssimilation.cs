using System;
using Symbiote.Core;
using Symbiote.Wcf.Client;

namespace Symbiote.Wcf
{
    public static class WcfClientAssimilation
    {
        public static IAssimilate WcfClient(this IAssimilate assimilate, Action<WcfClientConfigurator> configure)
        {
            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IServiceMetadataCache>().Use<InMemoryMetadataCache>();
                                      x.For(typeof (IService<>)).Use(typeof (ServiceClient<>));
                                      x.For(typeof(IServiceClientConfigurationStrategy<>)).Use(typeof(DefaultConfigurationStrategy<>));
                                  });

            var wcfClientConfig = new WcfClientConfigurator();
            configure(wcfClientConfig);

            return assimilate;
        }
    }
}
