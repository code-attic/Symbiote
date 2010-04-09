using System;
using System.Collections.Generic;

namespace Symbiote.WebSocket
{
    public interface IWebSocketServerConfiguration
    {
        string SocketUrl { get; set; }
        string ServerUrl { get; set; }
        int Port { get; set; }
        int ReceiveBufferSize { get; set; }
        int MaxPendingConnections { get; set; }
        bool ListenForPolicyRequests { get; set; }
    }
}