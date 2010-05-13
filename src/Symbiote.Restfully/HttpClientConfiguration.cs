namespace Symbiote.Restfully
{
    public class HttpClientConfiguration : IHttpClientConfiguration
    {
        public string ServerUrl { get; set; }
        public int Timeout { get; set; }
    }
}