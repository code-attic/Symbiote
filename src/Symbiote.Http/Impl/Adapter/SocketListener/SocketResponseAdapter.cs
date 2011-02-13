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
                builder.Append( "\r\n\r\n" );
                var header = builder.ToString();
                var headerBuffer = Encoding.UTF8.GetBytes( header );
                var bodyBuffer = responseBody.GetBuffer();
                Response.Send( headerBuffer );
                Response.Send( bodyBuffer );
                Response.Close();
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