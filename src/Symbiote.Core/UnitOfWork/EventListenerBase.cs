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

namespace Symbiote.Core.UnitOfWork
{
    public abstract class EventListenerBase<T> : IEventListener<T> where T : IEvent
    {
        #region IEventListener<T> Members

        public bool ListenSubTypesOfEvent { get; private set; }

        public Type EventType
        {
            get { return typeof( T ); }
        }

        public void ListenTo( IEvent evnt )
        {
            try
            {
                OnNext( (T) evnt );
            }
            catch ( Exception exception )
            {
                OnError( exception );
            }
        }

        public abstract void OnCompleted();

        public abstract void OnError( Exception error );

        public abstract void OnNext( T value );

        #endregion

        protected EventListenerBase( bool listenToSubTypesOfEvent )
        {
            ListenSubTypesOfEvent = listenToSubTypesOfEvent;
        }

        protected EventListenerBase() : this( true )
        {
        }
    }
}