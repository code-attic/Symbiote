namespace Symbiote.JsonRpc.Config
{
    public interface IHttpServiceClientConfiguration
    {
        string ServerUrl { get; set; }
        int Timeout { get; set; }
    }
}