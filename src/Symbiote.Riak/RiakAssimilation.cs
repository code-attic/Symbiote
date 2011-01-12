using System;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Riak.Config;
using Symbiote.Riak.Impl;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Connection;

namespace Symbiote.Riak
{
    public static class RiakAssimilation
    {
        public static IAssimilate Riak( this IAssimilate assimilate, Action<RiakConfigurator> configurate )
        {
            var configurator = new RiakConfigurator();
            configurate( configurator );

            Assimilate.Dependencies( x =>
            {
                x.For<IRiakConfiguration>().Use(configurator.Configuration);
                x.For<IConnectionFactory>().Use<ConnectionFactory>();
                x.For<IConnectionProvider>().Use<PooledConnectionProvider>();
                x.For<IBasicCommandFactory>().Use<ProtoBufCommandFactory>().AsSingleton();
                x.For<IConnectionPool>().Use<LockingConnectionPool>();
                x.For<IRiakServer>().Use<RiakServer>();
                x.For<IDocumentRepository>().Use<DocumentRepository>();
            } );

            return assimilate;
        }
    }
}