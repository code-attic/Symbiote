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
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;

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

                                            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                                            store.Open(OpenFlags.ReadOnly);
                                            var certCount = store.Certificates.Count;
                                            var enumerator = store.Certificates.GetEnumerator();

                                            var list = new List<X509Certificate>();

                                            while(enumerator.MoveNext())
                                            {
                                                list.Add(enumerator.Current);
                                            }

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
                                     using (var stream = new SslStream(
                                         client.GetStream(),
                                         false,
                                         validateCert2))
                                     {

                                         stream.AuthenticateAsClient("symbiote");
                                         using (var reader = new StreamReader(stream))
                                         {
                                             message = reader.ReadLine();
                                         }

                                         stream.Close();
                                         client.Close();
                                     }
                                 };

        protected static bool validateCert2(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }
        
        private It should_have_message = () => message.ShouldEqual("HOWDY!");
    }

    public abstract class with_web_request : with_tcp_listener
    {
        protected static WebRequest request;
        protected static WebResponse response;

        private Establish context = () =>
                                        {
                                            ServicePointManager.ServerCertificateValidationCallback = validateCert;
                                            request = WebRequest.Create("https://localhost:8001");
                                            request.Credentials = new NetworkCredential("alex", "4l3x");
                                            request.PreAuthenticate = true;
                                            request.BeginGetResponse(onResponse, null);
                                        };

        private static void onResponse(IAsyncResult ar)
        {
            var response = request.EndGetResponse(ar);
        }
    }

    public class when_receiving_web_request : with_web_request
    {
        protected static string request;

        private Because of = () =>
                                 {
                                     int consecutiveFeeds = 0;

                                     var builder = new DelimitedBuilder(Environment.NewLine);

                                     while(secureStream == null || !secureStream.CanRead)
                                     {
                                         
                                     }

                                     var reader = new StreamReader(secureStream);
                                     while(consecutiveFeeds < 2 && !reader.EndOfStream)
                                     {
                                         var nextChar = reader.Peek();
                                         if (nextChar != 13)
                                         {
                                             var line = reader.ReadLine();
                                             if (string.IsNullOrEmpty(line))
                                             {
                                                 consecutiveFeeds++;
                                             }
                                             else
                                             {
                                                 builder.Append(line);
                                                 consecutiveFeeds = 0;
                                             }
                                         }
                                         else
                                         {
                                             break;
                                         }
                                     }
                                     request = builder.ToString();

                                     using(var writer = new StreamWriter(secureStream))
                                     {
                                         writer.WriteLine("BOOGABOOGA!");
                                     }
                                 };

        private It should_have_request_body = () => request.ShouldNotBeEmpty();
    }
}
