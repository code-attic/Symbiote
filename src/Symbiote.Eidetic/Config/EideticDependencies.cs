using System;
using Enyim.Caching.Configuration;
using Symbiote.Core;
using Symbiote.Core.Cache;
using Symbiote.Core.DI;
using Symbiote.Eidetic.Config;
using Symbiote.Eidetic.Extensions;
using Symbiote.Eidetic.Impl;

namespace Symbiote.Eidetic
{
    public class EideticDependencies : IDefineDependencies
    {
        public Action<DependencyConfigurator> Dependencies()
        {
            var config = new EideticConfigurator();
            Assimilate.Dependencies( x => x.For<IMemcachedClientConfiguration>()
                                              .Use( config.Configuration ) );

            return container => 
                       {
                           container.For<IMemcachedClientConfiguration>()
                               .Use<DefaultMemcachedConfiguration>();
                           container.For<IRemembrance>()
                               .Use<JsonRemembrance>();
                           container.For<IRemember>()
                               .Use<MemcachedAdapter>();
                           container.For<ICacheProvider>()
                               .Use<EideticCacheProvider>();
                       };
        }
    }
}