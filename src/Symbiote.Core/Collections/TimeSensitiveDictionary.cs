using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                Head = node;
                ItemCount++;

                if(ItemCount > Limit)
                {
                    var deleted = Tail;
                    Tail = Tail.Previous;
                    deleted.Erase();
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
                while(node != null)
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

        public virtual int Count
        {
            get { return ItemCount; }
        }

        public MruList(int limit)
        {
            Limit = limit;
            SlimLock = new ReaderWriterLockSlim();
        }

        public struct Enumerator: IEnumerator<MruNode<V>>
        {
            private MruList<V> list;
            private MruNode<V> current;
            
            public void Dispose()
            {
                list = null;
                current = null;
            }

            public bool MoveNext()
            {
                var can = current.Next.Equals( default( V ) );
                if (can)
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

    public class MruNode<V>
    {
        public MruList<V> List { get; set; }
        public MruNode<V> Previous { get; set; }
        public MruNode<V> Next { get; set; }
        public V Value { get; set; }

        public void Accessed()
        {
            if (this.Previous == null)
                return;

            var parent = Previous;
            var child = Next;
            Next = List.Head;
            Previous = null;
            parent.Next = child;
            
            if(child != null)
                child.Previous = parent;
        }

        public void Delete()
        {
            if (this.Previous == null)
            {
                List.Head = this.Next;
                Erase();
                return;
            }

            if(this.Next == null)
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
        }

        public static implicit operator V(MruNode<V> node)
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

    public class MruDictionary<K, V>
        : IDisposable
    {
        protected MruList<V> MostRecentlyUsed { get; set; }
        protected ConcurrentDictionary<K, MruNode<V>> Dictionary { get; set; }

        public MruDictionary(  ) : this( 1024 )
        {   
        }

        public MruDictionary( int limit)
        {
            MostRecentlyUsed = new MruList<V>( limit );
            Dictionary = new ConcurrentDictionary<K, MruNode<V>>();
        }

        public void Clear()
        {
            Dictionary.Clear();
            MostRecentlyUsed.Clear();
        }

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public bool ContainsKey( K key )
        {
            return Dictionary.ContainsKey( key );
        }

        public void Add( K key, V value )
        {
            Dictionary.TryAdd(key, MostRecentlyUsed.Add( value ));
        }

        public bool Remove( K key )
        {
            MruNode<V> handle;
            var success = Dictionary.TryRemove( key, out handle );
            if(success)
                handle.Delete();
            return success;
        }

        public bool TryGetValue( K key, out V value )
        {
            MruNode<V> output;
            var success = Dictionary.TryGetValue( key, out output );
            value = success
                        ? output
                        : default( V );
            return success;
        }

        public V this[ K key ]
        {
            get { return Dictionary[key]; }
            set
            {
                Add( key, value );
            }
        }

        public ICollection<K> Keys
        {
            get { return Dictionary.Keys; }
        }

        public ICollection<V> Values
        {
            get { return Dictionary.Values.Cast<V>().ToList(); }
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
