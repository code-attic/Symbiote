using System;
using System.IO;
using System.Net;
using Machine.Specifications;
using Symbiote.Restfully;
using Symbiote.Restfully.Impl;
using Symbiote.Restfully.Impl.Rpc;

namespace Restfully.Tests
{
    public abstract class with_http_listener
    {
        protected static HttpListener listener;
        protected static HttpListenerContext httpContext;
        protected static string baseUri;
        protected static string relativePath;
        protected static string queryString;
        protected static ResourceRequest resourceRequest;

        private Establish context = () =>
                                        {
                                            baseUri = "http://localhost:8420/";
                                            listener = new HttpListener();
                                            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                                            listener.Prefixes.Add(baseUri);
                                            listener.Start();
                                            listener.BeginGetContext(ProcessRequest, null);
                                        };

        public static void ProcessRequest(IAsyncResult asyncResult)
        {
            httpContext = listener.EndGetContext(asyncResult);

            relativePath = httpContext.Request.Url.AbsolutePath;
            queryString = httpContext.Request.Url.Query;

            using(var stream = httpContext.Response.OutputStream)
            {
                var textWriter = new StreamWriter(stream);
                textWriter.Write("howdy");
                textWriter.Flush();
            }
        }
    }
}
