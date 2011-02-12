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

namespace Symbiote.Core.Futures
{
    public class FutureResult<T>
        : Future<T>
    {
        protected Func<T> Call { get; set; }
        protected Action<IAsyncResult> OnResult { get; set; }

        protected override void InvokeCall()
        {
            Call.BeginInvoke( CloseHandle, null );
        }

        protected void CloseHandle( IAsyncResult result )
        {
            if ( result != null )
            {
                var value = Call.EndInvoke( result );
                if ( !HasResult )
                {
                    Result = value;
                    HasResult = true;
                    ResetTrigger.Set();
                }
                if ( Coroutine != null && HasResult )
                    Coroutine( Result );
                Loop();
            }
            if ( !HasResult && Attempts >= Limit )
                Result = OnFail();
        }

        public FutureResult( Func<T> call )
        {
            Call = call;
            ResetTrigger = new CallbackResult();
            AsyncResult = ResetTrigger;
        }
    }
}