using System.Net.Sockets;

namespace Symbiote.WebSocket.Impl
{
    public interface ICreateWebSockets
    {
        IWebSocket GetSocket(string clientId, Socket socket, int bufferSize);
    }
}