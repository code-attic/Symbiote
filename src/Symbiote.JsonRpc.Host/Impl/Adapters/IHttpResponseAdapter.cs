using System.IO;

namespace Symbiote.JsonRpc.Host.Impl.Adapters
{
    public interface IHttpResponseAdapter
    {
        Stream OutputStream { get; }
    }
}