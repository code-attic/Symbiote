namespace Symbiote.Http.Impl.Adapter
{
    public interface IContextTransformer
    {
        Context From<T>( T context );
    }
}