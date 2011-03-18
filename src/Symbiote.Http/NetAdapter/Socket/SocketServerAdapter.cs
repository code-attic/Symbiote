using Symbiote.Core;
using Symbiote.Net;

namespace Symbiote.Http.NetAdapter.Socket 
{
    public class SocketServerAdapter :
        IServerAdapter
    {
        public ISocketServer SocketServer { get; set; }

        public void Start()
        {
            SocketServer.Start( (socket, onClose ) =>
                                    {
                                        var proxy = Assimilate.GetInstanceOf<SocketApplicationProxy>();
                                        proxy.Start( socket, onClose );
                                    } );
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
