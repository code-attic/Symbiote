using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.WebSocket.Impl
{
    public class WebSocketServerConfiguration : IWebSocketServerConfiguration
    {
        public virtual string SocketUrl
        {
            get
            {
                return "{0}://{1}:{2}/{3}"
                    .AsFormat(
                        UseSecureSocket ? "wss" : "ws",
                        SocketServer,
                        Port,
                        SocketResource
                    );
            }
        }
        public virtual string ServerUrl { get; set; }
        public virtual int Port { get; set; }
        public virtual int ReceiveBufferSize { get; set; }
        public virtual int MaxPendingConnections { get; set; }
        public virtual bool ListenForPolicyRequests { get; set; }
        public string SocketServer { get; set; }
        public string SocketResource { get; set; }
        public bool UseSecureSocket { get; set; }

        public virtual IList<Action<Tuple<string, string>>> MessageProcessors { get; set; }
        public virtual IList<Action<string>> ClientConnectionHandlers { get; set;}
        public virtual IList<Action<string>> ClientDisconnectionHandlers { get; set;}
        public virtual IList<Action> ServerShutdownHandlers { get; set;}

        public WebSocketServerConfiguration()
        {
            Port = 8181;
            ReceiveBufferSize = 512;
            MaxPendingConnections = 80;
            ListenForPolicyRequests = false;
            MessageProcessors = new List<Action<Tuple<string, string>>>();
            ClientConnectionHandlers = new List<Action<string>>();
            ClientDisconnectionHandlers = new List<Action<string>>();
            ServerShutdownHandlers = new List<Action>();
        }
    }
}