using System.Net.Sockets;

namespace Symbiote.Net
{
    public class ClientFactory
    {
        protected IHttpServerConfiguration Configuration { get; set; }

        public IHttpClient GetClient(TcpClient client)
        {
            if(Configuration.UseHttps)
            {
                return CreateSecureClient(client);
            }
            else
            {
                return CreateSimpleClient(client);
            }
        }

        protected IHttpClient CreateSecureClient(TcpClient client)
        {
            return new SecureHttpClient(
                client, 
                Configuration.X509StoreLocation, 
                Configuration.X509StoreName,
                Configuration.X509CertName);
        }

        protected IHttpClient CreateSimpleClient(TcpClient client)
        {
            return new SimpleHttpClient(client);
        }

        public ClientFactory(IHttpServerConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}