using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;

namespace Symbiote.WebSocket
{
    public enum Signal : byte { Start = 0, End = 255 };

    public interface IWebSocket 
        : IDisposable, IObservable<Tuple<string, string>>

    {
        string ClientId { get; set; }
        Socket NetworkSocket { get; }
        void Close();
        void Send(string message);
    }

    public interface ISocketServer :
        IDisposable,
        IObservable<Tuple<string, string>>
    {
        IList<IWebSocket> ClientSockets { get; }
        void Close();
        Socket Listener { get; }
        int Port { get; }
        string WebServerUrl { get; }
        string WebSocketUrl { get; }
        void SendToAll(string data, string from);
        void Send(string data, string from, string to);
        void Start();
    }

    public class WebSocketObserver : 
        IObserver<Tuple<string,string>>,
        IDisposable
    {
        public void OnNext(Tuple<string, string> value)
        {
            
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }

        public void Dispose()
        {
            
        }
    }

    public class WebSocketServer : ISocketServer
    {
        protected IList<IObserver<Tuple<string, string>>> _observers = new List<IObserver<Tuple<string, string>>>();
        protected IList<WebSocketObserver> _socketObservers = new List<WebSocketObserver>();
        protected ILogger<ISocketServer> _logger;
        protected int _bufferSize;
        protected IPEndPoint _localEndPoint;
        protected ICreateWebSockets _socketFactory;

        public virtual IList<IWebSocket> ClientSockets { get; private set; }
        public virtual Socket Listener { get; private set; }
        public virtual int Port { get; private set; }
        public string WebServerUrl { get; private set; }
        public string WebSocketUrl { get; private set; }

        public virtual void Close()
        {
            _observers.ForEach(x => x.OnCompleted());
            _observers.Clear();
            _observers = null;
            Listener.Close();
            Listener.Dispose();
            Listener = null;
            ClientSockets.ForEach(x => x.Dispose());
            ClientSockets.Clear();
            ClientSockets = null;
        }

        public virtual void Dispose()
        {
            Close();
        }

        public virtual void SendToAll(string data, string from)
        {
            ClientSockets
                .ForEach(x => x.Send(data));
        }

        public virtual void Send(string data, string from, string to)
        {
            ClientSockets
                .Where(x => x.ClientId == to)
                .ForEach(x => x.Send(data));
        }

        public virtual void Start()
        {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _localEndPoint = new IPEndPoint(IPAddress.Loopback, Port);
            Listener.Bind(_localEndPoint);
            Listener.Listen(50);
            _logger.Log(
                LogLevel.Info, 
                "Web socket server started on {0} \r\n\t at {1}, Port: {2}", 
                DateTime.Now, WebServerUrl, Port);
        }

        protected virtual void ListenForConnections()
        {
            Listener.BeginAccept(OnIncommingConnection, null);
        }

        protected virtual void OnIncommingConnection(IAsyncResult ar)
        {
            var newSocket = Listener.EndAccept(ar);
            Handshake(newSocket);
            var webSocket = _socketFactory.GetSocket(Guid.NewGuid().ToString(), newSocket, _bufferSize);
            ClientSockets.Add(webSocket);
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
                writer.WriteLine("WebSocket-Origin: " + WebSocketUrl);
                writer.WriteLine("WebSocket-Location: " + WebServerUrl);
                writer.WriteLine("");

                "Handshake complete."
                    .ToInfo<ISocketServer>();
            }
        }

        public virtual IDisposable Subscribe(IObserver<Tuple<string, string>> observer)
        {
            _observers.Add(observer);
            return observer as IDisposable;
        }

        public WebSocketServer(WebSocketServerConfiguration configuration, ICreateWebSockets socketFactory)
        {
            ClientSockets = new List<WebSocket>();
            WebServerUrl = configuration.ServerUrl;
            WebSocketUrl = configuration.SocketUrl;
            _bufferSize = configuration.ReceiveBufferSize;
            Port = configuration.Port;
            _socketFactory = socketFactory;
        }
    }

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

        public WebSocketConfigurator()
        {
            Configuration = new WebSocketServerConfiguration();
        }
    }

    public interface ICreateWebSockets
    {
        IWebSocket GetSocket(string clientId, Socket socket, int bufferSize);
    }

    public class WebSocketFactory : ICreateWebSockets
    {
        public IWebSocket GetSocket(string clientId, Socket socket, int bufferSize)
        {
            return new WebSocket(clientId, socket, bufferSize);
        }
    }

    public class WebSocketServerConfiguration
    {
        public virtual string SocketUrl { get; set; }
        public virtual string ServerUrl { get; set; }
        public virtual int Port { get; set; }
        public virtual int ReceiveBufferSize { get; set; }
        public virtual int MaxPendingConnections { get; set; }

        public WebSocketServerConfiguration()
        {
            Port = 8181;
            ReceiveBufferSize = 512;
            MaxPendingConnections = 80;
        }
    }

    public class WebSocket : IWebSocket
    {
        protected IList<IObserver<Tuple<string, string>> _observers = new List<IObserver<Tuple<string, string>>>();
        protected Socket _socket;
        protected int _bufferSize;
        protected DelimitedBuilder _builder = new DelimitedBuilder("");
        protected bool _receiving = false;

        public string ClientId { get; set; }

        public Socket NetworkSocket
        {
            get { return _socket; }
            protected set { _socket = value; }
        }

        public virtual IDisposable Subscribe(IObserver<Tuple<string, string>> observer)
        {
            _observers.Add(observer);
            return observer as IDisposable;
        }

        public virtual void Close()
        {
            _socket.Close();
            _observers.ForEach(x => x.OnCompleted());
            _observers.Clear();
            _observers = null;
        }

        public virtual void Dispose()
        {
            Close();   
        }

        public virtual void Send(string message)
        {
            var content = new List<byte>(Encoding.UTF8.GetBytes(message));
            content.Insert(0, (byte)Signal.Start);
            content.Add((byte)Signal.End);
            _socket.Send(content.ToArray());
        }

        protected void Receive(IAsyncResult result)
        {
            var received = _socket.EndReceive(result);

            if(received > 0)
            {
                var buffer = result.AsyncState as byte[];
                var start = 0;
                var end = buffer.Length - 1;

                if(buffer[0] == (byte)Signal.Start)
                {
                    _receiving = true;
                    _builder.Clear();
                    start = 1;
                }

                var ends = buffer.Contains((byte) Signal.End);
                _builder.Append(UTF8Encoding.UTF8.GetString(buffer, start, end));
                
                if(ends)
                {
                    _receiving = false;
                    var message = Tuple.Create(ClientId, _builder.ToString());
                    _observers.ForEach(x => x.OnNext(message));
                }
            }

            Listen();
        }

        protected virtual void Listen()
        {
            var buffer = new byte[_bufferSize];
            _socket.BeginReceive(buffer, 0, _bufferSize, SocketFlags.None, Receive, buffer);
        }

        public WebSocket(string clientId, Socket socket) : this(clientId, socket, 512)
        {
        }

        public WebSocket(string clientId, Socket socket, int bufferSize)
        {
            ClientId = clientId;
            _socket = socket;
            _bufferSize = bufferSize;
            Listen();
        }
    }
}
