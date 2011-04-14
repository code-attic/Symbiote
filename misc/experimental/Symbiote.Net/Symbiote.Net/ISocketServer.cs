using System;

namespace Symbiote.Net
{
    public interface ISocketServer
    {
        bool Running { get; }
        void Start( Action<ISocket> onConnection );
        void Stop();
    }
}