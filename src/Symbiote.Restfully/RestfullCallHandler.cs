using System;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Symbiote.JsonRpc.Config;
using Symbiote.JsonRpc.Impl.Rpc;

namespace Symbiote.JsonRpc
{
    public class RestfullCallHandler : IHttpHandler
    {
        private IHttpServiceHostConfiguration _configuration;
        protected IHttpServiceHostConfiguration Configuration 
        {
            get 
            { 
                _configuration = _configuration ?? ServiceLocator.Current.GetInstance<IHttpServiceHostConfiguration>();
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
