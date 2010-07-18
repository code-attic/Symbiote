using System;

namespace Symbiote.JsonRpc.Host
{
    public interface IJsonRpcHost : IDisposable
    {
        void Start();
        void Stop();
    }
}