using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Symbiote.Core;
using Symbiote.Eidetic;
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
                                      if(!ObjectFactory.Container.Model.HasDefaultImplementationFor<IRemember>())
                                      {
                                          throw new AssimilationException(
                                              "You must have an implementation of IRemember configured to use caching in Relax. Consider referencing Symbiote.Eidetic and adding the .Eidetic() call before this in your assimilation to utilize memcached or memcachedb as the cache provider for Relax."
                                          );
                                      }

                                      c.For(typeof (IDocumentRepository<>)).Use(typeof (CachedDocumentRepository<>));
                                  }
                                  else
                                  {
                                      c.For(typeof (IDocumentRepository<>)).Use(typeof (DocumentRepository<>));
                                  }
                              });

            return assimilate;
        }
    }
}
