using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter
{
    public interface IRouteRequest
    {
        IApplication GetApplicationFor(IRequest request);
    }
}
