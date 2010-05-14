using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Symbiote.Core.Extensions;
using System.Linq;

namespace Symbiote.Restfully.Impl
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
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var listenerContext = _listener.EndGetContext(result);
            var resourceRequest = new ResourceRequest(listenerContext, _configuration);
            var contractType = GetServiceContractTypeByName(resourceRequest.Resource);
            var actionName = resourceRequest.Action;
            var methodInfo = contractType.GetMethod(actionName);
            var response = methodInfo.InvokeRemoteProcedure(contractType, resourceRequest.RequestBody);
            using(var stream = resourceRequest.Context.Response.OutputStream)
            using(var streamWriter = new StreamWriter(stream))
            {
                var body = response != null ? response.ToJson() : "ok";
                streamWriter.Write(body);
                stopWatch.Stop();
                streamWriter.Flush();
            }
            var elapsed = stopWatch.ElapsedMilliseconds;
        }

        protected virtual Type GetServiceContractTypeByName(string serviceName)
        {
            return _configuration.RegisteredServices.FirstOrDefault(
                x => x.Item1.Name == serviceName || x.Item2.Name == serviceName).Item1;
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