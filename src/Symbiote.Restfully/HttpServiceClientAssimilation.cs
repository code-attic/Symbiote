using System;
using Symbiote.Core;
using Symbiote.JsonRpc.Config;

namespace Symbiote.JsonRpc
{
    public static class HttpServiceClientAssimilation
    {
        public static IAssimilate HttpServiceClient(this IAssimilate assimilate, Action<HttpServiceClientConfigurator> configure)
        {
            var configurator = new HttpServiceClientConfigurator(new HttpServiceClientConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IHttpServiceClientConfiguration>().Use(configurator.GetConfiguration());
                    
                                  });

            return assimilate;
        }
    }
}