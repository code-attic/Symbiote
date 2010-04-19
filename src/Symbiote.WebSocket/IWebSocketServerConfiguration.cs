using System;
using System.Collections.Generic;

namespace Symbiote.WebSocket
{
    public interface IWebSocketServerConfiguration
    {
        string ServerUrl { get; set; }
        string SocketUrl { get; }
        string SocketServer { get; set; }
        string SocketResource { get; set; }
        bool UseSecureSocket { get; set; }
        bool StrictOriginMatching { get; set; }
        int Port { get; set; }
        int ReceiveBufferSize { get; set; }
        int MaxPendingConnections { get; set; }
        bool ListenForPolicyRequests { get; set; }
    }
}