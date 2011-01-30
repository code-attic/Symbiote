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

namespace Symbiote.Core.Collections
{
    public class MruNode<V>
    {
        public MruList<V> List { get; set; }
        public MruNode<V> Previous { get; set; }
        public MruNode<V> Next { get; set; }
        public V Value { get; set; }
        public Action OnRemoval { get; set; }

        public void Accessed()
        {
            if ( Previous == null )
                return;

            var parent = Previous;
            var child = Next;
            Next = List.Head;
            Previous = null;
            parent.Next = child;

            if ( child != null )
                child.Previous = parent;
        }

        public void Delete()
        {
            if ( Previous == null )
            {
                List.Head = Next;
                Erase();
                return;
            }

            if ( Next == null )
            {
                List.Tail = Previous;
                Erase();
                return;
            }

            Previous.Next = Next;
            Next.Previous = Previous;
            Erase();
        }

        public void Erase()
        {
            List = null;
            Next = null;
            Previous = null;
            Value = default(V);
            OnRemoval();
        }

        public static implicit operator V( MruNode<V> node )
        {
            node.Accessed();
            return node.Value;
        }

        public MruNode( V value, MruList<V> list )
        {
            Value = value;
            List = list;
        }

        public MruNode( MruList<V> list, MruNode<V> previous, V value )
        {
            List = list;
            Previous = previous;
            Value = value;
        }
    }
}