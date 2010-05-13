namespace Symbiote.Restfully
{
    public interface IHttpClientConfiguration
    {
        string ServerUrl { get; set; }
        int Timeout { get; set; }
    }
}