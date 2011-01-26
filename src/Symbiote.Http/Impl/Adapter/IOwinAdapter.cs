using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter
{
    public interface IOwinAdapter
    {
        IContext Process<T>( T context );
    }
}