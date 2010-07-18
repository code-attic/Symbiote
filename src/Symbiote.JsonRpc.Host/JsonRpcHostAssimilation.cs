using System;
using Symbiote.Core;
using Symbiote.JsonRpc.Host.Config;
using Symbiote.JsonRpc.Host.Impl;

namespace Symbiote.JsonRpc.Host
{
    public static class JsonRpcHostAssimilation
    {
        public static IAssimilate JsonRpcHost(this IAssimilate assimilate, Action<JsonRpcHostConfigurator> configure)
        {
            var configurator = new JsonRpcHostConfigurator(new JsonRpcHostConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IJsonRpcHostConfiguration>().Use(configurator.GetConfiguration());
                                      if(configurator.GetConfiguration().SelfHosted)
                                          x.For<IJsonRpcHost>().Use<SimpleJsonRpcHost>();
                                  });

            return assimilate;
        }
    }
}
