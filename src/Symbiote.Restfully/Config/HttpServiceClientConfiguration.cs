namespace Symbiote.JsonRpc.Config
{
    public class HttpServiceClientConfiguration : IHttpServiceClientConfiguration
    {
        public string ServerUrl { get; set; }
        public int Timeout { get; set; }
    }
}