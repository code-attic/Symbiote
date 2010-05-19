using System;
using Symbiote.Core;

namespace Net.Tests
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
                                  });

            return assimilate;
        }
    }
}
