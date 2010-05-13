using System.Web;
using System.Web.Routing;

namespace Symbiote.Restfully
{
    public interface IServiceRoute
    {
        IHttpHandler GetHandler(RequestContext requestContext);
    }
}