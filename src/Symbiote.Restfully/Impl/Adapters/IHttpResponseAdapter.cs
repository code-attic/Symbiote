using System.IO;

namespace Symbiote.Restfully.Impl.Adapters
{
    public interface IHttpResponseAdapter
    {
        Stream OutputStream { get; }
    }
}