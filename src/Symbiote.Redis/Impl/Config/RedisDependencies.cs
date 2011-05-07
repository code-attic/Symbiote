using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Redis.Impl;
using Symbiote.Redis.Impl.Config;
using Symbiote.Redis.Impl.Connection;
using Symbiote.Redis.Impl.Serialization;

namespace Symbiote.Redis
{
    public class RedisDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            var configurator = new RedisConfigurator();
            return x =>
                       {
                           x.For<RedisConfiguration>().Use( configurator.Configuration );
                           x.For<IConnectionPool>().Use<LockingConnectionPool>().AsSingleton();
                           x.For<IRedisClient>().Use<RedisClient>();
                           x.For<ICacheSerializer>().Use<ProtobufCacheSerializer>();
                           x.For<IConditionalCacheSerializer>().Use<ConditionalCacheSerializer>();
                           x.For<IConnectionProvider>().Use<PooledConnectionProvider>();
                           x.For<IConnectionFactory>().Use<ConnectionFactory>();
                       };
        }
    }
}