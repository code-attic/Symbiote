namespace Symbiote.JsonRpc.Client.Config
{
    public interface IJsonRpcClientConfiguration
    {
        string ServerUrl { get; set; }
        int Timeout { get; set; }
    }
}