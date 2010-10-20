/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
                throw new HttpException(400, "Symbiote JSON RPC could not process the request because it was invalid.", ex);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
