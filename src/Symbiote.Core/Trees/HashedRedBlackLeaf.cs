/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Symbiote.Core.Hashing;

namespace Symbiote.Core.Trees
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