namespace Symbiote.Core.Hashing
{
    public static class LeafExtensions
    {
        public static bool IsEmpty<TKey,TValue>(this IRedBlackLeaf<TKey, TValue> leaf)
        {
            return leaf == null || leaf.Value.Equals(default(TValue));
        }

        public static bool IsRed<TKey, TValue>(this IRedBlackLeaf<TKey, TValue> leaf)
        {
            return !leaf.IsEmpty() && leaf.Color == LeafColor.RED;
        }

        public static bool IsBlack<TKey, TValue>(this IRedBlackLeaf<TKey, TValue> leaf)
        {
            return !leaf.IsEmpty() && leaf.Color == LeafColor.BLACK;
        }
    }
}