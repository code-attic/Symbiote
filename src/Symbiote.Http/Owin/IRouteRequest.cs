namespace Symbiote.Http.Owin
{
    public interface IRouteRequest
    {
        IApplication GetApplicationFor(IRequest request);
    }
}
