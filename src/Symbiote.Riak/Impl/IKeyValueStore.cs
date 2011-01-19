namespace Symbiote.Riak.Impl
{
    public interface IKeyValueStore :
        IDeleteByKey, IGetByKey, IGetAll
    {
        void Persist<T>(string key, T instance);
    }
}