using System;

namespace Symbiote.Core.Impl.Futures
{
    public class FutureResult<T>
        : Future<T>
    {
        protected Func<T> Call { get; set; }
        protected CallbackResult ResetTrigger { get; set; }
        protected Action<IAsyncResult> OnResult { get; set; }

        protected override void InvokeCall()
        {
           Call.BeginInvoke(CloseHandle, null);
        }

        protected void CloseHandle(IAsyncResult result)
        {
            if (result != null)
            {
                var value = Call.EndInvoke(result);
                if(!HasResult)
                {
                    Result = value;
                    HasResult = true;
                    ResetTrigger.Set();
                }
            }
        }

        public FutureResult(Func<T> call)
        {
            Init();
            Call = call;
            ResetTrigger = new CallbackResult();
            AsyncResult = ResetTrigger;
        }
    }
}