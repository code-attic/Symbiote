using System;
using System.Threading;

namespace Symbiote.Core.Utility
{
    public class FutureResult<T>
    {
        protected IAsyncResult AsyncResult { get; set; }
        protected int Attempts { get; set; }
        protected Func<T> Call { get; set; }
        protected Action<T> Coroutine { get; set; }
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
                    AsyncResult = Call.BeginInvoke(null, null);
                    if (!AsyncResult.IsCompleted)
                    {
                        if(AsyncResult.AsyncWaitHandle.WaitOne( Timeout ))
                        {
                            Result = Call.EndInvoke(AsyncResult);
                            HasResult = true;
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
                        Result = Call.EndInvoke(AsyncResult);
                        HasResult = true;
                    }
                    Attempts++;
                    Thread.Sleep( TimeBetweenTries );
                }
                if (Coroutine != null && HasResult)
                    Coroutine(Result);
                return Result;
            }
        }

        public FutureResult<T> MaxRetries(int retries)
        {
            Limit = retries;
            return this;
        }

        public FutureResult<T> OnValue(Action<T> handle)
        {
            Coroutine = handle;
            if (HasResult)
                handle(Value);
            return this;
        }

        public FutureResult<T> TimeBetweenRetries(int miliseconds)
        {
            TimeBetweenTries = TimeSpan.FromMilliseconds(miliseconds);
            return this;
        }

        public FutureResult<T> TimeBetweenRetries(TimeSpan span)
        {
            TimeBetweenTries = span;
            return this;
        }

        public FutureResult<T> WaitFor(TimeSpan span)
        {
            Timeout = span;
            return this;
        }

        public FutureResult<T> WaitFor(int miliseconds)
        {
            Timeout = TimeSpan.FromMilliseconds(miliseconds);
            return this;
        }

        public FutureResult(Func<T> call)
        {
            Limit = 1;
            TimeBetweenTries = TimeSpan.Zero;
            Timeout = TimeSpan.FromMilliseconds(-1);
            Call = call;
        }

        public static implicit operator T(FutureResult<T> future)
        {
            return future.Value;
        }
    }
}