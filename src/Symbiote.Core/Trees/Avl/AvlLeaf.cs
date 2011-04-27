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
using System.Collections;
using System.Globalization;

namespace Symbiote.Core.Trees
{
    public class AvlLeaf<TKey, TValue>
        : IAvlLeaf<TKey, TValue>
    {
        protected AvlLeaf<TKey, TValue>[] _children = new AvlLeaf<TKey, TValue>[2];
        protected Comparer Comparer { get; set; }
        public int Balance { get; set; }
        public TKey Key { get; set; }
        public IAvlLeaf<TKey, TValue> Parent { get; set; }
        public TValue Value { get; set; }

        public IAvlLeaf<TKey, TValue> Right
        {
            get { return _children[1]; }
            set
            {
                var leaf = value as AvlLeaf<TKey, TValue>;
                if ( leaf != null && !leaf.Key.Equals( Key ) )
                    _children[1] = leaf;
            }
        }

        public IAvlLeaf<TKey, TValue> Left
        {
            get { return _children[0]; }
            set
            {
                var leaf = value as AvlLeaf<TKey, TValue>;
                if ( leaf != null && !leaf.Key.Equals( Key ) )
                    _children[0] = leaf;
            }
        }

        public bool IsRoot
        {
            get { return Parent.IsEmpty(); }
        }

        public int Count
        {
            get { return GetCount(); }
        }

        public IAvlLeaf<TKey, TValue> this[ bool right ]
        {
            get { return _children[right ? 1 : 0]; }
            set { _children[right ? 1 : 0] = value as AvlLeaf<TKey, TValue>; }
        }

        public TValue Get( TKey key )
        {
            var leaf = Seek( key );
            return leaf.IsEmpty() ? default(TValue) : leaf.Value;
        }

        protected int GetCount()
        {
            var left = Left.IsEmpty() ? 0 : Left.Count;
            var right = Right.IsEmpty() ? 0 : Right.Count;

            return 1 + left + right;
        }

        public bool GreaterThan( TKey key )
        {
            return Comparer.Compare( Key, key) > 0;
        }

        public bool LessThan( TKey key )
        {
            return Comparer.Compare( Key, key) < 0;
        }

        public IAvlLeaf<TKey, TValue> Seek( TKey key )
        {
            if ( key.Equals( Key ) )
            {
                return this;
            }
            else if ( LessThan( key ) )
            {
                return Right.IsEmpty() ? null : _children[1].Seek( key );
            }
            else
            {
                return Left.IsEmpty() ? null : _children[0].Seek( key );
            }
        }

        public AvlLeaf( TKey key, TValue value, IAvlLeaf<TKey, TValue> parent )
        {
            Key = key;
            Parent = parent;
            Value = value;
            Comparer = new Comparer( CultureInfo.InvariantCulture );
        }
    }
}