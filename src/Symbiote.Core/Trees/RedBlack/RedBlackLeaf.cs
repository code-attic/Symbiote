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
using System.Diagnostics;
using System.Globalization;

namespace Symbiote.Core.Trees
{
    [DebuggerDisplay( "{Key} - {Color}" )]
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
                if ( value != null && !value.Key.Equals( Key ) )
                    _children[1] = value;
            }
        }

        public IRedBlackLeaf<TKey, TValue> Left
        {
            get { return _children[0]; }
            set
            {
                if ( value != null && !value.Key.Equals( Key ) )
                    _children[0] = value;
            }
        }

        public IRedBlackLeaf<TKey, TValue> this[ bool right ]
        {
            get { return _children[right ? 1 : 0]; }
            set { _children[right ? 1 : 0] = value; }
        }

        public bool IsRoot
        {
            get { return Parent.IsEmpty(); }
        }

        public int Count
        {
            get { return GetCount(); }
        }

        public bool GreaterThan( IRedBlackLeaf<TKey, TValue> leaf )
        {
            return GreaterThan( leaf.Key );
        }

        public bool LessThan( IRedBlackLeaf<TKey, TValue> leaf )
        {
            return LessThan( leaf.Key );
        }

        protected int GetCount()
        {
            var left = Left.IsEmpty() ? 0 : Left.Count;
            var right = Right.IsEmpty() ? 0 : Right.Count;

            return 1 + left + right;
        }

        public bool GreaterThan( TKey key )
        {
            return Comparer.Compare( Key, key ) >= 1;
        }

        public bool LessThan( TKey key )
        {
            return Comparer.Compare( Key, key ) < 0;
        }

        public RedBlackLeaf( TKey key, TValue value, RedBlackLeaf<TKey, TValue> parent )
        {
            Parent = parent;
            Key = key;
            Value = value;
            Color = LeafColor.RED;
            Comparer = new Comparer( CultureInfo.InvariantCulture );
        }
    }
}