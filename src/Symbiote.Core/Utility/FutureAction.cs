using System;
using System.Threading;

namespace Symbiote.Core.Utility
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

    public class FutureAction<T>
        : Future<T>
    {
        protected Action Call { get; set; }
        protected CallbackResult ResetTrigger { get; set; }

        protected override void InvokeCall()
        {
            Call.BeginInvoke( CloseHandle, null );
        }

        protected void CloseHandle(IAsyncResult result)
        {
            if (result != null)
            {
                Call.EndInvoke(result);
            }
        }

        public FutureAction(Action call)
        {
            Init();
            Call = call;
            ResetTrigger = new CallbackResult();
            AsyncResult = ResetTrigger;
        }

        public void Callback(T value)
        {
            Result = value;
            HasResult = true;
            ResetTrigger.Set();
        }

        public static implicit operator Action<T>(FutureAction<T> future)
        {
            return future.Callback;
        }
    }
}