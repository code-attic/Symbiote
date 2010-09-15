namespace Symbiote.Core.Hashing
{
    public interface ITree<TKey, TValue>
    {
        int Count { get; }
        void Delete(TKey key);
        TValue Get(TKey key);
        void Add(TKey key, TValue value);
    }
}