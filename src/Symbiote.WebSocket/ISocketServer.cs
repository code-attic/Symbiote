using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using Symbiote.WebSocket.Impl;

namespace Symbiote.WebSocket
{
    public interface ISocketServer :
        IDisposable,
        IObservable<Tuple<string, string>>
    {
        ConcurrentDictionary<string, IWebSocket> ClientSockets { get; }
        event Action<string> ClientConnected;
        event Action<string> ClientDisconnected;
        event Action Shutdown;

        void AddMessageHandle(Action<Tuple<string, string>> messageHandler);
        void Close();
        Socket Listener { get; }
        bool SendToAll(string data, string from);
        bool Send(string data, string from, string to);
        void Start();
    }
}