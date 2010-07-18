using System;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Symbiote.JsonRpc.Host.Config;
using Symbiote.JsonRpc.Host.Impl.Rpc;

namespace Symbiote.JsonRpc.Host
{
    public class JsonRpcHandler : IHttpHandler
    {
        private IJsonRpcHostConfiguration _configuration;
        protected IJsonRpcHostConfiguration Configuration 
        {
            get 
            { 
                _configuration = _configuration ?? ServiceLocator.Current.GetInstance<IJsonRpcHostConfiguration>();
                return _configuration;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            var segments = context.Request.Path.Split('/');
            if(segments.Length < 2)
            {
                segments = context.Request.Path.Split(new [] {@"\"}, StringSplitOptions.None);
            }
            try
            {
                var service = segments[0];
                var method = segments[1];
                var resourceRequest = new ResourceRequest(context, Configuration);
                resourceRequest.ExecuteProcedure();
            }
            catch (Exception ex)
            {
                throw new HttpException(400, "Symbiote Restfully could not process the request because it was invalid.", ex);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
