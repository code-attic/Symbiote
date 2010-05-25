using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Symbiote.Core.Extensions;

namespace Symbiote.Net
{
    public class SecureHttpClient : IHttpClient
    {
        public TcpClient TcpClient { get; protected set; }
        public Stream Stream { get; protected set; }
        
        public SecureHttpClient(TcpClient tcpClient, StoreLocation storeLocation, StoreName storeName, string certName)
        {
            TcpClient = tcpClient;
            Stream = EstablishSecureStream(storeLocation, storeName, certName);
        }

        private Stream EstablishSecureStream(StoreLocation storeLocation, StoreName storeName, string certName)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(X509FindType.FindBySubjectName, certName, false);
           
            if(certs.Count < 1)
            {
                throw new HttpServerException(
                    "Could not secure incoming connection because the specific certificate, {0} was not found in the {1} store under {2}"
                        .AsFormat(certName, storeName, storeLocation));
            }

            var certificate = certs[0];
            var secureStream = new SslStream(TcpClient.GetStream(), false, ValidateCertificate);
            secureStream.AuthenticateAsServer(
                certificate, 
                false, 
                SslProtocols.Default, 
                false);

            return secureStream;
        }

        private void FinishAuthentication(IAsyncResult ar)
        {
            var stream = ar.AsyncState as SslStream;
            stream.EndAuthenticateAsServer(ar);
        }

        private bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }
    }
}