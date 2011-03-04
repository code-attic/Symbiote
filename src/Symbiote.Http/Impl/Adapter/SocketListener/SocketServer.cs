using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core.Futures;
using Symbiote.Http.Impl.Adapter.TcpListener;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.SocketListener {
    
    public class ClientSocket
        : IDisposable
    {
        public string Id { get; set; }
        public Socket Socket { get; set; }
        public ClientSocket Next { get; set; }
        public ClientSocket Previous { get; set; }

        public Action OnError { get; set; }
        public Action<ArraySegment<byte>> OnData { get; set; }
        public Action Remove { get; set; }

        public byte[] Buffer { get; set; }
        public readonly int BufferSize = 8 * 1024;

        public void Disconnect()
        {
            Socket.Close();
            Socket = null;
            if( !Next.Id.Equals( Id ) )
                Previous.Next = Next;
            if( !Previous.Id.Equals( Id ) )
                Next.Previous = Previous;
            Remove();
        }

        public ClientSocket Add( string id, 
            Socket socket,
            Action remove, 
            Action<ArraySegment<byte>> onData,
            Action onError )
        {
            var newNode = new ClientSocket( id, socket, Next, this, onError, onData, remove );
            Next.Previous = newNode;
            Next = newNode;
            return newNode;
        }

        public void WaitForReceive()
        {
            if( !Socket.Connected )
                Disconnect();

            Socket.BeginReceive( Buffer, 0, BufferSize, SocketFlags.OutOfBand, OnReceive, null );
        }

        public void OnReceive( IAsyncResult result )
        {
            var total = Socket.EndReceive( result );
            if( total > 0 )
            {
                OnData( new ArraySegment<byte>( Buffer, 0, total ) );
            }
            WaitForReceive();
        }

        public ClientSocket( string id, 
            Socket socket, 
            ClientSocket next, 
            ClientSocket previous, 
            Action onError, 
            Action<ArraySegment<byte>> onData, 
            Action remove )
        {
            Id = id;
            Socket = socket;
            Next = next;
            Previous = previous;
            OnError = onError;
            OnData = onData;
            Remove = remove;

            WaitForReceive();
        }

        public void Dispose()
        {
            Socket.Dispose();
        }
    }
    
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
        public ClientSocket Root { get; set; }

        public void Listen()
        {
            Future
                .Of( () => HttpSocket.BeginAccept( OnClient, null ) )
                .Start();
        }

        public void AddClient( Socket socket )
        {

        }

        public void OnClient(IAsyncResult result)
        {
            var client = HttpSocket.EndAccept( result );
            Listen();
            AddClient( client );
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
