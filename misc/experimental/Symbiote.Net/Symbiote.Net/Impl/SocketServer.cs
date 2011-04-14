using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Symbiote.Core.Extensions;

namespace Symbiote.Net 
{
	public class SocketServer
		: ISocketServer
    {
        public SocketConfiguration Configuration { get; set; }
        public Socket Listener { get; set; }
        public bool Running { get; protected set; }
        public Action<ISocket> OnConnection { get; set; }
        public IPEndPoint ServerEndpoint { get; set; }
		public SocketNode Root { get; set; }
		public LoopScheduler Scheduler { get; set; }

        public void OnDisconnection( ISocketAdapter adapter )
        {
			adapter.Close();
        }

        public void WaitForConnection()
        {
            Listener.BeginAccept( OnClient, null );
        }

        private void OnClient( IAsyncResult result )
        {
            try
            {
                var socket = Listener.EndAccept( result );
                WaitForConnection();
				var adapter = new SocketAdapter( socket, Configuration );
                var socketNode = new SocketNode( adapter );
                Root.AddNode( socketNode );
                OnConnection( socketNode );
            }
            catch( SocketException sockEx )
            {
                Console.WriteLine( "WinSock sharted: {0}".AsFormat( sockEx ) );
            }
            finally
            {
                
            }
        }
		
		public void Loop()
		{
			var node = Root;
			while( Running )
			{
				if( node.Available )
					if( !node.ExecuteNextRead() )
						node.ExecuteNextWrite();
				node = node.Next;
			}
		}

        public void Start( Action<ISocket> onConnection )
        {
            Running = true;
            OnConnection = onConnection;
            Listener = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            Listener.Bind( ServerEndpoint );
            Listener.Listen( 10000 );
            WaitForConnection();
            var task = Task.Factory.StartNew( Loop );
        }

        public void Stop()
        {
            Running = false;
        }

        public SocketServer( SocketConfiguration configuration )
        {
            Configuration = configuration;
            ServerEndpoint = new IPEndPoint( IPAddress.Any, configuration.Port );
			Root = new SocketNode( null ) { Available = false };
			Scheduler = new LoopScheduler();
        }
    }
}
