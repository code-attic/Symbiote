using System;

namespace Symbiote.Restfully.Impl
{
    public interface IHttpServer : IDisposable
    {
        void Start();
        void Stop();
    }
}