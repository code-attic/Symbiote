using System;
using System.Threading;

namespace Symbiote.Core.Utility
{
    public class FutureCallback<T>
        : Future<T>
    {
        protected Action<Action<T>> Call { get; set; }
        protected Func<T> GetResult { get; set; }

        protected override void InvokeCall()
        {
            AsyncResult = Call.BeginInvoke(Callback, CloseHandle, null);
        }

        protected void CloseHandle(IAsyncResult result)
        {
            if(result != null)
            {
                Call.EndInvoke(result);
            }
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
            Result = value;
            HasResult = true;
            ((ManualResetEvent) AsyncResult.AsyncWaitHandle).Set();
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