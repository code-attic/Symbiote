using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Symbiote.Http.Impl.Adapter.SocketListener
{
    public class HttpSocketTransform : IContextTransformer<Socket>
    {

        const byte Cr = 0x0d;
        const byte Lf = 0x0a;
        const int CrLfCrLf =  218762506;

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

                //check for the request body separator
                var index = -1;
                while ( index++ < buffer.Length && bodyIndex == 0 )
                {
                    if ( index < buffer.Length - 5 
                         && buffer[index] == Cr 
                         && buffer[index + 1] == Lf
                         && buffer[index + 2] == Cr
                         && buffer[index + 3] == Lf)
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
                                  Headers = BuildHeaders( headers.Skip( 1 ) ),
                                  RequestStream = bodyIndex == memory.Length ? null : new MemoryStream( memory.GetBuffer(), bodyIndex, (int) memory.Length ),
                                  Method = requestSegments[0],
                                  Uri = requestSegments[1],
                                  Parameters = parameters,
                                  Url = parameterList[0],
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
            return new[]
                       {
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_METHOD, origin.Method ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_URI, origin.Url ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_HEADERS, origin.Headers ),
                           Tuple.Create<string, object>( Owin.ItemKeys.BASE_URI, origin.Uri ),
                           Tuple.Create<string, object>( Owin.ItemKeys.SERVER_NAME, origin.Server ),
                           Tuple.Create<string, object>( Owin.ItemKeys.URI_SCHEME, origin.Scheme ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REMOTE_ENDPOINT, origin.ClientEndpoint ),
                           Tuple.Create<string, object>( Owin.ItemKeys.VERSION, origin.Version ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST, origin ),
                           //Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_BODY, ( (b, o, l, c, e) => origin.Read(b, o, l, c, e) ),
                       }.ToDictionary( x => x.Item1, x => x.Item2 );
        }
    }
}