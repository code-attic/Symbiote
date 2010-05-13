using System.Web;
using System.Web.Routing;

namespace Symbiote.Restfully.Impl
{
    public class RouteDispatcher : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var serviceRoute = requestContext.RouteData.Route as IServiceRoute;
            return serviceRoute.GetHandler(requestContext);
        }
    }
}