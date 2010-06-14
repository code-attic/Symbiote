using System;

namespace Symbiote.Restfully
{
    public interface IHttpServiceHost : IDisposable
    {
        void Start();
        void Stop();
    }
}