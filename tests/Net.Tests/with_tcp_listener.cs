using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
        protected static string certPath = @"symbiote.pfx";

        private Establish context = () =>
                                        {
                                            serverAddress = IPAddress.Any;
                                            serverEndpoint = new IPEndPoint(serverAddress, 8001);

                                            serverCertificate = new X509Certificate(certPath);

                                            listener = new TcpListener(serverEndpoint);
                                            listener.Start();
                                            listener.BeginAcceptTcpClient(ProcessClient, null);
                                        };

        private static void ProcessClient(IAsyncResult ar)
        {
            try
            {
                serversClient = listener.EndAcceptTcpClient(ar);
                secureStream = new SslStream(serversClient.GetStream(), false);
                secureStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Default, false);

                secureStream.Write(UTF8Encoding.UTF8.GetBytes("HOWDY!"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    public abstract class with_tcp_client : with_tcp_listener
    {
        protected static TcpClient client;

        private Establish context = () =>
                                        {
                                           client = new TcpClient();
                                           client.Connect("localhost", 8001);
                                        };
    }

    public class when_connecting_to_secure_server : with_tcp_client
    {
        protected static string message = "";

        private Because of = () =>
                                 {
                                     var stream = client.GetStream();

                                     while (!stream.DataAvailable)
                                     {
                                         // wait
                                     }

                                     using (var reader = new StreamReader(stream))
                                         message = reader.ReadToEnd();

                                     stream.Close();
                                     client.Close();
                                 };

        private It should_have_message = () => message.ShouldEqual("HOWDY!");
    }

    
}
