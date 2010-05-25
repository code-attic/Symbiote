using System.IO;
using System.Net;

namespace Symbiote.Restfully.Impl
{
    public class HttpListenerResponseAdapter : IHttpResponseAdapter
    {
        protected HttpListenerResponse Response { get; set; }

        public Stream OutputStream { get { return Response.OutputStream; } }

        public HttpListenerResponseAdapter(HttpListenerResponse response)
        {
            Response = response;
        }
    }
}