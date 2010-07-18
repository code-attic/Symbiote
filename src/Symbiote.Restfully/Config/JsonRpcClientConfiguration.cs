namespace Symbiote.JsonRpc.Client.Config
{
    public class JsonRpcClientConfiguration : IJsonRpcClientConfiguration
    {
        public string ServerUrl { get; set; }
        public int Timeout { get; set; }
    }
}