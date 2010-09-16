namespace Symbiote.Core.Hashing.Impl
{
    public class HashedRedBlackLeaf<TKey, TValue>
        : IRedBlackLeaf<TKey, TValue>
    {
        protected HashedRedBlackLeaf<TKey, TValue>[] _children = new HashedRedBlackLeaf<TKey, TValue>[2];
        protected static IHashingProvider HashProvider = new MD5HashProvider();

        public LeafColor Color { get; set; }
        public int HashKey { get; set; }
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

        public TValue Get(TKey key)
        {
            var hashKey = HashProvider.Hash(key);
            return Get(hashKey);
        }

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

        public TValue Nearest<T>(T key)
        {
            var hashKey = HashProvider.Hash(key);
            return Nearest(hashKey);
        }

        public IRedBlackLeaf<TKey, TValue> Seek(TKey key)
        {
            var hashKey = HashProvider.Hash(key);
            return SeekByHash(hashKey);
        }

        protected int GetCount()
        {
            var left = Left.IsEmpty() ? 0 : Left.Count;
            var right = Right.IsEmpty() ? 0 : Right.Count;

            return 1 + left + right;
        }

        public IRedBlackLeaf<TKey, TValue> SeekByHash(int key)
        {
            if (key.Equals(HashKey))
            {
                return this;
            }
            else if (key > HashKey)
            {
                return Right.IsEmpty() ? null : _children[1].SeekByHash(key);
            }
            else
            {
                return Left.IsEmpty() ? null : _children[0].SeekByHash(key);
            }
        }

        protected TValue Get(int key)
        {
            var leaf = SeekByHash(key);
            return leaf.IsEmpty() ? default(TValue) : leaf.Value;
        }

        protected TValue Nearest(int key)
        {
            if (key.Equals(HashKey))
                return Value;
            else if (key > HashKey)
                return Right.IsEmpty() ? Value : Right.Nearest(key);
            else if (key < HashKey)
                return Left.IsEmpty() ? Value : Left.Nearest(key);
            else
                return default(TValue);
        }

        public HashedRedBlackLeaf()
        {
            Color = LeafColor.RED;
        }
    }
}