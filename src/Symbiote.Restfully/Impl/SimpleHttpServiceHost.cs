using System;
using System.Diagnostics;
using System.Net;
using Symbiote.Core.Extensions;
using Symbiote.JsonRpc.Config;
using Symbiote.JsonRpc.Impl.Rpc;

namespace Symbiote.JsonRpc.Impl
{
    public class SimpleHttpServiceHost : IHttpServiceHost
    {
        protected IHttpServiceHostConfiguration _configuration;
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
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var listenerContext = _listener.EndGetContext(result);
            var resourceRequest = new ResourceRequest(listenerContext, _configuration);
            resourceRequest.ExecuteProcedure();
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

        public SimpleHttpServiceHost(IHttpServiceHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            
        }
    }
}