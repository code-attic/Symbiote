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

using System;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Actor.Impl.Eventing
{
    public class EventPublisher
        : IEventPublisher,
        IObservable<IEvent>
    {
        public IList<Tuple<Guid, IObserver<IEvent>>> Subscribers { get; set; }

        public IDisposable Subscribe( IObserver<IEvent> observer )
        {
            var id = Guid.NewGuid();
            Subscribers.Add( Tuple.Create( id, observer ) );
            return new EventSubscriptionToken( id, RemoveSubscriber );
        }

        public void RemoveSubscriber(Guid tokenId)
        {
            int index = 0;
            Subscribers.SkipWhile( x =>
            {
                index++;
                return x.Item1 != tokenId;
            } );
            Subscribers.RemoveAt( index - 1 );
        }

        public void PublishEvents( IEnumerable<IEvent> events )
        {
            events.ForEach( e => Subscribers.ForEach( x => x.Item2.OnNext( e ) ) );
        }
    }
}
