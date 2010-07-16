using System;
using System.IO;
using System.Web;

namespace Symbiote.JsonRpc.Impl.Adapters
{
    public class HttpRequestAdapter : IHttpRequestAdapter
    {
        protected HttpRequest Request { get; set; }

        public Uri Url
        {
            get { return Request.Url; }
        }

        public Stream InputStream
        {
            get { return Request.InputStream; }
        }

        public HttpRequestAdapter(HttpRequest request)
        {
            Request = request;
        }
    }
}