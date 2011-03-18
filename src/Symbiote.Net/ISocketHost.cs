using System;

namespace Symbiote.Net
{
    public interface ISocketServer
    {
        bool Running { get; }
        void Start( Action<ISocketAdapter, Action<ISocketAdapter>> onConnection );
        void Stop();
    }
}