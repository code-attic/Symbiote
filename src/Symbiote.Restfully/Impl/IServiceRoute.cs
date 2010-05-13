using System.Web;
using System.Web.Routing;

namespace Symbiote.Restfully.Impl
{
    public interface IServiceRoute
    {
        IHttpHandler GetHandler(RequestContext requestContext);
    }
}