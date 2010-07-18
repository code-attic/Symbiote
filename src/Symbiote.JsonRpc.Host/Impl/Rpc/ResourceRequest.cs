using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Symbiote.Core.Extensions;
using Symbiote.JsonRpc.Host.Config;
using Symbiote.JsonRpc.Host.Impl.Adapters;

namespace Symbiote.JsonRpc.Host.Impl.Rpc
{
    public class ResourceRequest
    {
        protected IJsonRpcHostConfiguration Configuration { get; set; }

        public IHttpContextAdapter Context { get; protected set; }
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

                using (var stream = Context.Request.InputStream)
                using (var streamReader = new StreamReader(stream))
                    RequestBody = streamReader.ReadToEnd();
            }
            else
            {
                IsValid = false;
                return;
            }
        }

        public void ExecuteProcedure()
        {
            var contractType = GetServiceContractTypeByName(Resource);
            var actionName = Action;
            var methodInfo = contractType.GetMethod(actionName);
            var response = methodInfo.InvokeRemoteProcedure(contractType, RequestBody);
            using (var stream = Context.Response.OutputStream)
            using (var streamWriter = new StreamWriter(stream))
            {
                var body = response != null ? response.ToJson() : "ok";
                streamWriter.Write(body);
                streamWriter.Flush();
            }
        }

        protected virtual Type GetServiceContractTypeByName(string serviceName)
        {
            return Configuration.RegisteredServices.FirstOrDefault(
                x => x.Item1.Name == serviceName || x.Item2.Name == serviceName).Item1;
        }

        public ResourceRequest(HttpListenerContext context, IJsonRpcHostConfiguration configuration)
        {
            Context = new HttpContextAdapter(context);
            Configuration = configuration;
            Initialize();
        }
        
        public ResourceRequest(HttpContext context, IJsonRpcHostConfiguration configuration)
        {
            Context = new HttpContextAdapter(context);
            Configuration = configuration;
            Initialize();
        }
    }
}