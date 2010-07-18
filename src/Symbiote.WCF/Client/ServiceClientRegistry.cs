using StructureMap.Configuration.DSL;

namespace Symbiote.Wcf.Client
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