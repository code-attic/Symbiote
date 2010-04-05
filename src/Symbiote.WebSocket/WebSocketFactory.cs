using System.Net.Sockets;

namespace Symbiote.WebSocket
{
    public class WebSocketFactory : ICreateWebSockets
    {
        public IWebSocket GetSocket(string clientId, Socket socket, int bufferSize)
        {
            return new WebSocket(clientId, socket, bufferSize);
        }
    }
}