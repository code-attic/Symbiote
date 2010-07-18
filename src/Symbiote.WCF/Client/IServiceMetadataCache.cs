using System.ServiceModel.Description;

namespace Symbiote.Wcf.Client
{
    public interface IServiceMetadataCache
    {
        ServiceEndpoint GetEndPoint<TContract>(string metadataExchange);
    }
}