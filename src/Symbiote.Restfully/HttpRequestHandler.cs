using System.Web;
using System.Web.Routing;

namespace Symbiote.Restfully
{
    public abstract class HttpRequestHandler : IHttpHandler
    {
        protected IHttpServerConfiguration _configuration;
        protected RequestContext _requestContext;

        public abstract void ProcessRequest(HttpContext context);

        public bool IsReusable
        {
            get { return false; }
        }

        protected HttpRequestHandler(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }
    }
}