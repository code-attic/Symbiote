// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System.Collections.Generic;
using System.Net;
using Symbiote.Core.Extensions;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.HttpListener
{
    public class HttpResponseAdapter : IResponseAdapter
    {
        public HttpListenerResponse Response { get; set; }

        public void Respond( string status, IDictionary<string, string> headers, IEnumerable<object> body )
        {
            var httpStatus = HttpStatus.Lookup[status];
            Response.StatusCode = httpStatus.Code;
            Response.StatusDescription = httpStatus.Description;

            headers.ForEach( x => Response.AddHeader( x.Key, x.Value ) );
            Response.ProtocolVersion = HttpVersion.Version11;
            body.ForEach( x => ResponseEncoder.Write( x, Response.OutputStream ) );

            Response.Close();
        }

        public HttpResponseAdapter( HttpListenerContext context )
        {
            Response = context.Response;
        }
    }
}