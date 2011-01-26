using System.Collections.Generic;

namespace Symbiote.Http.Impl.Adapter
{
    public interface IResponseProvider
    {
        IEnumerable<object> GetResponseBody();
    }
}