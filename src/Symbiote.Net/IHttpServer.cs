using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Net
{
    public interface IHttpServer
    {
        void Start();
        void Stop();
    }

    public interface IClient
    {
        TcpClient Tcp { get; }
        Stream Stream { get; }
    }

    public class SimpleClient : IClient
    {
        public TcpClient Tcp { get; protected set; }
        public Stream Stream { get; protected set; }

        public SimpleClient(TcpClient tcp)
        {
            Tcp = tcp;
            Stream = tcp.GetStream();
        }
    }

    public class SecureClient : IClient
    {
        public TcpClient Tcp { get; protected set; }
        public Stream Stream { get; protected set; }
        
        public SecureClient(TcpClient tcp, StoreLocation storeLocation, StoreName storeName, string certName)
        {
            Tcp = tcp;
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
            var secureStream = new SslStream(Tcp.GetStream(), false, ValidateCertificate);
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

    public class HttpServerException : Exception
    {
        public HttpServerException(string message) :base(message) {}
        public HttpServerException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ClientFactory
    {
        protected IHttpServerConfiguration Configuration { get; set; }

        public IClient GetClient(TcpClient client)
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

        protected IClient CreateSecureClient(TcpClient client)
        {
            return new SecureClient(
                client, 
                Configuration.X509StoreLocation, 
                Configuration.X509StoreName,
                Configuration.X509CertName);
        }

        protected IClient CreateSimpleClient(TcpClient client)
        {
            return new SimpleClient(client);
        }

        public ClientFactory(IHttpServerConfiguration configuration)
        {
            Configuration = configuration;
        }
    }

    public class HttpRequest
    {
        public string Verb { get; protected set; }
        public string Host { get; protected set; }
        public string Path { get; protected set; }
        public string Query { get; protected set; }
        public string RawUrl { get; protected set; }
        public string Protocol { get; protected set; }
        public Version Version { get; protected set; }

        public HttpRequest(string requestBody)
        {
            Process(requestBody);
        }

        protected void Process(string requestBody)
        {
            try
            {
                // break into lines
                var lines = requestBody.Split(new string[] { @"\r\n", "@\n" }, StringSplitOptions.None);

                // handle each line with a method
                ProcessRequestLine(lines[0]);
                ProcessHostHeader(lines[1]);

                
            }
            catch (Exception ex)
            {
                throw new HttpServerException("Cannot process poorly formed client request. \r\n {0}".AsFormat(requestBody));
            }
            
        }

        private void ProcessHostHeader(string hostLine)
        {
            Host = hostLine.Split(':')[1].Trim();
        }

        private void ProcessRequestLine(string requestLine)
        {
            var values = requestLine.Split(' ');
            var url = values[1].Split('?');
            var protocolAndVersion = values[2].Split('/');

            Verb = values[0];
            Path = url[0];
            Query = url[1];
            Protocol = protocolAndVersion[0];
            Version = protocolAndVersion[1] == "1.0" ? HttpVersion.Version11 : HttpVersion.Version11;
        }
    }

    public class HttpServer : IHttpServer, IDisposable
    {
        protected TcpListener Listener { get; set; }
        protected IHttpServerConfiguration Configuration { get; set; }
        protected bool Running { get; set; }
        protected IPAddress ServerAddress { get; set; }
        protected IPEndPoint ServerEndpoint { get; set; }
        protected ConcurrentBag<TcpClient> ConnectedClients { get; set; }
        protected ClientFactory ClientFactory { get; set; }

        public void Start()
        {
            Listener.Start();
            Running = true;

            while(Running)
            {
                Listener.BeginAcceptTcpClient(HandleClient, null);
            }
        }

        private void HandleClient(IAsyncResult ar)
        {
            var client = Listener.EndAcceptTcpClient(ar);
            
        }

        public void Stop()
        {
            Listener.Stop();
        }

        protected void InitializeListener()
        {
            ServerAddress = IPAddress.Any;
            ServerEndpoint = new IPEndPoint(ServerAddress, 8001);
            Listener = new TcpListener(ServerEndpoint);
            Listener.ExclusiveAddressUse = true;
        }

        public HttpServer(IHttpServerConfiguration configuration)
        {
            Configuration = configuration;
            ConnectedClients = new ConcurrentBag<TcpClient>();
            InitializeListener();
            ClientFactory = new ClientFactory(configuration);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
