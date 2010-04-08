using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;

namespace Symbiote.WebSocket
{
    public class WebSocketServer : ISocketServer
    {
        protected IList<IObserver<Tuple<string, string>>> _observers = new List<IObserver<Tuple<string, string>>>();
        protected ConcurrentDictionary<string, IDisposable> _socketObservers = new ConcurrentDictionary<string, IDisposable>();
        protected ConcurrentDictionary<string, string> _clientAliasTable = new ConcurrentDictionary<string, string>();
        protected int _bufferSize;
        protected IPEndPoint _localEndPoint;
        protected ICreateWebSockets _socketFactory;

        protected readonly string _handshake_line1 = "GET {0} HTTP/1.1";
        protected readonly string _handshake_line2 = "Upgrade: WebSocket";
        protected readonly string _handshake_line3 = "Connection: Upgrade";
        protected readonly string _handshake_line4 = "Host: {0}";
        protected readonly string _handshake_line5 = "Origin: {0}";
        protected readonly string _handshake_line6 = "";

        protected string HandshakeLine1
        {
            get
            {
                var regex = new Regex(@"(?<=[:][0-9]+(?=[\/])).+");
                var applicationPath = regex.Match(WebSocketUrl).Value;
                return
                    _handshake_line1.AsFormat(applicationPath);
            }
        }
        protected string HandshakeLine2 { get { return _handshake_line2; } }
        protected string HandshakeLine3 { get { return _handshake_line3; } }
        protected string HandshakeLine4
        {
            get
            {
                var regex = new Regex(@"(?<=[:][0-9]+(?=[\/])).+");
                var applicationPath = regex.Match(WebSocketUrl).Value;
                return _handshake_line4
                        .AsFormat(WebSocketUrl.Replace(applicationPath, "").Replace(@"ws://",""));
            }
        }
        protected string HandshakeLine5 { get { return _handshake_line5.AsFormat(WebServerUrl); } }
        protected string HandshakeLine6 { get { return _handshake_line6; } }

        protected IEnumerable<string> ExpectedHandshakeLines
        {
            get
            {
                yield return HandshakeLine1;
                yield return HandshakeLine2;
                yield return HandshakeLine3;
                yield return HandshakeLine4;
                yield return HandshakeLine5;
                yield return HandshakeLine6;
            }
        }

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
                    .FirstOrDefault();
                ClientSockets
                    .Where(x => x.ClientId == client)
                    .ForEach(x => x.Send(data));   
            }
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
            var clientId = Guid.NewGuid().ToString();
            if(Handshake(newSocket))
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
            }
            else if(_clientAliasTable.ContainsKey(message.Item1))
            {
                var newMessage = Tuple.Create(_clientAliasTable[message.Item1], message.Item2);
                _observers
                    .ForEach(x => x.OnNext(newMessage));
            }
        }

        protected virtual bool Handshake(Socket socket)
        {
            using(var stream = new NetworkStream(socket))
            using(var reader = new StreamReader(stream))
            using(var writer = new StreamWriter(stream))
            {
                "Attempting handshake..."
                    .ToInfo<ISocketServer>();

                var request = new DelimitedBuilder("\r\n");
                var requestLine = "";
                var expected = ExpectedHandshakeLines.ToArray();
                var handshakeIndex = 0;
                do
                {
                    requestLine = reader.ReadLine();
                    if(requestLine != expected[handshakeIndex])
                    {
                        "Step {0} of handshake from client is: \r\n\t {1}. \r\n\t Expected: {2}"
                            .ToError<ISocketServer>(handshakeIndex, requestLine, expected[handshakeIndex]);
                        socket.Close();
                        return false;
                    }
                    handshakeIndex++;
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

            return true;
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

    public class AliasDefinition
    {
        public string Name { get; set; }
    }
}