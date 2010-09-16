namespace Symbiote.Core.Hashing.Impl
{
    public interface IAvlLeaf<TKey, TValue>
    {
        TKey Key { get; set; }
        IAvlLeaf<TKey, TValue> Parent { get; set; }
        TValue Value { get; set; }
        IAvlLeaf<TKey, TValue> Right { get; set; }
        IAvlLeaf<TKey, TValue> Left { get; set; }
        bool IsRoot { get; }
        int Count { get; }
        int Balance { get; set; }
        TValue Get(TKey key);
        bool GreaterThan(TKey key);
        bool LessThan(TKey key);
        TValue Nearest<T>(T key);
        IAvlLeaf<TKey, TValue> Seek(TKey key);

        IAvlLeaf<TKey, TValue> this[bool right] { get; set; }
    }
}