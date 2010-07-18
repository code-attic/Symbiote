using System;
using System.Diagnostics;
using System.Net;
using Symbiote.Core.Extensions;
using Symbiote.JsonRpc.Host.Config;
using Symbiote.JsonRpc.Host.Impl.Rpc;

namespace Symbiote.JsonRpc.Host.Impl
{
    public class SimpleJsonRpcHost : IJsonRpcHost
    {
        protected IJsonRpcHostConfiguration _configuration;
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

        public SimpleJsonRpcHost(IJsonRpcHostConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Dispose()
        {
            
        }
    }
}