// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using Symbiote.Core.Hashing;

namespace Symbiote.Core.Trees
{
    public class HashedAvlLeaf<TKey, TValue>
        : IAvlLeaf<TKey, TValue>
    {
        protected static IHashingProvider HashProvider = new MD5HashProvider();
        protected HashedAvlLeaf<TKey, TValue>[] _children = new HashedAvlLeaf<TKey, TValue>[2];
        public long HashKey { get; set; }
        
        public int Balance { get; set; }
        public TKey Key { get; set; }
        public IAvlLeaf<TKey, TValue> Parent { get; set; }
        public TValue Value { get; set; }

        public IAvlLeaf<TKey, TValue> Right
        {
            get { return _children[1]; }
            set
            {
                var leaf = value as HashedAvlLeaf<TKey, TValue>;
                if ( leaf != null && leaf.HashKey != HashKey )
                    _children[1] = leaf;
            }
        }

        public IAvlLeaf<TKey, TValue> Left
        {
            get { return _children[0]; }
            set
            {
                var leaf = value as HashedAvlLeaf<TKey, TValue>;
                if ( leaf != null && leaf.HashKey != HashKey )
                    _children[0] = leaf;
            }
        }

        public IAvlLeaf<TKey, TValue> this[ bool right ]
        {
            get { return _children[right ? 1 : 0]; }
            set { _children[right ? 1 : 0] = value as HashedAvlLeaf<TKey, TValue>; }
        }

        public bool IsRoot
        {
            get { return Parent.IsEmpty(); }
        }

        public int Count
        {
            get { return GetCount(); }
        }

        public TValue Get( TKey key )
        {
            var hashKey = HashProvider.Hash( key );
            return Get( hashKey );
        }

        public bool GreaterThan( TKey key )
        {
            var hashKey = HashProvider.Hash( key );
            return HashKey > hashKey;
        }

        public bool LessThan( TKey key )
        {
            var hashKey = HashProvider.Hash( key );
            return HashKey < hashKey;
        }

        public IAvlLeaf<TKey, TValue> Seek( TKey key )
        {
            var hashKey = HashProvider.Hash( key );
            return SeekByHash( hashKey );
        }

        protected int GetCount()
        {
            var left = Left.IsEmpty() ? 0 : Left.Count;
            var right = Right.IsEmpty() ? 0 : Right.Count;

            return 1 + left + right;
        }

        public IAvlLeaf<TKey, TValue> SeekByHash( long key )
        {
            if ( key.Equals( HashKey ) )
            {
                return this;
            }
            else if ( key > HashKey )
            {
                return Right.IsEmpty() ? null : _children[1].SeekByHash( key );
            }
            else
            {
                return Left.IsEmpty() ? null : _children[0].SeekByHash( key );
            }
        }

        protected TValue Get( long key )
        {
            var leaf = SeekByHash( key );
            return leaf.IsEmpty() ? default(TValue) : leaf.Value;
        }
    }
}