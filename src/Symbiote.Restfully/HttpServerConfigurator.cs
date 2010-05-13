namespace Symbiote.Restfully
{
    public class HttpServerConfigurator
    {
        private IHttpServerConfiguration _configuration;

        public HttpServerConfigurator Port(int port)
        {
            _configuration.Port = port;
            return this;
        }

        public HttpServerConfigurator UseDefaults()
        {
            _configuration.UseDefaults();
            return this;
        }

        public IHttpServerConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public HttpServerConfigurator(IHttpServerConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}