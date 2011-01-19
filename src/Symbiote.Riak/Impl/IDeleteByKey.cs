namespace Symbiote.Riak.Impl
{
    public interface IDeleteByKey
    {
        void Delete<T>(string key);
    }
}