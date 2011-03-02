using System;
using Symbiote.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGraph.Impl.Config;

namespace NetGraph
{
    public static class NetGraphAssimilation
    {
        public static IAssimilate NetGraph(this IAssimilate assimilate, Action<NetGraphConfigurator> configure)
        {
            var configurator = new NetGraphConfigurator();
            configure(configurator);

            Assimilate
                .Dependencies(x =>
                {
                    x.For<NetGraphConfiguration>().Use(configurator.Configuration);
                    x.For<INetGraphClient>().Use <NetGraphClient>();
//                    x.For<INetGraphClient>().Use(new NetGraphClient(configurator.Configuration));
//                    x.For<IRedisConnectionFactory>().Use<RedisConnectionFactory>();
                });
            return assimilate;
        }
    }
}
