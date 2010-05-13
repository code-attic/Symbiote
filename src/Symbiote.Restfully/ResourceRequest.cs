using System;
using System.IO;
using System.Net;

namespace Symbiote.Restfully
{
    public class ResourceRequest
    {
        protected IHttpServerConfiguration Configuration { get; set; }

        public HttpListenerContext Context { get; protected set; }
        public HttpListenerRequest Request { get { return Context.Request; } }
        public HttpListenerResponse Response { get { return Context.Response; } }
        public string Resource { get; protected set; }
        public string Action { get; protected set; }
        public bool IsValid { get; protected set; }
        public string RequestBody { get; protected set; }

        protected virtual void Initialize()
        {
            var splits = Context.Request.Url.AbsolutePath.Split(new [] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            if (splits.Length == 2)
            {
                Resource = splits[0];
                Action = splits[1];
                IsValid = true;

                using (var stream = Request.InputStream)
                using (var streamReader = new StreamReader(stream))
                    RequestBody = streamReader.ReadToEnd();
            }
            else
            {
                IsValid = false;
                return;
            }
        }

        public ResourceRequest(HttpListenerContext context, IHttpServerConfiguration configuration)
        {
            Context = context;
            Configuration = configuration;
            Initialize();
        }
    }
}