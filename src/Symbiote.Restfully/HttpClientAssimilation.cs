using System;
using Symbiote.Core;
using Symbiote.Restfully.Impl;

namespace Symbiote.Restfully
{
    public static class HttpClientAssimilation
    {
        public static IAssimilate HttpClient(this IAssimilate assimilate, Action<HttpClientConfigurator> configure)
        {
            var configurator = new HttpClientConfigurator(new HttpClientConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IHttpClientConfiguration>().Use(configurator.GetConfiguration());
                    
                                  });

            return assimilate;
        }
    }
}