namespace Symbiote.JsonRpc.Client.Config
{
    public class JsonRpcClientConfigurator
    {
        protected IJsonRpcClientConfiguration Configuration { get; set; }

        public JsonRpcClientConfigurator Server(string serverUrl)
        {
            Configuration.ServerUrl = serverUrl;
            return this;
        }

        public JsonRpcClientConfigurator Timeout(int miliseconds)
        {
            Configuration.Timeout = miliseconds;
            return this;
        }

        public IJsonRpcClientConfiguration GetConfiguration()
        {
            return Configuration;
        }

        public JsonRpcClientConfigurator(IJsonRpcClientConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}