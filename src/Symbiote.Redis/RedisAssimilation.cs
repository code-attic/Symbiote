using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Redis.Impl;
using Symbiote.Redis.Impl.Config;
using Symbiote.Redis.Impl.Connection;
using Symbiote.Redis.Impl.Serialization;

namespace Symbiote.Redis
{
    public static class RedisAssimilation
    {
        public static IAssimilate Redis(this IAssimilate assimilate, Action<RedisConfigurator> configure)
        {
            var configurator = new RedisConfigurator();
            configure(configurator);

            Assimilate
                .Dependencies(x =>
                                  {
                                      x.For<RedisConfiguration>().Use(configurator.Configuration);
                                      x.For<IRedisConnectionPool>().Use<LockingRedisConnectionPool>().AsSingleton();
                                      x.For<IRedisClient>().Use<RedisClient>();
                                      x.For<ICacheSerializer>().Use<JsonCacheSerializer>();
                                      x.For<IRedisConnectionFactory>().Use<RedisConnectionFactory>();
                                  });
            return assimilate;
        }
    }
}
