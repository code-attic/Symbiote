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
    public class FutureAsyncCallback<T>
        : Future<T>
    {
        protected Func<AsyncCallback, IAsyncResult> Call { get; set; }
        protected Func<IAsyncResult, T> End { get; set; }
        protected Func<T> GetResult { get; set; }

        protected override void InvokeCall()
        {
            AsyncResult = Call( Callback );
        }

        public void Callback(IAsyncResult result)
        {
            if(result != null)
            {
                Result = End(result);
                HasResult = true;
                ((ManualResetEvent)ResetTrigger.AsyncWaitHandle).Set();
                if(Coroutine != null)
                    Coroutine( Result );
            }
        }

        public static implicit operator Func<T>(FutureAsyncCallback<T> future)
        {
            return () => future.Value;
        }

        public FutureAsyncCallback(Func<AsyncCallback, IAsyncResult> call, Func<IAsyncResult, T> callback)
        {
            Init();
            Call = call;
            End = callback;
            GetResult = () => default(T);
            ResetTrigger = new CallbackResult();
        }
    }
}