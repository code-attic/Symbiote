using System;
using System.Net;
using Symbiote.Core;

namespace Symbiote.Net
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
                                      var config = configurator.GetConfiguration();
                                      x.For<IHttpServerConfiguration>().Use(config);
                                      x.For<IHttpServer>().Use<HttpServer>();
                                      x.For<IAuthenticationValidator>().Use<WorthlessAuthenticationValidator>();
                                      
                                      if(config.AuthSchemes == AuthenticationSchemes.Basic)
                                      {
                                          x.For<IHttpAuthChallenger>().Use<HttpBasicAuthChallenger>();
                                      }
                                  });

            return assimilate;
        }
    }
}
