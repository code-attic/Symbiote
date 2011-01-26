using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter
{
    public class Context : IContext
    {
        public IRequest Request { get; set; }
        public IResponseAdapter Response { get; set; }

        public Context( IRequest request, IResponseAdapter response )
        {
            Request = request;
            Response = response;
        }
    }
}