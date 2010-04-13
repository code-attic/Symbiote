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
        protected int _bufferSize;
        protected IPEndPoint _localEndPoint;
        protected ICreateWebSockets _socketFactory;
        protected IHandlePolicyRequests _policyRequestHandler;
        protected IShakeHands _handShaker;
        protected bool _listenForPolicyRequests;

        public event Action<string> ClientConnected;
        public event Action<string> ClientDisconnected;
        public event Action Shutdown;

        public virtual IList<IWebSocket> ClientSockets { get; private set; }
        public virtual Socket Listener { get; private set; }
        public virtual int Port { get; private set; }
        public string WebServerUrl { get; private set; }
        public string WebSocketUrl { get; private set; }

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
            ClientSockets.ForEach(x => x.Dispose());
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

        public virtual void SendToAll(string data, string from)
        {
            ClientSockets
                .ForEach(x => x.Send(data));
        }

        public virtual void Send(string data, string from, string to)
        {
            if(string.IsNullOrEmpty(to))
            {
                ClientSockets
                    .ForEach(x => x.Send(data));   
            }
            else
            {
                var client = _clientAliasTable
                    .Where(x => x.Value == to)
                    .Select(x => x.Key)
                    .FirstOrDefault() ?? to;
                ClientSockets
                    .Where(x => x.ClientId == client)
                    .ForEach(x => x.Send(data));   
            }
        }

        public virtual void Start()
        {
            CreateSocketListener();
            "Web socket server started on {0} \r\n\t at {1}, Port: {2}"
                .ToInfo<ISocketServer>(DateTime.Now, WebServerUrl, Port);
            if (_listenForPolicyRequests)
            {
                _policyRequestHandler.Start();
            }
        }
        
        protected virtual void CreateSocketListener()
        {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            var hostName = Dns.GetHostName();
            var ipEntries = Dns.GetHostAddresses(hostName).Where(x => x.AddressFamily == AddressFamily.InterNetwork);
            _localEndPoint = new IPEndPoint(ipEntries.First(), Port);
            Listener.Bind(_localEndPoint);
            //ipEntries
            //    .ForEach(x => Listener.Bind(new IPEndPoint(x, Port)));
            Listener.Listen(50);
            ListenForConnections();
        }

        protected virtual void ListenForConnections()
        {
            Listener.BeginAccept(OnIncommingConnection, null);
        }

        protected virtual void OnIncommingConnection(IAsyncResult ar)
        {
            var newSocket = Listener.EndAccept(ar);
            var clientId = Guid.NewGuid().ToString();
            if(_handShaker.ValidateHandShake(newSocket))
            {
                var webSocket = _socketFactory.GetSocket(clientId, newSocket, _bufferSize);
                webSocket.OnDisconnect = x =>
                {
                    if (ClientDisconnected != null) ClientDisconnected(x);
                };
                ClientSockets.Add(webSocket);

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
            ClientSockets = new List<IWebSocket>();
            _handShaker = handShaker;
            _policyRequestHandler = policyRequestHandler;
            WebServerUrl = configuration.ServerUrl;
            WebSocketUrl = configuration.SocketUrl;
            _bufferSize = configuration.ReceiveBufferSize;
            Port = configuration.Port;
            _socketFactory = socketFactory;
            _listenForPolicyRequests = configuration.ListenForPolicyRequests;
        }
    }
}