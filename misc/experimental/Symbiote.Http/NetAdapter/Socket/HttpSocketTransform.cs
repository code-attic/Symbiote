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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Symbiote.Http.Impl.Adapter.SocketListener
{
    public class HttpSocketAdapter
    {
        
    }

    public class HttpSocketTransform : IContextTransformer<Socket>
    {
        const byte CR = 0x0d;
		const byte LF = 0x0a;
		const byte DOT = 0x2e;
		const byte SPACE = 0x20;
		const byte SEMI = 0x3b;
		const byte COLON = 0x3a;
		const byte HASH = 0x23;
		const byte QMARK = 0x3f;
		const byte SLASH = 0x2f;
		const byte DASH = 0x2d;
		const byte NULL = 0x00;
        readonly byte[] LINE_TERMINATOR = new [] { CR, LF };
        readonly byte[] HEADER_TERMINATOR = new[] { CR, LF, CR, LF };

        public Context From<T>( T context )
        {
            return From( context as Socket );
        }

        public Context From( Socket context )
        {
            var memory = new MemoryStream();
            var buffer = new byte[8*1024];
            var read = 0;
            var last = 0;
            var headerIndex = 0;
            var bodyIndex = 0;
            do
            {
                read = context.Receive( buffer, 0, buffer.Length, SocketFlags.None );
                memory.Write( buffer, 0, read );

                // check for the request body separator
                var index = -1;
                while ( index++ < buffer.Length && bodyIndex == 0 )
                {
                    if ( index < buffer.Length - 5 
                         && buffer[index] == CR 
                         && buffer[index + 1] == LF
                         && buffer[index + 2] == CR
                         && buffer[index + 3] == LF)
                    {
                        headerIndex = index;
                        bodyIndex = index + 4;
                    }
                }
            } while ( read == buffer.Length );

            if ( memory.Length == 0 )
            {
                return null;
            }

            var headers = Encoding.UTF8
                .GetString(memory.GetBuffer(), 0, headerIndex - 1)
                .Split( new [] { "\r\n" }, StringSplitOptions.None );

            var requestSegments = headers[0].Split( ' ' );
            var parameterList = requestSegments[1].Split( '?' );
            var parameters = parameterList.Length < 2 
                                 ? null 
                                 : parameterList[1]
                                       .Split( '&' )
                                       .Select( x => 
                                                    { 
                                                        var parts = x.Split( '=' ); 
                                                        return Tuple.Create( parts[0], parts[1] );
                                                    } ).ToDictionary( x => x.Item1, x => x.Item2 );

            var request = new Request()
                              {
                                  ClientEndpoint = (IPEndPoint) context.RemoteEndPoint,
                                  //Headers = BuildHeaders( headers.Skip( 1 ) ),
                                  RequestStream = bodyIndex == memory.Length ? null : new MemoryStream( memory.GetBuffer(), bodyIndex, (int) memory.Length ),
                                  Method = requestSegments[0],
                                  BaseUri = requestSegments[1],
                                  Parameters = parameters,
                                  RequestUri = parameterList[0],
                              };

            request.Items = ProcessItems( request );

            return new Context( request, new SocketResponseAdapter( context ) );
        }

        public IDictionary<string, IEnumerable<string>> BuildHeaders(IEnumerable<string> headers)
        {
            return headers.Select( x =>
                                       {
                                           var header = x.Split( ':' );
                                           return Tuple.Create( header[0].Trim(), header[1].Split( ',' ).Select( v => v.Trim() ) );
                                       } )
                .ToDictionary( x => x.Item1, x => x.Item2 );
        }

        public IDictionary<string, object> ProcessItems( Request origin )
        {
            var requestBody = new Action<Action<ArraySegment<byte>>, Action<Exception>>( origin.ReadNext );

            return new Dictionary<string, object>()
                {
                    { Owin.ItemKeys.REQUEST_METHOD, origin.Method },   
                    { Owin.ItemKeys.REQUEST_URI, origin.RequestUri },
                    { Owin.ItemKeys.REQUEST_HEADERS, origin.Headers },
                    { Owin.ItemKeys.BASE_URI, origin.BaseUri },
                    { Owin.ItemKeys.SERVER_NAME, origin.Server },
                    { Owin.ItemKeys.URI_SCHEME, origin.Scheme },
                    { Owin.ItemKeys.REMOTE_ENDPOINT, origin.ClientEndpoint },
                    { Owin.ItemKeys.VERSION, origin.Version },
                    { Owin.ItemKeys.REQUEST, origin },
                    { Owin.ItemKeys.REQUEST_BODY, requestBody },
                };
        }
    }
}