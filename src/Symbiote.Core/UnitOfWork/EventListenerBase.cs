using System;

namespace Symbiote.Core.UnitOfWork
{
    public abstract class EventListenerBase<T> : IEventListener<T> where T : IEvent
    {
        protected EventListenerBase(bool listenToSubTypesOfEvent)
        {
            ListenSubTypesOfEvent = listenToSubTypesOfEvent;
        }

        protected EventListenerBase() : this( true )
        {
            
        }

        public bool ListenSubTypesOfEvent { get; private set; }

        public Type EventType
        {
            get { return typeof(T); }
        }

        public void ListenTo(IEvent evnt)
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
    }
}