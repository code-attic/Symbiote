using System;
using Symbiote.Core;
using Symbiote.JsonRpc.Config;
using Symbiote.JsonRpc.Impl;

namespace Symbiote.JsonRpc
{
    public static class HttpServiceHostAssimilation
    {
        public static IAssimilate HttpServiceHost(this IAssimilate assimilate, Action<HttpServiceHostConfigurator> configure)
        {
            var configurator = new HttpServiceHostConfigurator(new HttpServiceHostConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IHttpServiceHostConfiguration>().Use(configurator.GetConfiguration());
                                      if(configurator.GetConfiguration().SelfHosted)
                                          x.For<IHttpServiceHost>().Use<SimpleHttpServiceHost>();
                                  });

            return assimilate;
        }
    }
}
