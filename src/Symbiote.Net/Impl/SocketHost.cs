using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Futures;

namespace Symbiote.Net 
{
	public class SocketServer 
		: ISocketServer
    {
        public SocketConfiguration Configuration { get; set; }
        public Socket Listener { get; set; }
        public bool Running { get; protected set; }
        public Action<ISocketAdapter, Action<ISocketAdapter>> OnConnection { get; set; }
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
				Root.AddNode( new SocketNode( adapter ) );
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

        public void Start( Action<ISocketAdapter, Action<ISocketAdapter>> onConnection )
        {
            Running = true;
            OnConnection = onConnection;
            Listener = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP );
            Listener.Bind( ServerEndpoint );
            Listener.Listen( 10000 );
            WaitForConnection();
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
