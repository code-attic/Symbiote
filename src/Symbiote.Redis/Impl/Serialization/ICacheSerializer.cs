namespace Symbiote.Redis.Impl.Serialization
{
    public interface ICacheSerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] bytes);
    }
}