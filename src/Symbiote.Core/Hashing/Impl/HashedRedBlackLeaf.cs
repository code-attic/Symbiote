using System;

namespace Symbiote.Core.Hashing.Impl
{
    public class HashedRedBlackLeaf<TKey, TValue>
        : IRedBlackLeaf<TKey, TValue>
    {
        protected HashedRedBlackLeaf<TKey, TValue>[] _children = new HashedRedBlackLeaf<TKey, TValue>[2];
        protected static IHashingProvider HashProvider = new MD5HashProvider();

        public LeafColor Color { get; set; }
        public long HashKey { get; set; }
        public TKey Key { get; set; }
        public IRedBlackLeaf<TKey, TValue> Parent { get; set; }
        public TValue Value { get; set; }
        public IRedBlackLeaf<TKey, TValue> Right
        {
            get { return _children[1]; }
            set
            {
                var leaf = value as HashedRedBlackLeaf<TKey, TValue>;
                if (leaf != null && leaf.HashKey != HashKey)
                    _children[1] = leaf;
            }
        }
        public IRedBlackLeaf<TKey, TValue> Left
        {
            get { return _children[0]; }
            set
            {
                var leaf = value as HashedRedBlackLeaf<TKey, TValue>;
                if (leaf != null && leaf.HashKey != HashKey)
                    _children[0] = leaf;
            }
        }
        public IRedBlackLeaf<TKey, TValue> this[bool right]
        {
            get { return _children[right ? 1 : 0]; }
            set { _children[right ? 1 : 0] = value as HashedRedBlackLeaf<TKey, TValue>; }
        }

        public bool IsRoot { get { return Parent.IsEmpty(); } }
        public int Count { get { return GetCount(); } }

        public bool GreaterThan(TKey key)
        {
            var hashKey = HashProvider.Hash(key);
            return HashKey > hashKey;
        }

        public bool LessThan(TKey key)
        {
            var hashKey = HashProvider.Hash(key);
            return HashKey < hashKey;
        }

        public bool GreaterThan(IRedBlackLeaf<TKey, TValue> leaf)
        {
            var compare = leaf as HashedRedBlackLeaf<TKey, TValue>;
            return compare == null ? true : HashKey > compare.HashKey;
        }

        public bool LessThan(IRedBlackLeaf<TKey, TValue> leaf)
        {
            var compare = leaf as HashedRedBlackLeaf<TKey, TValue>;
            return compare == null ? true : HashKey < compare.HashKey;
        }

        protected int GetCount()
        {
            var left = Left.IsEmpty() ? 0 : Left.Count;
            var right = Right.IsEmpty() ? 0 : Right.Count;

            return 1 + left + right;
        }

        public HashedRedBlackLeaf()
        {
            Color = LeafColor.RED;
        }
    }
}