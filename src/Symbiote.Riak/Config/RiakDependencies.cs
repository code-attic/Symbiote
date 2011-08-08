using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Core.Persistence;
using Symbiote.Riak.Impl;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Connection;

namespace Symbiote.Riak.Config
{
    public class RiakDependencies : IDefineDependencies
    {
        public Action<DependencyConfigurator> Dependencies()
        {
            var configurator = new RiakConfigurator();
            return container =>
                       {
                           container.For<IRiakConfiguration>().Use( configurator.Configuration );
                           container.For<IConnectionFactory>().Use<ConnectionFactory>();
                           container.For<IConnectionProvider>().Use<PooledConnectionProvider>().AsSingleton();
                           container.For<ICommandFactory>().Use<ProtoBufCommandFactory>().AsSingleton();
                           container.For<IConnectionPool>().Use<LockingConnectionPool>();
                           container.For<IRiakClient>().Use<RiakClient>();
                           container.For<IDocumentRepository>().Use<RiakClient>();
                           container.For<IKeyValueStore>().Use<RiakClient>();
                           container.For<ITrackVectors>().Use<VectorRegistry>().AsSingleton();
                       };
        }
    }
}