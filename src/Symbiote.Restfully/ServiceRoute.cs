using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using StructureMap;
using StructureMap.Pipeline;

namespace Symbiote.Restfully
{
    public class ServiceRoute<THandler> : Route, IServiceRoute
        where THandler : IHttpHandler
    {
        public IHttpHandler GetHandler(RequestContext requestContext)
        {
            return ObjectFactory.GetInstance<THandler>(new ExplicitArguments(new Dictionary<string, object>() { {"requestContext", requestContext} }));
        }

        public ServiceRoute(string url, IRouteHandler routeHandler) : base(url, routeHandler)
        {
        }

        public ServiceRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler)
        {
        }

        public ServiceRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler)
        {
        }

        public ServiceRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }
    }
}