using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Symbiote.WCFDynamicClient.Impl
{
    public interface IServiceConfiguration
    {
        string Configuration { get; set; }
        Binding Binding { get; set; }
        EndpointAddress Endpoint { get; set; }
        string MetadataExchangeAddress { get; set; }
    }
}