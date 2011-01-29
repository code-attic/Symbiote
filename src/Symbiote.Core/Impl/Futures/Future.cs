using System;
using System.Threading;

namespace Symbiote.Core.Impl.Futures
{
    public class Future
    {
        public static FutureResult<T> Of<T>( Func<T> call )
        {
            return new FutureResult<T>( call );
        }

        public static FutureCallback<T> Of<T>(Action<Action<T>> call)
        {
           return new FutureCallback<T>( call );
        }
    
        public static FutureAction Blind(Action call)
        {
            return new FutureAction( call );
        }

        public static FutureAction<T> Of<T>(Action call)
        {
            return new FutureAction<T>(call);
        }
    }

    public abstract class Future<T>
    {
        protected IAsyncResult AsyncResult { get; set; }
        protected int Attempts { get; set; }
        protected Action<T> Coroutine { get; set; }
        protected bool HasResult { get; set; }
        protected bool ActiveCall;
        protected int Limit { get; set; }
        protected Func<T> OnFail { get; set; }
        protected Action<Exception> ExceptionHandler { get; set; }
        protected T Result { get; set; }
        protected TimeSpan Timeout { get; set; }
        protected TimeSpan TimeBetweenTries { get; set; }

        protected T Value
        {
            get
            {
                while (Attempts < Limit && !HasResult)
                {
                    InvokeCall();
                    if (!AsyncResult.IsCompleted)
                    {
                        AsyncResult.AsyncWaitHandle.WaitOne(Timeout);
                    }
                    Attempts++;
                    Thread.Sleep(TimeBetweenTries);
                }

                if (!HasResult && Attempts >= Limit)
                    Result = OnFail();
                
                if (Coroutine != null && HasResult)
                    Coroutine(Result);

                return Result;
            }
        }

        protected void Init()
        {
            Limit = 1;
            TimeBetweenTries = TimeSpan.Zero;
            Timeout = TimeSpan.FromMilliseconds(-1);
            OnFail = () => default(T);
        }

        protected abstract void InvokeCall();

        public Future<T> MaxRetries(int retries)
        {
            Limit = retries;
            return this;
        }

        //public Future<T> Now()
        //{
        //    var value = Value;
        //    return this;
        //}

        public Future<T> OnFailure(Func<T> onFailure)
        {
            OnFail = onFailure;
            return this;
        }

        public Future<T> OnException(Action<Exception> exceptionHandler)
        {
            ExceptionHandler = exceptionHandler;
            return this;
        }

        public Future<T> OnValue(Action<T> handle)
        {
            Coroutine = handle;
            if (HasResult)
                handle(Value);
            return this;
        }

        public Future<T> TimeBetweenRetries(int miliseconds)
        {
            TimeBetweenTries = TimeSpan.FromMilliseconds(miliseconds);
            return this;
        }

        public Future<T> TimeBetweenRetries(TimeSpan span)
        {
            TimeBetweenTries = span;
            return this;
        }

        public Future<T> WaitFor(TimeSpan span)
        {
            Timeout = span;
            return this;
        }

        public Future<T> WaitFor(int miliseconds)
        {
            Timeout = TimeSpan.FromMilliseconds(miliseconds);
            return this;
        }

        public static implicit operator T(Future<T> future)
        {
            return future.Value;
        }
    }
}
