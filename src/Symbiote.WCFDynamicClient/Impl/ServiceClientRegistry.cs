using StructureMap.Configuration.DSL;

namespace Symbiote.WCFDynamicClient.Impl
{
    public class ServiceClientRegistry : Registry
    {
        public ServiceClientRegistry()
        {
            ForRequestedType(typeof(IService<>))
                .TheDefaultIsConcreteType(typeof(ServiceClient<>));

            ForRequestedType(typeof(IServiceClientConfigurationStrategy<>))
                .TheDefaultIsConcreteType(typeof(DefaultConfigurationStrategy<>));
        }
    }
}