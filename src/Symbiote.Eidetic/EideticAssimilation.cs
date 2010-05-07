using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching.Configuration;
using Symbiote.Core;
using Symbiote.Core.Cache;
using Symbiote.Eidetic.Config;
using Symbiote.Eidetic.Extensions;
using Symbiote.Eidetic.Impl;

namespace Symbiote.Eidetic
{
    public static class EideticAssimilation
    {
        public static IAssimilate Eidetic(this IAssimilate assimilate)
        {
            assimilate
                .Dependencies(x =>
                  {
                      x.For<IMemcachedClientConfiguration>()
                          .Use<DefaultMemcachedConfiguration>();
                      x.For<IRemembrance>()
                          .Use<JsonRemembrance>();
                      x.For<IRemember>()
                          .Use<MemcachedAdapter>();
                      x.For<ICacheProvider>()
                          .Use<EideticCacheProvider>();
                  });
            return assimilate;
        }

        public static IAssimilate Eidetic(this IAssimilate assimilate, Action<EideticConfigurator> configure)
        {
            assimilate.Eidetic();
            var config = new EideticConfigurator();
            configure(config);
            assimilate.Dependencies(x => x.For<IMemcachedClientConfiguration>()
                                             .Use(config.Configuration));
            return assimilate;
        }
    }
}
