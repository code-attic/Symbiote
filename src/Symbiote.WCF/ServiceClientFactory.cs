using System;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Wcf.Client;

namespace Symbiote.Wcf
{
    public class ServiceClientFactory
    {
        public static IService<TContract> GetClient<TContract>()
            where TContract : class
        {
            return ServiceLocator.Current.GetInstance<IService<TContract>>();
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
