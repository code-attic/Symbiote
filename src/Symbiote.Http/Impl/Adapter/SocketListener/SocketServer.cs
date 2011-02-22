using System;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core.Futures;
using Symbiote.Http.Impl.Adapter.TcpListener;
using Symbiote.Http.Owin;

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
            if( context != null )
            {
                var application = RequestRouter.GetApplicationFor( context.Request );
                application.Process(
                    context.Request.Items,
                    context.Response.Respond,
                    OnApplicationException
                    );
            }
        }

        public void OnApplicationException( Exception exception )
        {
            Console.WriteLine( "Well, this is bad: \r\n {0}", exception );
        }


        public void Start()
        {
            HttpSocket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            HttpSocket.Bind( ServerEndpoint );
            HttpSocket.Listen( 5000 );
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
}
