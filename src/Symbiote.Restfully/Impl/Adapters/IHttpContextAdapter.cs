namespace Symbiote.JsonRpc.Impl.Adapters
{
    public interface IHttpContextAdapter
    {
        IHttpRequestAdapter Request { get; }
        IHttpResponseAdapter Response { get; }
    }
}