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
        bool GreaterThan(IRedBlackLeaf<TKey, TValue> leaf);
        bool LessThan(IRedBlackLeaf<TKey, TValue> leaf);

        IRedBlackLeaf<TKey, TValue> this[bool right] { get; set; }
    }
}