namespace Symbiote.Restfully.Impl
{
    public interface IRemoteProcedure
    {
        string Contract { get; }
        string Method { get; }
        object Invoke();
        string JsonExpressionTree { set; }
    }
}