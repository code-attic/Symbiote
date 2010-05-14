using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Symbiote.WCFDynamicClient.Impl;

namespace Symbiote.WCFDynamicClient
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

        public static void RegisterConfigruationStategy<TContract, TStrategy>()
            where TStrategy : IServiceClientConfigurationStrategy<TContract>
            where TContract : class
        {
            ObjectFactory.Configure(c =>
            {
                c.ForRequestedType<IServiceClientConfigurationStrategy<TContract>>()
                    .AddConcreteType<TStrategy>();
                c.ForRequestedType<IService<TContract>>().TheDefaultIsConcreteType<ServiceClient<TContract>>();
            });

        }

        public static void RegisterConfigurationDelegate<TContract>(Action<IServiceConfiguration> configurationDelegate)
            where TContract : class
        {
            ObjectFactory.Configure(c =>
            {
                c.ForRequestedType<IServiceClientConfigurationStrategy<TContract>>()
                    .TheDefault.Is.ConstructedBy(f => new DelegateConfigurationStrategy<TContract>(configurationDelegate));
            });
        }
    }
}
