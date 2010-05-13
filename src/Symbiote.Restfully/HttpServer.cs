using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using Symbiote.Core.Extensions;

namespace Symbiote.Restfully
{
    public class HttpServer : IHttpServer
    {
        protected IHttpServerConfiguration _configuration;
        protected HttpListener _listener;

        public void Start()
        {
            CreateListener();
            BeginListening();
        }

        protected virtual void BeginListening()
        {
            _listener.BeginGetContext(ProcessRequest, null);
        }

        protected virtual void ProcessRequest(IAsyncResult result)
        {
            var listenerContext = _listener.EndGetContext(result);
            var resourceRequest = new ResourceRequest(listenerContext, _configuration);
            var procedure = resourceRequest.RequestBody.FromJson() as IRemoteProcedure;
            var response = procedure.Invoke();
            using(var stream = resourceRequest.Response.OutputStream)
            using(var streamWriter = new StreamWriter(stream))
            {
                var body = response != null ? response.ToJson() : "ok";
                streamWriter.Write(body);
                streamWriter.Flush();
            }
        }

        protected virtual void CreateListener()
        {
            _listener = new HttpListener();
            _configuration
                .HostedUrls
                .ForEach(x => _listener.Prefixes.Add(x));

            _listener.AuthenticationSchemes = _configuration.AuthSchemes;
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Abort();
            _listener.Stop();
        }

        public HttpServer(IHttpServerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            
        }
    }
}