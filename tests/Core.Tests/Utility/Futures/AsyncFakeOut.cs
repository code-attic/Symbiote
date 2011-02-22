using System;
using System.Threading;
using Symbiote.Core.Futures;

namespace Core.Tests.Utility.Futures
{
    public class AsyncFakeOut
    {
        private int _value = 1;

        private CallbackResult _trigger = new CallbackResult();

        public IAsyncResult BeginRead(AsyncCallback callback)
        {
            _trigger.Reset();
            Thread.Sleep( 2 );
            _trigger.Set();
            callback(_trigger);
            return _trigger;
        }

        public int EndRead(IAsyncResult result)
        {
            result.AsyncWaitHandle.WaitOne();
            return _value++;
        }
    }
}