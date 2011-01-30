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
using System.Collections.Generic;
using System.Threading;

namespace Symbiote.Core.Collections
{
    public class MruList<V>
        : IEnumerable<MruNode<V>>
    {
        public ReaderWriterLockSlim SlimLock { get; set; }
        public MruNode<V> Head { get; set; }
        public MruNode<V> Tail { get; set; }
        public int Limit { get; set; }
        protected int ItemCount { get; set; }

        public virtual int Count
        {
            get { return ItemCount; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<MruNode<V>> GetEnumerator()
        {
            return new Enumerator( this );
        }

        public virtual MruNode<V> Add( V item )
        {
            var node = new MruNode<V>( item, this );
            try
            {
                SlimLock.EnterWriteLock();
                node.Next = Head;
                Tail = Tail ?? node;
                if(Head != null)
                    Head.Previous = node;
                Head = node;
                ItemCount++;

                if ( ItemCount > Limit )
                {
                    var deleted = Tail;
                    if(Tail != null)
                    {
                        Tail = Tail.Previous;
                        deleted.Erase();
                    }
                    ItemCount--;
                }
            }
            finally
            {
                SlimLock.ExitWriteLock();
            }
            return node;
        }

        public virtual void Clear()
        {
            try
            {
                SlimLock.EnterWriteLock();
                var node = Head;
                while ( node != null )
                {
                    var old = node;
                    node = node.Next;
                    old.Erase();
                }
                ItemCount = 0;
                Head = null;
                Tail = null;
            }
            finally
            {
                SlimLock.ExitWriteLock();
            }
        }

        public MruList( int limit )
        {
            Limit = limit;
            SlimLock = new ReaderWriterLockSlim();
        }

        public struct Enumerator : IEnumerator<MruNode<V>>
        {
            private MruNode<V> current;
            private MruList<V> list;

            public void Dispose()
            {
                list = null;
                current = null;
            }

            public bool MoveNext()
            {
                var can = current.Next.Equals( default(V) );
                if ( can )
                    current = current.Next;
                return can;
            }

            public void Reset()
            {
                current = list.Head;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public MruNode<V> Current
            {
                get { return current; }
            }

            public Enumerator( MruList<V> list ) : this()
            {
                this.list = list;
            }
        }
    }
}