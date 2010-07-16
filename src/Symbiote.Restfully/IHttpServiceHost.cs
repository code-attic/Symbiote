using System;

namespace Symbiote.JsonRpc
{
    public interface IHttpServiceHost : IDisposable
    {
        void Start();
        void Stop();
    }
}