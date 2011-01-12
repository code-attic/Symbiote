namespace Symbiote.Redis.Impl.Connection
{
    public interface IConnectionPool
    {
        IConnection Acquire();
        void Release(IConnection connection);
    }
}