using Symbiote.Daemon;
using Symbiote.Http;
using Symbiote.Http.Owin;

namespace HelloHttp
{
    public class HostService : IDaemon
    {
        public IServerAdapter Host { get; set; }

        public void Start()
        {
            Host.Start();
        }

        public void Stop()
        {
            Host.Stop();
        }

        public HostService(IServerAdapter host)
        {
            Host = host;
        }
    }
}