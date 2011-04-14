using System;
using Symbiote.Core;
using Symbiote.Core.Fibers;
using Symbiote.Net;

namespace Symbiote.Http.NetAdapter.Socket 
{
    public class SocketServerAdapter :
        IServerAdapter
    {
        public ISocketServer SocketServer { get; set; }
        public Director<ArraySegment<byte>> ApplicationMailboxes { get; set; }

        public void Start()
        {
            SocketServer.Start( OnSocket );
        }

        private void OnSocket( ISocket socket )
        {
            
        }

        public void Stop()
        {
            SocketServer.Stop();
        }

        public SocketServerAdapter( ISocketServer socketServer )
        {
            SocketServer = socketServer;
        }
    }
}
