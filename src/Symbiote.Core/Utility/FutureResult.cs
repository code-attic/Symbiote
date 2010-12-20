using System;
using System.Threading;

namespace Symbiote.Core.Utility
{
    public class FutureResult<T>
        : Future<T>
    {
        protected Func<T> Call { get; set; }

        protected override void InvokeCall()
        {
            AsyncResult = Call.BeginInvoke(CloseHandle, null);
        }

        protected void CloseHandle(IAsyncResult result)
        {
            if (result != null)
            {
                Call.EndInvoke(result);
            }
        }

        public FutureResult(Func<T> call)
        {
            Limit = 1;
            TimeBetweenTries = TimeSpan.Zero;
            Timeout = TimeSpan.FromMilliseconds(-1);
            Call = call;
        }

        
    }
}