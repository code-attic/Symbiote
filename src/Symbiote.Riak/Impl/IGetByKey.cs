namespace Symbiote.Riak.Impl
{
    public interface IGetByKey
    {
        T Get<T>(string key);
    }
}