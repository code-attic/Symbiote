using System;
using System.Web.Hosting;
using Symbiote.Core;
using Symbiote.Restfully.Impl;

namespace Symbiote.Restfully
{
    public static class HttpServerAssimilation
    {
        public static IAssimilate HttpServer(this IAssimilate assimilate, Action<HttpServerConfigurator> configure)
        {
            var configurator = new HttpServerConfigurator(new HttpServerConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IHttpServerConfiguration>().Use(configurator.GetConfiguration());
                                      if(configurator.GetConfiguration().SelfHosted)
                                          x.For<IHttpServer>().Use<HttpServer>();
                                  });

            return assimilate;
        }
    }
}
