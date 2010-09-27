/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
        
        public virtual WebSocketConfigurator EnforceStrictOriginMatching()
        {
            Configuration.StrictOriginMatching = true;
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