using System.IO;

namespace Symbiote.Restfully.Impl
{
    public interface IHttpResponseAdapter
    {
        Stream OutputStream { get; }
    }
}