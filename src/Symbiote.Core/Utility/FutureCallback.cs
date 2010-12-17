using System;
using System.Threading;

namespace Symbiote.Core.Utility
{
    public class FutureCallback<T>
    {
        protected IAsyncResult AsyncResult { get; set; }
        protected int Attempts { get; set; }
        protected Action<Action<T>> Call { get; set; }
        protected Action<T> Coroutine { get; set; }
        protected Func<T> GetResult { get; set; }
        protected bool HasResult { get; set; }
        protected int Limit { get; set; }
        protected T Result { get; set; }
        protected TimeSpan Timeout { get; set; }
        protected TimeSpan TimeBetweenTries { get; set; }

        protected T Value
        {
            get
            {
                while(Attempts < Limit && !HasResult)
                {
                    AsyncResult = Call.BeginInvoke(Callback, null, null);
                    
                    if (!AsyncResult.IsCompleted)
                    {
                        if(AsyncResult.AsyncWaitHandle.WaitOne( Timeout ))
                        {
                            Result = GetResult();
                            Call.EndInvoke( AsyncResult );
                        }
                        else
                        {
                            AsyncResult.AsyncWaitHandle.Close();
                            AsyncResult.AsyncWaitHandle.Dispose();
                            AsyncResult.AsyncWaitHandle.SafeWaitHandle.SetHandleAsInvalid();
                        }
                    }
                    else
                    {
                        Result = GetResult();
                        Call.EndInvoke(AsyncResult);
                    }
                    Attempts++;
                    Thread.Sleep( TimeBetweenTries );
                }
                if (Coroutine != null && HasResult)
                    Coroutine(Result);
                return Result;
            }
        }

        public FutureCallback<T> MaxRetries(int retries)
        {
            Limit = retries;
            return this;
        }

        public FutureCallback<T> OnValue(Action<T> handle)
        {
            Coroutine = handle;
            if (HasResult)
                handle(Value);
            return this;
        }

        public FutureCallback<T> TimeBetweenRetries(int miliseconds)
        {
            TimeBetweenTries = TimeSpan.FromMilliseconds(miliseconds);
            return this;
        }

        public FutureCallback<T> TimeBetweenRetries(TimeSpan span)
        {
            TimeBetweenTries = span;
            return this;
        }

        public FutureCallback<T> WaitFor(TimeSpan span)
        {
            Timeout = span;
            return this;
        }

        public FutureCallback<T> WaitFor(int miliseconds)
        {
            Timeout = TimeSpan.FromMilliseconds(miliseconds);
            return this;
        }

        public FutureCallback(Action<Action<T>> call)
        {
            Limit = 1;
            TimeBetweenTries = TimeSpan.Zero;
            Timeout = TimeSpan.FromMilliseconds(-1);
            Call = call;
            GetResult = () => default(T);
        }

        public void Callback( T value ) 
        {
            GetResult = () => value;
            ((ManualResetEvent) AsyncResult.AsyncWaitHandle).Set();
            HasResult = true;
        }

        public static implicit operator T(FutureCallback<T> future)
        {
            return future.Value;
        }

        public static implicit operator Action<T>(FutureCallback<T> future)
        {
            Action<T> onValue = x =>
            {
                future.GetResult = () => x;
                ((ManualResetEvent)future.AsyncResult.AsyncWaitHandle).Set();
            };
            return onValue;
        }
    }
}