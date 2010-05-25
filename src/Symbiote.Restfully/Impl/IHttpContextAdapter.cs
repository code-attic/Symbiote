namespace Symbiote.Restfully.Impl
{
    public interface IHttpContextAdapter
    {
        IHttpRequestAdapter Request { get; }
        IHttpResponseAdapter Response { get; }
    }
}