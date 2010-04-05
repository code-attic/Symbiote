using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.WebSocket
{
    public class WebSocket : IWebSocket
    {
        protected IList<IObserver<Tuple<string, string>>> _observers = new List<IObserver<Tuple<string, string>>>();
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

        public Action<string> OnDisconnect { get; set; }

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
            OnDisconnect(ClientId);
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

        public WebSocket(string clientId, Socket socket, int bufferSize)
        {
            ClientId = clientId;
            _socket = socket;
            _bufferSize = bufferSize;
            Listen();
        }
    }
}