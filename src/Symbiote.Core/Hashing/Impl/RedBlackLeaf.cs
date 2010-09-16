using System;
using System.Collections;
using System.Globalization;

namespace Symbiote.Core.Hashing.Impl
{
    public class RedBlackLeaf<TKey, TValue> 
        : IRedBlackLeaf<TKey, TValue>
    {
        protected IRedBlackLeaf<TKey, TValue>[] _children = new IRedBlackLeaf<TKey, TValue>[2];
        protected Comparer Comparer { get; set; }

        public LeafColor Color { get; set; }
        public TKey Key { get; set; }
        public IRedBlackLeaf<TKey, TValue> Parent { get; set; }
        public TValue Value { get; set; }

        public IRedBlackLeaf<TKey, TValue> Right
        {
            get { return _children[1]; }
            set
            {
                if (value != null && !value.Key.Equals(Key))
                    _children[1] = value;
            }
        }
        public IRedBlackLeaf<TKey, TValue> Left
        {
            get { return _children[0]; }
            set
            {
                if (value != null && !value.Key.Equals(Key))
                    _children[0] = value;
            }
        }
        public IRedBlackLeaf<TKey, TValue> this[bool right]
        {
            get { return _children[right ? 1 : 0]; }
            set { _children[right ? 1 : 0] = value; }
        }
        public bool IsRoot { get { return Parent.IsEmpty(); } }
        public int Count { get { return GetCount(); } }

        protected int GetCount()
        {
            var left = Left.IsEmpty() ? 0 : Left.Count;
            var right = Right.IsEmpty() ? 0 : Right.Count;

            return 1 + left + right;
        }

        public IRedBlackLeaf<TKey, TValue> Seek(TKey key)
        {
            if (key.Equals(Key))
            {
                return this;
            }
            else if (LessThan(key))
            {
                return Right.IsEmpty() ? null : Right.Seek(key);
            }
            else
            {
                return Left.IsEmpty() ? null : Left.Seek(key);
            }
        }

        public TValue Get(TKey key)
        {
            var leaf = Seek(key);
            return leaf.IsEmpty() ? default(TValue) : leaf.Value;
        }

        public TValue Nearest<T>(T key)
        {
            var cast = (TKey) Convert.ChangeType(key, typeof(TKey));
            return NearestToKey(cast);
        }

        protected TValue NearestToKey(TKey key)
        {
            if (key.Equals(Key))
                return Value;
            else if (LessThan(key))
                return Right.IsEmpty() ? Value : Right.Nearest(key);
            else if (Comparer.Compare(key, Key) < 0)
                return Left.IsEmpty() ? Value : Left.Nearest(key);
            else
                return default(TValue);
        }

        public bool GreaterThan(TKey key)
        {
            return Comparer.Compare(Key, key) >= 1;
        }

        public bool LessThan(TKey key)
        {
            return Comparer.Compare(Key, key) < 0;
        }

        public RedBlackLeaf(TKey key, TValue value, RedBlackLeaf<TKey, TValue> parent)
        {
            Parent = parent;
            Key = key;
            Value = value;
            Color = LeafColor.RED;
            Comparer = new Comparer(CultureInfo.InvariantCulture);
        }
    }
}