using System.Net.Sockets;

namespace Symbiote.WebSocket.Impl
{
    public interface IShakeHands
    {
        bool ValidateHandShake(Socket socket);
    }
}