using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core.Futures;
using Symbiote.Http.Impl.Adapter.TcpListener;
using Symbiote.Http.Owin;
using Symbiote.Core.Extensions;

namespace Symbiote.Http.Impl.Adapter.SocketListener {
    public class SocketServer :
        IHost, IDisposable
    {
        public IHttpServerConfiguration Configuration { get; set; }
        public IRouteRequest RequestRouter { get; set; }
        public HttpSocketTransform ContextTransformer {get; set; }
        public bool Running { get; set; }
        public IPAddress ServerAddress { get; set; }
        public IPEndPoint ServerEndpoint { get; set; }
        public Socket HttpSocket { get; set; }

        public void Listen()
        {
            Future
                .Of( () => HttpSocket.BeginAccept( OnClient, null ) )
                .Start();
        }

        public void OnClient(IAsyncResult result)
        {
            var client = HttpSocket.EndAccept( result );
            Listen();
            ProcessRequest( client );
        }

        public void ProcessRequest( Socket clientSocket )
        {
            var context = ContextTransformer.From( clientSocket );
            var application = RequestRouter.GetApplicationFor( context.Request );
            application.Process(
                context.Request.Items,
                context.Response.Respond,
                OnApplicationException
                );
        }

        public void OnApplicationException( Exception exception )
        {
            Console.WriteLine( "Well, this is bad: \r\n {0}", exception );
        }


        public void Start()
        {
            HttpSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            HttpSocket.Bind( ServerEndpoint );
            HttpSocket.Listen( 1000 );
            Running = true;
            Listen();
        }

        public void Stop()
        {
            HttpSocket.Close();
        }

        public SocketServer( IHttpServerConfiguration configuration, IRouteRequest router )
        {
            Configuration = configuration;
            ServerAddress = IPAddress.Any;
            ServerEndpoint = new IPEndPoint( ServerAddress, configuration.Port );
            RequestRouter = router;
            ContextTransformer = new HttpSocketTransform();
        }

        public void Dispose()
        {
            if( HttpSocket.Connected )
                Stop();
        }
    }

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

    public class SocketResponseAdapter : IResponseAdapter
    {
        public Socket Response { get; set; }

        public void Respond( string status, IDictionary<string, string> headers, IEnumerable<object> body )
        {
            var httpStatus = HttpStatus.Lookup[status];

            var builder = new StringBuilder("\r\n");
            var responseBody = new MemoryStream();

            builder.AppendFormat( "HTTP/1.1 {0}", status );
            headers.ForEach( x => builder.AppendFormat( "{0}: {1}", x.Key, x.Value ) );
            
            body.ForEach( x => ResponseEncoder.Write( x, responseBody ) );

            Response.Send( Encoding.UTF8.GetBytes( builder.ToString() ) );
            Response.Send( responseBody.GetBuffer() );
        }

        public SocketResponseAdapter( Socket context )
        {
            Response = context;
        }
    }
}
