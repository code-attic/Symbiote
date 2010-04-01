using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Relax.Impl;

namespace Symbiote.Relax
{
    public static class RelaxAssimilation
    {
        public static IAssimilate Relax(this IAssimilate assimilate, Action<CouchConfigurator> configure)
        {
            var config = new CouchConfigurator();
            configure(config);

            assimilate
                .Dependencies(c =>
                              {
                                  var configuration = config.GetConfiguration();
                                  c.For<ICouchConfiguration>().Use(configuration);
                                  c.For<ICouchCommand>().Use<CouchCommand>();
                                  c.For<ICouchCommandFactory>().Use<CouchCommandFactory>();
                                  c.For<ICouchCacheProvider>().Use<EideticCacheProvider>();
                                  c.For<ICacheKeyBuilder>().Use<CacheKeyBuilder>();
                                  if (configuration.Cache)
                                  {
                                      c.For(typeof (IDocumentRepository)).Use(typeof (CachedDocumentRepository));
                                  }
                                  else
                                  {
                                      c.For(typeof (IDocumentRepository)).Use(typeof (DocumentRepository));
                                  }
                              });

            return assimilate;
        }
    }
}
