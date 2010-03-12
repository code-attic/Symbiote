using System.ServiceModel.Description;

namespace Symbiote.WCFDynamicClient.Impl
{
    public interface IServiceMetadataCache
    {
        ServiceEndpoint GetEndPoint<TContract>(string metadataExchange);
    }
}