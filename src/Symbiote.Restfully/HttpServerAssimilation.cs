using System;
using System.Web.Hosting;
using Symbiote.Core;

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
                                      x.For<IHttpServer>().Use<HttpServer>();
                                  });

            return assimilate;
        }
    }
}
