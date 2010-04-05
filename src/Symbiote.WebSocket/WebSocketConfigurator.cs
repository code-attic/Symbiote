using System;

namespace Symbiote.WebSocket
{
    public class WebSocketConfigurator
    {
        public virtual WebSocketServerConfiguration Configuration { get; private set; }

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

        public virtual WebSocketConfigurator ServerUrl(string serverUrl)
        {
            Configuration.ServerUrl = serverUrl;
            return this;
        }

        public virtual WebSocketConfigurator SocketUrl(string socketUrl)
        {
            Configuration.SocketUrl = socketUrl;
            return this;
        }

        public virtual WebSocketConfigurator OnMessage(Action<Tuple<string, string>> messageHandler)
        {
            Configuration.MessageProcessors.Add(messageHandler);
            return this;
        }

        public virtual WebSocketConfigurator OnClientConnect(Action<string> clientConnectionHandler)
        {
            Configuration.ClientConnectionHandlers.Add(clientConnectionHandler);
            return this;
        }

        public virtual WebSocketConfigurator OnClientDisconnect(Action<string> clientDisconnectionHandler)
        {
            Configuration.ClientDisconnectionHandlers.Add(clientDisconnectionHandler);
            return this;
        }

        public virtual WebSocketConfigurator OnServerShutdown(Action serverShutdownHandler)
        {
            Configuration.ServerShutdownHandlers.Add(serverShutdownHandler);
            return this;
        }

        public WebSocketConfigurator()
        {
            Configuration = new WebSocketServerConfiguration();
        }
    }
}