using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using Symbiote.Core.Extensions;
using Symbiote.WebSocket.Impl;

namespace Symbiote.WebSocket
{
    public class DefaultPolicyRequestHandler : IHandlePolicyRequests
    {
        protected IWebSocketServerConfiguration _configuration;
        protected ICreateWebSockets _socketFactory;
        protected XDocument _policyResponse;
        protected IPEndPoint _policyEndPoint;
        protected virtual Socket PolicyListener { get; private set; }

        public void Start()
        {
            CreatePolicyListener();
        }

        public void Stop()
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }

        protected void Close()
        {
            if(PolicyListener != null)
            {
                PolicyListener.Close();
                PolicyListener.Dispose();
                PolicyListener = null;
            }
        }

        protected virtual void CreatePolicyListener()
        {
            PolicyListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _policyEndPoint = new IPEndPoint(IPAddress.Loopback, 843);
            PolicyListener.Bind(_policyEndPoint);
            PolicyListener.Listen(50);
            ListenForPolicyConnections();
        }

        private void OnPolicyRequest(IAsyncResult ar)
        {
            "Policy Request Started"
                .ToInfo<ISocketServer>();

            var policySocket = PolicyListener.EndAccept(ar);
            var policyString = _policyResponse.ToString();
            using (var stream = new NetworkStream(policySocket))
            using (var writer = new StreamWriter(stream))
            {
                using(var reader = new StringReader(policyString))
                {
                    var line = reader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        writer.WriteLine(line);
                        line = reader.ReadLine();
                    }
                    writer.Write('\0');
                }
            }
            ListenForPolicyConnections();
        }

        protected virtual void ListenForPolicyConnections()
        {
            PolicyListener.BeginAccept(OnPolicyRequest, null);
        }

        public DefaultPolicyRequestHandler(IWebSocketServerConfiguration configuration, ICreateWebSockets socketFactory)
        {
            _configuration = configuration;
            _socketFactory = socketFactory;

            _policyResponse = new XDocument(
                new XElement("cross-domain-policy",
                             new XElement("allow-access-from",
                                          new XAttribute("domain", "*"),
                                          new XAttribute("to-ports", configuration.Port)
                                 )
                    )
                );
        }
    }
}