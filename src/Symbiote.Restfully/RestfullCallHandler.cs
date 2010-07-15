using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Restfully.Config;
using Symbiote.Restfully.Impl;
using Symbiote.Restfully.Impl.Rpc;

namespace Symbiote.Restfully
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
