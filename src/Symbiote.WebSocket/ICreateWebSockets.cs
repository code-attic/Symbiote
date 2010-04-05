using System.Net.Sockets;

namespace Symbiote.WebSocket
{
    public interface ICreateWebSockets
    {
        IWebSocket GetSocket(string clientId, Socket socket, int bufferSize);
    }
}