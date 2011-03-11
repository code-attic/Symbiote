using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Machine.Specifications;

namespace Net.Tests
{
    public abstract class with_tcp_listener
    {
        protected static IPAddress serverAddress;
        protected static IPEndPoint serverEndpoint;
        protected static TcpListener listener;
        protected static TcpClient serversClient;
        protected static SslStream secureStream;
        protected static X509Certificate serverCertificate;
        
        private Establish context = () =>
                                        {
                                            serverAddress = IPAddress.Any;
                                            serverEndpoint = new IPEndPoint(serverAddress, 8001);

                                            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                                            store.Open(OpenFlags.ReadOnly);
                                            serverCertificate =
                                                store.Certificates.Find(X509FindType.FindBySubjectName, "symbiote",
                                                                        false)[0];

                                            listener = new TcpListener(serverEndpoint);
                                            listener.Start();
                                            listener.BeginAcceptTcpClient(ProcessClient, null);
                                        };

        private static void ProcessClient(IAsyncResult ar)
        {
            try
            {
                serversClient = listener.EndAcceptTcpClient(ar);
                secureStream = new SslStream(serversClient.GetStream(), false, validateCert);
                secureStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Default, true);

                //secureStream.Write(UTF8Encoding.UTF8.GetBytes("HOWDY!" + Environment.NewLine));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected static bool validateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }
    }
}
