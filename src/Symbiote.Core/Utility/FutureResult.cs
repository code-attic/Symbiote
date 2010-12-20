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
                var value = Call.EndInvoke(result);
                if(!HasResult)
                {
                    Result = value;
                    HasResult = true;
                }
            }
        }

        public FutureResult(Func<T> call)
        {
            Init();
            Call = call;
        }
    }
}