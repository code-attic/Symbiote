using System;
using StructureMap;
using Symbiote.Wcf.Client;

namespace Symbiote.Wcf
{
    public class ServiceClientFactory
    {
        public static IService<TContract> GetClient<TContract>()
            where TContract : class
        {
            return ObjectFactory.GetInstance<IService<TContract>>();
        }

        public static IService<TContract> GetClient<TContract>(Action<IServiceConfiguration> configure)
            where TContract : class
        {
            var client = GetClient<TContract>();
            configure(client as IServiceConfiguration);
            return client;
        }
    }
}
