using System;

namespace Symbiote.WebSocket.Impl
{
    public interface IHandlePolicyRequests: IDisposable
    {
        void Start();
        void Stop();
    }
}