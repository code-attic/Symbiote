using System;
using System.Collections.Generic;

namespace Symbiote.Daemon.Host
{
    public interface IServiceCoordinator :
        IDisposable
    {
        void Start();
        void Stop();
        void Pause();
        void Continue();

        void StartService(string name);
        void StopService(string name);
        void PauseService(string name);
        void ContinueService(string name);

        int HostedServiceCount { get; }
        IServiceController GetService(string s);
        IList<ServiceInformation> GetServiceInfo();
    }
}