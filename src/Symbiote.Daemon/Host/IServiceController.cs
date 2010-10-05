using System;

namespace Symbiote.Daemon.Host
{
    public interface IServiceController :
        IDisposable
    {
        Type ServiceType { get; }
        string Name { get; }
        ServiceState State { get; }

        void Initialize();
        void Start();
        void Stop();
        void Pause();
        void Continue();
    }
}