using System;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Riak.Config;

namespace Symbiote.Riak
{
    public static class RiakAssimilation
    {
        public static IAssimilate Riak( this IAssimilate assimilate, Action<RiakConfigurator> configurate )
        {
            var configurator = new RiakConfigurator();
            configurate( configurator );

            Assimilate.Dependencies( x => { x.For<IRiakConfiguration>().Use( configurator.Configuration ); } );

            return assimilate;
        }
    }
}