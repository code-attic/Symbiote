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
using Symbiote.Core;

namespace Symbiote.Actor.Impl.Eventing
{
    public class EventContext<TActor>
        : IEventContext
        where TActor : class
    {
        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }
        public IKeyAccessor<TActor> KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }
        public IList<IEvent> Events { get; set; }

        public void Dispose()
        {
            Commit();
        }

        public void Commit()
        {
            Publisher.PublishEvents( Events );
        }

        public void Publish<TEvent>( Action<TEvent> populateEvent )
        {
            var newEvent = Assimilate.GetInstanceOf<TEvent>();
            var baseEvent = newEvent as IEvent;
            PopulateDefaultEventFields (baseEvent);
            populateEvent( newEvent );
            Events.Add( baseEvent );
        }

        protected void PopulateDefaultEventFields( IEvent baseEvent ) 
        {
            baseEvent.ActorId = KeyAccessor.GetId( Actor );
            baseEvent.ActorType = Actor.GetType().FullName;
            baseEvent.UtcTimeStamp = DateTime.UtcNow;
        }

        public void Rollback()
        {
            OriginalState.Reset( Actor );
        }

        public EventContext( 
            TActor actor, 
            IMemento<TActor> originalState, 
            IKeyAccessor<TActor> keyAccessor,
            IEventPublisher eventPublisher )
        {
            Actor = actor;
            OriginalState = originalState;
            Events = new List<IEvent>();
            KeyAccessor = keyAccessor;
            Publisher = eventPublisher;
        }
    }
}