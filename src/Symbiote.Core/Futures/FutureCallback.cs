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
using System.Threading;

namespace Symbiote.Core.Futures
{
    public class FutureCallback<T>
        : Future<T>
    {
        protected Action<Action<T>> Call { get; set; }
        protected Func<T> GetResult { get; set; }
        protected CallbackResult ResetTrigger { get; set; }

        protected override void InvokeCall()
        {
            AsyncResult = Call.BeginInvoke( Callback, CloseHandle, null );
        }

        protected void CloseHandle( IAsyncResult result )
        {
            if ( result != null )
            {
                Call.EndInvoke( result );
            }
        }

        public void Callback( T value )
        {
            Result = value;
            HasResult = true;
            ((ManualResetEvent) AsyncResult.AsyncWaitHandle).Set();
        }

        public static implicit operator Action<T>( FutureCallback<T> future )
        {
            return future.Callback;
        }

        public FutureCallback( Action<Action<T>> call )
        {
            Init();
            Call = call;
            GetResult = () => default(T);
        }
    }
}