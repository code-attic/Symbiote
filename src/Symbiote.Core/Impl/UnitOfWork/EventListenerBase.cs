using System;

namespace Symbiote.Core.Impl.UnitOfWork
{
    public abstract class EventListenerBase<T> : IEventListener<T> where T : IEvent
    {
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