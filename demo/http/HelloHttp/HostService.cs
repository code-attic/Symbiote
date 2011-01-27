using Symbiote.Daemon;
using Symbiote.Http.Owin;

namespace HelloHttp
{
    public class HostService : IDaemon
    {
        public IHost Host { get; set; }

        public void Start()
        {
            Host.Start();
        }

        public void Stop()
        {
            Host.Stop();
        }

        public HostService(IHost host)
        {
            Host = host;
        }
    }
}