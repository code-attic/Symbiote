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
        public virtual string SocketServer { get; set; }
        public virtual string SocketResource { get; set; }
        public virtual bool UseSecureSocket { get; set; }
        public virtual bool StrictOriginMatching { get; set; }

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