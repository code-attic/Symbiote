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
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Utility
{
    public class ObserverCollection<TEvent>
    {
        protected ConcurrentDictionary<Guid, IObserver<TEvent>> Observers { get; set; }

        public int Count { get { return Observers.Count; } }

            public IDisposable AddObserver( IObserver<TEvent> observer )
        {
            var token = new ObserverToken( RemoveObserver );
            Observers.TryAdd( token.Id, observer );
            return token;
        }
        
        public void OnEvent( TEvent e )
        {
            Notify( x => x.OnNext( e ) );
        }

        public void OnError( Exception e )
        {
            Notify( x => x.OnError( e ) );
        }

        public void OnComplete()
        {
            Notify( x => x.OnCompleted() );
        }

        public void Notify( Action<IObserver<TEvent>> action )
        {
            Observers.Values.ForEach( action );
        }

        public void RemoveObserver( Guid id )
        {
            IObserver<TEvent> observer;
            Observers.TryRemove( id, out observer );
        }

        public ObserverCollection()
        {
            Observers = new ConcurrentDictionary<Guid, IObserver<TEvent>>();
        }
    }
}