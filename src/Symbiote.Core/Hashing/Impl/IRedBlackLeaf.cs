namespace Symbiote.Core.Hashing.Impl
{
    public interface IRedBlackLeaf<TKey, TValue>
    {
        LeafColor Color { get; set; }
        TKey Key { get; set; }
        IRedBlackLeaf<TKey, TValue> Parent { get; set; }
        TValue Value { get; set; }
        IRedBlackLeaf<TKey, TValue> Right { get; set; }
        IRedBlackLeaf<TKey, TValue> Left { get; set; }
        bool IsRoot { get; }
        int Count { get; }

        TValue Get(TKey key);
        bool GreaterThan(TKey key);
        bool LessThan(TKey key);
        TValue Nearest<T>(T key);
        IRedBlackLeaf<TKey, TValue> Seek(TKey key);

        IRedBlackLeaf<TKey, TValue> this[bool right] { get; set; }
    }
}