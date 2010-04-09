using System;
using System.Net.Sockets;

namespace Symbiote.WebSocket.Impl
{
    public interface IWebSocket 
        : IDisposable, IObservable<Tuple<string, string>>

    {
        string ClientId { get; set; }
        Socket NetworkSocket { get; }
        Action<string> OnDisconnect { get; set; }
        void Close();
        void Send(string message);
    }
}