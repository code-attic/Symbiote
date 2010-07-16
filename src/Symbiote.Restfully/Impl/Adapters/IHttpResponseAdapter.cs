using System.IO;

namespace Symbiote.JsonRpc.Impl.Adapters
{
    public interface IHttpResponseAdapter
    {
        Stream OutputStream { get; }
    }
}