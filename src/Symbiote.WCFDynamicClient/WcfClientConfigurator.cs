using System;
using StructureMap;
using Symbiote.Wcf.Client;

namespace Symbiote.Wcf
{
    public class WcfClientConfigurator
    {
        public WcfClientConfigurator RegisterService<TContract, TStrategy>()
            where TStrategy : IServiceClientConfigurationStrategy<TContract>
            where TContract : class
        {
            ObjectFactory.Configure(x =>
                                        {
                                            x.For<IServiceClientConfigurationStrategy<TContract>>()
                                                .Use<TStrategy>();
                                        });

            return this;
        }

        public WcfClientConfigurator RegisterService<TContract>(Action<IServiceConfiguration> configurationDelegate)
            where TContract : class
        {
            ObjectFactory.Configure(x =>
                                        {
                                            x.For<IServiceClientConfigurationStrategy<TContract>>()
                                                .TheDefault.Is.ConstructedBy(f => new DelegateConfigurationStrategy<TContract>(configurationDelegate));
                                        });

            return this;
        }

        public WcfClientConfigurator RegisterService<TContract>()
            where TContract : class
        {
            return this;
        }
    }
}