using System;
using Symbiote.Core;
using Symbiote.JsonRpc.Client.Config;
using Symbiote.JsonRpc.Client.Impl.Rpc;

namespace Symbiote.JsonRpc.Client
{
    public static class JsonRpcClientAssimilation
    {
        public static IAssimilate JsonRpcClient(this IAssimilate assimilate, Action<JsonRpcClientConfigurator> configure)
        {
            var configurator = new JsonRpcClientConfigurator(new JsonRpcClientConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<IJsonRpcClientConfiguration>().Use(configurator.GetConfiguration());
                                      x.For(typeof (IRemoteProxy<>)).Use(typeof (RemoteProxy<>));
                                  });

            return assimilate;
        }
    }
}