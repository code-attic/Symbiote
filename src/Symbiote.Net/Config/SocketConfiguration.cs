using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Symbiote.Net
{
    public class SocketConfiguration
    {
        public int AllowedPendingRequests { get; set; }
        public AuthenticationSchemes AuthSchemes { get; set; }
        public int BufferSize { get; set; }
        public int Port { get; set; }
        public bool UseHttps { get; set; }
        public string X509CertName { get; set; }
        public StoreName X509StoreName { get; set; }
        public StoreLocation X509StoreLocation { get; set; }

        public SocketConfiguration()
        {
            Port = 8998;
            BufferSize = 4 * 1024;
        }
    }
}