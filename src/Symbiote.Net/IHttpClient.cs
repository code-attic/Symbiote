using System.IO;
using System.Net.Sockets;

namespace Symbiote.Net
{
    public interface IHttpClient
    {
        TcpClient TcpClient { get; }
        Stream Stream { get; }
    }
}