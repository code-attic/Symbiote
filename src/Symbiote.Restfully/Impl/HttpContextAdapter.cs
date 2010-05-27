using System.Net;
using System.Web;

namespace Symbiote.Restfully.Impl
{
    public class HttpContextAdapter : IHttpContextAdapter
    {
        public IHttpRequestAdapter Request { get; protected set; }
        public IHttpResponseAdapter Response { get; protected set; }

        public HttpContextAdapter(HttpListenerContext context)
        {
            Request = new HttpListenerRequestAdapter(context.Request);
            Response = new HttpListenerResponseAdapter(context.Response);
        }

        public HttpContextAdapter(HttpContext context)
        {
            Request = new HttpRequestAdapter(context.Request);
            Response = new HttpResponseAdapter(context.Response);
        }
    }
}