namespace Symbiote.Core.Hashing.Impl
{
    public class HashedRedBlackTree<TKey, TValue>
        : RedBlackTreeBase<TKey, TValue>, ITree<TKey, TValue>
    {
        protected IHashingProvider HashProvider { get; set; }

        protected override IRedBlackLeaf<TKey, TValue> CreateLeaf(TKey key, TValue value)
        {
            var newLeaf = new HashedRedBlackLeaf<TKey, TValue>()
                        {
                            HashKey = HashProvider.Hash(key),
                            Key = key,
                            Value = value,
                        };

            return newLeaf;
        }

        public HashedRedBlackTree() : base()
        {
            HashProvider = new MD5HashProvider();
        }
    }
}