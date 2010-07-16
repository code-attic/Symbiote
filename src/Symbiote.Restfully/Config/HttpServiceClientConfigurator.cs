namespace Symbiote.JsonRpc.Config
{
    public class HttpServiceClientConfigurator
    {
        protected IHttpServiceClientConfiguration Configuration { get; set; }

        public HttpServiceClientConfigurator Server(string serverUrl)
        {
            Configuration.ServerUrl = serverUrl;
            return this;
        }

        public HttpServiceClientConfigurator Timeout(int miliseconds)
        {
            Configuration.Timeout = miliseconds;
            return this;
        }

        public IHttpServiceClientConfiguration GetConfiguration()
        {
            return Configuration;
        }

        public HttpServiceClientConfigurator(IHttpServiceClientConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}