using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Symbiote.WCFDynamicClient.Impl
{
    public class DefaultConfigurationStrategy<T> : IServiceClientConfigurationStrategy<T>
    {
        public void ConfigureServiceClient(IServiceConfiguration configuration)
        {
            configuration.Configuration = GetEndPointForContractType(typeof(T));
        }

        public string GetEndPointForContractType(Type contractType)
        {
            ClientSection clientSection =
                ConfigurationManager.GetSection("system.serviceModel/client") as ClientSection;

            var endpoints =
                clientSection.ElementInformation.Properties[string.Empty].Value as ChannelEndpointElementCollection;

            foreach (ChannelEndpointElement endpoint in endpoints)
            {
                if (endpoint.Contract.Equals(contractType.FullName) || endpoint.Contract.Equals(contractType.AssemblyQualifiedName))
                    return endpoint.Name;
            }
            return "";
        }
    }
}