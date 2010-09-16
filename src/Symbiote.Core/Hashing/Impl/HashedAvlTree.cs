namespace Symbiote.Core.Hashing.Impl
{
    public class HashedAvlTree<TKey, TValue> 
        : AvlTreeBase<TKey, TValue>
    {
        protected IHashingProvider HashProvider { get; set; }

        protected override IAvlLeaf<TKey, TValue> CreateLeaf(TKey key, TValue value)
        {
            return new HashedAvlLeaf<TKey, TValue>()
                       {
                           Key = key,
                           Value = value,
                           HashKey = HashProvider.Hash(key),
                       };
        }

        public HashedAvlTree()
        {
            HashProvider = new MD5HashProvider();
        }
    }
}