using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Symbiote.Core.Utility
{
    public class Future<TValue>
        : IAsyncResult
    {
        protected AsyncCallback Callback { get; set; }
        protected ManualResetEvent ResetEvent { get; set; }

        public bool IsCompleted { get; protected set; }
        public WaitHandle AsyncWaitHandle { get { return ResetEvent; } }
        public object AsyncState { get; protected set; }
        public bool CompletedSynchronously { get; protected set; }
        public TValue Value { get; protected set; }

        public void Done(TValue value)
        {
            if(!IsCompleted)
            {
                Value = value;
                IsCompleted = true;
                ResetEvent.Set();
                if (Callback != null)
                    Callback( this );
            }
        }

        public bool WaitFor( TimeSpan timeout )
        {
            return ResetEvent.WaitOne( timeout );
        }

        public bool WaitFor(int miliseconds)
        {
            return ResetEvent.WaitOne( miliseconds );
        }

        public Future()
            : this (null, null)
        {}

        public Future(AsyncCallback callback, object asyncState)
        {
            Callback = callback;
            AsyncState = asyncState;
            ResetEvent = new ManualResetEvent(false);
        }
    }
}
