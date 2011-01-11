using System;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Riak.Config;
using Symbiote.Riak.Impl;
using Symbiote.Riak.Impl.ProtoBuf;

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
                x.For<IRiakConfiguration>().Use( configurator.Configuration );
                x.For<IBasicCommandFactory>().Use<ProtoBufCommandFactory>().AsSingleton();
                x.For<IConnectionProvider>().Use<ProtoBufConnectionProvider>().AsSingleton();
                x.For<IRiakServer>().Use<RiakServer>();
                x.For<IDocumentRepository>().Use<DocumentRepository>();
            } );

            return assimilate;
        }
    }
}