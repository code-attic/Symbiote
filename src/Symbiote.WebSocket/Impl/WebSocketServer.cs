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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core.Extensions;

namespace Symbiote.WebSocket.Impl
{
    public class WebSocketServer : ISocketServer
    {
        protected IList<IObserver<Tuple<string, string>>> _observers = new List<IObserver<Tuple<string, string>>>();
        protected ConcurrentDictionary<string, IDisposable> _socketObservers = new ConcurrentDictionary<string, IDisposable>();
        protected ConcurrentDictionary<string, string> _clientAliasTable = new ConcurrentDictionary<string, string>();
        protected IPEndPoint _localEndPoint;
        protected ICreateWebSockets _socketFactory;
        protected IHandlePolicyRequests _policyRequestHandler;
        protected IShakeHands _handShaker;
        protected IWebSocketServerConfiguration _config;

        public event Action<string> ClientConnected;
        public event Action<string> ClientDisconnected;
        public event Action Shutdown;

        public virtual ConcurrentDictionary<string, IWebSocket> ClientSockets { get; private set; }
        public virtual Socket Listener { get; private set; }
        
        public virtual void AddMessageHandle(Action<Tuple<string, string>> messageHandler)
        {
            Subscribe(new DefaultClientObserver(messageHandler));
        }

        public virtual void Close()
        {
            if(Shutdown != null)
                Shutdown();

            _policyRequestHandler.Stop();
            _observers.ForEach(x => x.OnCompleted());
            _observers.Clear();
            _observers = null;
            Listener.Close();
            Listener.Dispose();
            Listener = null;
            ClientSockets.Values.ForEach(x => x.Dispose());
            ClientSockets.Clear();
            ClientSockets = null;
            _socketObservers.Clear();
            _socketObservers = null;
        }

        public virtual void Dispose()
        {
            ClientConnected = null;
            ClientDisconnected = null;
            Close();
        }

        public virtual bool SendToAll(string data, string from)
        {
            try
            {
                ClientSockets
                    .ForEach(x => x.Value.Send(data));
                return true;
            }
            catch (Exception e)
            {
                "An exception occurred sending the message '{0}' from {1} to all recipients \r\n\t {2}"
                    .ToError<ISocketServer>(data, from, e);
                return false;
            }
        }

        public virtual bool Send(string data, string from, string to)
        {
            if(string.IsNullOrEmpty(to))
            {
                return SendToAll(data, from);
            }
            else
            {
                try
                {
                    var client = _clientAliasTable
                                     .Where(x => x.Value == to)
                                     .Select(x => x.Key)
                                     .FirstOrDefault() ?? to;

                    IWebSocket socket = null;
                    if(ClientSockets.TryGetValue(client, out socket))
                    {
                        socket.Send(data);
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    "An exception occurred sending the message '{0}' from {1} to {2} \r\n\t {3}"
                        .ToError<ISocketServer>(data, from, to, e);
                    return false;
                }
            }
        }

        public virtual void Start()
        {
            CreateSocketListener();
            "Web socket server started on {0} \r\n\t at {1}, Port: {2}"
                .ToInfo<ISocketServer>(DateTime.Now, _config.SocketServer, _config.Port);
            if (_config.ListenForPolicyRequests)
            {
                _policyRequestHandler.Start();
            }
        }
        
        protected virtual void CreateSocketListener()
        {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            var hostIP = Dns.GetHostEntry(_config.SocketServer).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
            var endpoint = new IPEndPoint(hostIP, _config.Port);
            Listener.Bind(endpoint);
            Listener.Listen(50);
            ListenForConnections();
        }

        protected virtual void ListenForConnections()
        {
            Listener.BeginAccept(OnIncommingConnection, null);
        }

        protected virtual void OnClientDisconnect(string clientId)
        {
            KillSocketById(clientId);
            if (ClientDisconnected != null) ClientDisconnected(clientId);
        }

        protected virtual void OnIncommingConnection(IAsyncResult ar)
        {
            var newSocket = Listener.EndAccept(ar);
            
            var clientId = Guid.NewGuid().ToString();
            if(_handShaker.ValidateHandShake(newSocket))
            {
                var webSocket = _socketFactory.GetSocket(clientId, newSocket, _config.ReceiveBufferSize);
                webSocket.OnDisconnect = x => OnClientDisconnect(x);
                ClientSockets.TryAdd(clientId, webSocket);

                var observer = new WebSocketObserver()
                {
                    ClientId = clientId,
                    ClientDisconnect = x => { if (ClientDisconnected != null) ClientDisconnected(x); },
                    ClientError = (x, ex) => { "Client {0} \r\n\t {1}".ToError<ISocketServer>(x, ex); },
                    MessageReceived = MessageReceived
                };

                _socketObservers[clientId] = webSocket.Subscribe(observer);   
            }

            ListenForConnections();
        }

        protected virtual void MessageReceived(Tuple<string, string> message)
        {
            var alias = message.Item2.FromJson<AliasDefinition>();
            if (alias != null && !string.IsNullOrEmpty(alias.Name))
            {
                KillSocketByClientId(alias.Name);
                _clientAliasTable[message.Item1] = alias.Name;
                if (ClientConnected != null)
                    ClientConnected(alias.Name);
                Send(@"{""userAliasLoggedSuccessfully"":true}", "server", alias.Name);
            }
            else if(_clientAliasTable.ContainsKey(message.Item1))
            {
                var newMessage = Tuple.Create(_clientAliasTable[message.Item1], message.Item2);
                _observers
                    .ForEach(x => x.OnNext(newMessage));
            }
        }

        protected virtual void KillSocketByClientId(string clientId)
        {
            IWebSocket existingSocket = null;
            var id = Guid.Empty.ToString();

            if(_clientAliasTable.TryRemove(clientId, out id))
            if (ClientSockets.TryRemove(id, out existingSocket))
            {
                existingSocket.Dispose();
            }
        }

        protected virtual void KillSocketById(string id)
        {
            IWebSocket existingSocket = null;
            if (ClientSockets.TryRemove(id, out existingSocket))
            {
                existingSocket.Dispose();
            }
            var nothing = ""; // its really lame they don't let you remove a key w/o getting the value...
            var aliases = _clientAliasTable.Where(x => x.Value == id).Select(x => x.Key);
            aliases.ForEach(x => _clientAliasTable.TryRemove(x, out nothing));
        }

        public virtual IDisposable Subscribe(IObserver<Tuple<string, string>> observer)
        {
            _observers.Add(observer);
            return observer as IDisposable;
        }

        public WebSocketServer(
            IWebSocketServerConfiguration configuration, 
            ICreateWebSockets socketFactory,
            IShakeHands handShaker,
            IHandlePolicyRequests policyRequestHandler)
        {
            _config = configuration;
            _handShaker = handShaker;
            _policyRequestHandler = policyRequestHandler;
            _socketFactory = socketFactory;
            ClientSockets = new ConcurrentDictionary<string, IWebSocket>();
        }
    }
}