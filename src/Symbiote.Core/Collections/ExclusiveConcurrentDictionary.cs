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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Collections
{
    public class ExclusiveConcurrentDictionary<TKey, TValue>
    {
        protected ReaderWriterLockSlim SlimLock { get; set; }
        protected ConcurrentDictionary<TKey, TValue> Dictionary { get; set; }
        public int MostWaiting { get; set; } // for diagnostic purposes

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public IEnumerable<TValue> Values
        {
            get { return Dictionary.Values; }
        }

        public TValue this[ TKey key ]
        {
            get { return GetOrDefault( key ); }
            set
            {
                try
                {
                    Dictionary[key] = value;
                    //Dictionary.AddOrUpdate( key, value, ( x, y ) => value );
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e );
                }
            }
        }

        public TValue GetOrDefault( TKey key )
        {
            return Dictionary.GetOrDefault( key );
        }

        public TValue ReadOrWrite( TKey key, Func<TValue> valueProvider )
        {
            TValue value = default(TValue);
            if ( !Dictionary.TryGetValue( key, out value ) )
            {
                try
                {
                    SlimLock.EnterWriteLock();
                    UpdateWaiting();
                    if ( !Dictionary.TryGetValue( key, out value ) )
                        value = Dictionary.GetOrAdd( key, valueProvider() );
                }
                finally
                {
                    SlimLock.ExitWriteLock();
                }
            }
            return value;
        }

        public void UpdateWaiting()
        {
            var waiting = SlimLock.WaitingWriteCount;
            MostWaiting = MostWaiting > waiting
                              ? MostWaiting
                              : waiting;
        }

        public ExclusiveConcurrentDictionary()
        {
            SlimLock = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

            Dictionary = new ConcurrentDictionary<TKey, TValue>();
        }
    }
}