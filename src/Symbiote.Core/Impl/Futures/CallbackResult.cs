﻿/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Threading;

namespace Symbiote.Core.Impl.Futures
{
    public class CallbackResult : IAsyncResult
    {
        protected ManualResetEvent Reset { get; set; }

        public bool IsCompleted { get; set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return Reset; }
        }
        public object AsyncState
        {
            get { return null; }
        }
        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public void Set()
        {
            Reset.Set();
        }

        public CallbackResult()
        {
            Reset = new ManualResetEvent( false );
        }
    }
}