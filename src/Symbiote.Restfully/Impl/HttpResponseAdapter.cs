﻿using System.IO;
using System.Web;

namespace Symbiote.Restfully.Impl
{
    public class HttpResponseAdapter : IHttpResponseAdapter
    {
        protected HttpResponse Response { get; set; }

        public Stream OutputStream { get { return Response.OutputStream; } }

        public HttpResponseAdapter(HttpResponse response)
        {
            Response = response;
        }
    }
}