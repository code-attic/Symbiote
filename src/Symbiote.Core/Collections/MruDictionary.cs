﻿// /* 
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Symbiote.Core.Collections
{
    public class MruDictionary<K, V>
        : IDisposable
    {
        protected MruList<V> MostRecentlyUsed { get; set; }
        protected ConcurrentDictionary<K, MruNode<V>> Dictionary { get; set; }

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public V this[ K key ]
        {
            get { return Dictionary[key]; }
            set { Add( key, value ); }
        }

        public ICollection<K> Keys
        {
            get { return Dictionary.Keys; }
        }

        public ICollection<V> Values
        {
            get { return Dictionary.Values.Select( x => (V) x ).ToList(); }
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            Dictionary.Clear();
            MostRecentlyUsed.Clear();
        }

        public bool ContainsKey( K key )
        {
            return Dictionary.ContainsKey( key );
        }

        public void Add( K key, V value )
        {
            var mruNode = MostRecentlyUsed.Add( value );
            mruNode.OnRemoval = () =>
                                    {
                                        MruNode<V> node;
                                        Dictionary.TryRemove( key, out node );
                                    };
            Dictionary.TryAdd( key, mruNode );
        }

        public bool Remove( K key )
        {
            MruNode<V> handle;
            var success = Dictionary.TryRemove( key, out handle );
            if ( success )
                handle.Delete();
            return success;
        }

        public bool TryGetValue( K key, out V value )
        {
            MruNode<V> output;
            var success = Dictionary.TryGetValue( key, out output );
            value = success
                        ? output
                        : default(V);
            return success;
        }

        public MruDictionary() : this( 1024 )
        {
        }

        public MruDictionary( int limit )
        {
            MostRecentlyUsed = new MruList<V>( limit );
            Dictionary = new ConcurrentDictionary<K, MruNode<V>>();
        }
    }
}