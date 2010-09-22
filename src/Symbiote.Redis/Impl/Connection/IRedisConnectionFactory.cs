namespace Symbiote.Redis.Impl.Connection
{
    public interface IRedisConnectionFactory
    {
        IRedisConnection GetConnection();
    }
}