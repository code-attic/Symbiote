using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;

namespace Symbiote.WebSocket
{
    public class WebSocketServer : ISocketServer
    {
        protected IList<IObserver<Tuple<string, string>>> _observers = new List<IObserver<Tuple<string, string>>>();
        protected ConcurrentDictionary<string, IDisposable> _socketObservers = new ConcurrentDictionary<string, IDisposable>();
        protected int _bufferSize;
        protected IPEndPoint _localEndPoint;
        protected ICreateWebSockets _socketFactory;

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
            IEnumerableExtenders.ForEach<IWebSocket>(ClientSockets
                                         .Where(x => x.ClientId == to), x => x.Send(data));
        }

        public virtual void Start()
        {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _localEndPoint = new IPEndPoint(IPAddress.Loopback, Port);
            Listener.Bind(_localEndPoint);
            Listener.Listen(50);
            "Web socket server started on {0} \r\n\t at {1}, Port: {2}"
                .ToInfo<ISocketServer>(DateTime.Now, WebServerUrl, Port);

            ListenForConnections();
        }

        protected virtual void ListenForConnections()
        {
            Listener.BeginAccept(OnIncommingConnection, null);
        }

        protected virtual void OnIncommingConnection(IAsyncResult ar)
        {
            var newSocket = Listener.EndAccept(ar);
            Handshake(newSocket);
            var clientId = Guid.NewGuid().ToString();
            var webSocket = _socketFactory.GetSocket(clientId, newSocket, _bufferSize);
            webSocket.OnDisconnect = x =>
                                         {
                                             if (ClientDisconnected != null) ClientDisconnected(x);
                                         };
            ClientSockets.Add(webSocket);

            if(ClientConnected != null)
                ClientConnected(clientId);

            var observer = new WebSocketObserver()
                               {
                                   ClientId = clientId,
                                   ClientDisconnect = x => { if(ClientDisconnected != null) ClientDisconnected(x); },
                                   ClientError = (x, ex) => { "Client {0} \r\n\t {1}".ToError<ISocketServer>(x, ex); },
                                   MessageReceived = MessageReceived
                               };

            _socketObservers[clientId] = webSocket.Subscribe(observer);

            ListenForConnections();
        }

        protected virtual void MessageReceived(Tuple<string, string> message)
        {
            _observers
                .ForEach(x => x.OnNext(message));
        }

        protected virtual void Handshake(Socket socket)
        {
            using(var stream = new NetworkStream(socket))
            using(var reader = new StreamReader(stream))
            using(var writer = new StreamWriter(stream))
            {
                "Attempting handshake..."
                    .ToInfo<ISocketServer>();

                var request = new DelimitedBuilder("\r\n");
                var requestLine = "";
                do
                {
                    requestLine = reader.ReadLine();
                    request.Append(requestLine);
                } while (!string.IsNullOrEmpty(requestLine));

                "{0}".ToInfo<ISocketServer>(request.ToString());

                writer.WriteLine("HTTP/1.1 101 Web Socket Protocol Handshake");
                writer.WriteLine("Upgrade: WebSocket");
                writer.WriteLine("Connection: Upgrade");
                writer.WriteLine("WebSocket-Origin: " + WebServerUrl);
                writer.WriteLine("WebSocket-Location: " + WebSocketUrl);
                writer.WriteLine("");
            }

            "Handshake complete."
                .ToInfo<ISocketServer>();
        }

        public virtual IDisposable Subscribe(IObserver<Tuple<string, string>> observer)
        {
            _observers.Add(observer);
            return observer as IDisposable;
        }

        public WebSocketServer(IWebSocketServerConfiguration configuration, ICreateWebSockets socketFactory)
        {
            ClientSockets = new List<IWebSocket>();
            WebServerUrl = configuration.ServerUrl;
            WebSocketUrl = configuration.SocketUrl;
            _bufferSize = configuration.ReceiveBufferSize;
            Port = configuration.Port;
            _socketFactory = socketFactory;

            configuration
                .MessageProcessors
                .ForEach(AddMessageHandle);
        }
    }
}