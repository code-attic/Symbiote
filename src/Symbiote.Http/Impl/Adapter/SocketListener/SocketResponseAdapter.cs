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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Utility;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.SocketListener
{
    public class SocketResponseAdapter : IResponseAdapter
    {
        public Socket Response { get; set; }

        public void Respond( string status, IDictionary<string, string> headers, IEnumerable<object> body )
        {
            try
            {
                var httpStatus = HttpStatus.Lookup[status];

                var builder = new DelimitedBuilder("\r\n");
                var headerBuilder = new HeaderBuilder( headers );
                var responseBody = new MemoryStream();

                body.ForEach( x => ResponseEncoder.Write( x, responseBody ) );

                builder.AppendFormat( "HTTP/1.1 {0}", status );
                headerBuilder.ContentLength( responseBody.Length );
                headerBuilder.Date( DateTime.UtcNow );

                headers.ForEach( x => builder.AppendFormat( "{0}: {1}", x.Key, x.Value ) );
                builder.Append( "\r\n" );
                var header = builder.ToString();
                var headerBuffer = Encoding.UTF8.GetBytes( header );
                var bodyBuffer = responseBody.GetBuffer();
                Response.Send( headerBuffer );
                Response.Send( bodyBuffer );
                
            }
            catch ( SocketException socketException )
            {
                
            }
        }

        public SocketResponseAdapter( Socket context )
        {
            Response = context;
        }
    }
}