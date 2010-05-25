using System.IO;
using System.Net.Sockets;

namespace Symbiote.Net
{
    public class SimpleHttpClient : IHttpClient
    {
        public TcpClient TcpClient { get; protected set; }
        public Stream Stream { get; protected set; }

        public SimpleHttpClient(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            Stream = tcpClient.GetStream();
        }
    }
}