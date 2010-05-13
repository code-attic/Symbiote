using System;

namespace Symbiote.Restfully
{
    public interface IHttpServer : IDisposable
    {
        void Start();
        void Stop();
    }
}