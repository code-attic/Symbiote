using System;
using Symbiote.WebSocket.Impl;

namespace Symbiote.WebSocket
{
    public class WebSocketConfigurator
    {
        public virtual IWebSocketServerConfiguration Configuration { get; private set; }

        public virtual WebSocketConfigurator BufferSize(int buffer)
        {
            Configuration.ReceiveBufferSize = buffer;
            return this;
        }
        
        public virtual WebSocketConfigurator LimitPendingConnectionsTo(int limit)
        {
            Configuration.MaxPendingConnections = limit;
            return this;
        }

        public virtual WebSocketConfigurator Port(int port)
        {
            Configuration.Port = port;
            return this;
        }

        public virtual WebSocketConfigurator PermitFlashSocketConnections()
        {
            Configuration.ListenForPolicyRequests = true;
            return this;
        }

        public virtual WebSocketConfigurator ServerUrl(string serverUrl)
        {
            Configuration.ServerUrl = serverUrl;
            return this;
        }

        public virtual WebSocketConfigurator SocketServer(string serverName)
        {
            Configuration.SocketServer = serverName;
            return this;
        }

        public virtual WebSocketConfigurator SocketResource(string resourcePath)
        {
            Configuration.SocketResource= resourcePath;
            return this;
        }

        public virtual WebSocketConfigurator UseSecureSocket()
        {
            Configuration.UseSecureSocket = true;
            return this;
        }

        public WebSocketConfigurator()
        {
            Configuration = new WebSocketServerConfiguration();
        }
    }
}