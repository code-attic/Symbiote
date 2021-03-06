﻿// /* 
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
using System.Threading;

namespace Symbiote.Core.Futures
{
    public class Future
    {
        public static FutureResult<T> Of<T>( Func<T> call )
        {
            return new FutureResult<T>( call );
        }

        public static FutureCallback<T> Of<T>( Action<Action<T>> call )
        {
            return new FutureCallback<T>( call );
        }

        public static FutureAsyncCallback<T> Of<T>(Func<AsyncCallback, IAsyncResult> call, Func<IAsyncResult, T> callback)
        {
            return new FutureAsyncCallback<T>( call, callback );
        }

        public static FutureAction WithoutResult( Action call )
        {
            return new FutureAction( call );
        }

        public static FutureAction<T> Of<T>( Action call )
        {
            return new FutureAction<T>( call );
        }
    }

    public abstract class Future<T>
    {
        protected bool ActiveCall;
        protected IAsyncResult AsyncResult { get; set; }
        protected int Attempts { get; set; }
        protected Action<T> Coroutine { get; set; }
        protected Func<bool> CycleWhile { get; set; }
        protected Action<Exception> ExceptionHandler { get; set; }
        protected bool HasResult { get; set; }
        protected int Limit { get; set; }
        protected Func<T> OnFail { get; set; }
        protected CallbackResult ResetTrigger { get; set; }
        protected T Result { get; set; }
        protected TimeSpan Timeout { get; set; }
        protected TimeSpan TimeBetweenTries { get; set; }

        protected void Loop()
        {
            if( CycleWhile() )
            {
                Attempts = 0;
                Result = default(T);
                HasResult = false;
                ActiveCall = false;
                ResetTrigger = new CallbackResult();
                GetResult();
            }
        }

        protected T Value
        {
            get
            {
                while ( Attempts < Limit && !HasResult )
                {
                    GetResult();
                    if ( !AsyncResult.IsCompleted )
                    {
                        AsyncResult.AsyncWaitHandle.WaitOne( Timeout );
                        ActiveCall = false;
                    }
                    Thread.Sleep( TimeBetweenTries );
                }
                if ( !HasResult && Attempts >= Limit )
                    Result = OnFail();

                return Result;
            }
        }

        protected void GetResult()
        {
            if (!ActiveCall)
            {
                ActiveCall = true;
                InvokeCall();
                Attempts++;
            }
        }

        protected Future()
        {
            Limit = 1;
            CycleWhile = () => false;
            TimeBetweenTries = TimeSpan.Zero;
            Timeout = TimeSpan.FromMilliseconds( -1 );
            OnFail = () => default(T);
        }

        protected abstract void InvokeCall();

        public Future<T> LoopWhile(Func<bool> condition)
        {
            CycleWhile = condition;
            return this;
        }

        public Future<T> MaxRetries( int retries )
        {
            Limit = retries;
            return this;
        }

        public Future<T> OnException( Action<Exception> exceptionHandler )
        {
            ExceptionHandler = exceptionHandler;
            return this;
        }

        public Future<T> OnFailure( Func<T> onFailure )
        {
            OnFail = onFailure;
            return this;
        }

        public Future<T> OnValue( Action<T> handle )
        {
            Coroutine = handle;
            if ( HasResult )
                handle( Value );
            return this;
        }

        public Future<T> Start()
        {
            GetResult();
            return this;
        }

        public Future<T> TimeBetweenRetries( int miliseconds )
        {
            TimeBetweenTries = TimeSpan.FromMilliseconds( miliseconds );
            return this;
        }

        public Future<T> TimeBetweenRetries( TimeSpan span )
        {
            TimeBetweenTries = span;
            return this;
        }

        public Future<T> WaitFor( TimeSpan span )
        {
            Timeout = span;
            return this;
        }

        public Future<T> WaitFor( int miliseconds )
        {
            Timeout = TimeSpan.FromMilliseconds( miliseconds );
            return this;
        }

        public static implicit operator T( Future<T> future )
        {
            return future.Value;
        }

    }
}