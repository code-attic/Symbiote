using System;
using System.Web.Hosting;
using Symbiote.Core;
using Symbiote.Restfully.Config;
using Symbiote.Restfully.Impl;

namespace Symbiote.Restfully
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
