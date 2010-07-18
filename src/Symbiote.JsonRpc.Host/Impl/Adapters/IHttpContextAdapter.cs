namespace Symbiote.JsonRpc.Host.Impl.Adapters
{
    public interface IHttpContextAdapter
    {
        IHttpRequestAdapter Request { get; }
        IHttpResponseAdapter Response { get; }
    }
}