namespace Symbiote.Restfully.Impl.Adapters
{
    public interface IHttpContextAdapter
    {
        IHttpRequestAdapter Request { get; }
        IHttpResponseAdapter Response { get; }
    }
}