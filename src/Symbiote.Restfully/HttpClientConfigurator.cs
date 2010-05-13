namespace Symbiote.Restfully
{
    public class HttpClientConfigurator
    {
        protected IHttpClientConfiguration Configuration { get; set; }

        public HttpClientConfigurator Server(string serverUrl)
        {
            Configuration.ServerUrl = serverUrl;
            return this;
        }

        public HttpClientConfigurator Timeout(int miliseconds)
        {
            Configuration.Timeout = miliseconds;
            return this;
        }

        public IHttpClientConfiguration GetConfiguration()
        {
            return Configuration;
        }

        public HttpClientConfigurator(IHttpClientConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}