using System;
using System.Collections.Generic;

namespace Symbiote.Mikado.Impl
{
    public class BrokenRulesCollection : List<IBrokenRuleNotification>, IObserver<IBrokenRuleNotification>
    {
        public void OnCompleted()
        {
            // NoOp
        }

        public void OnError(Exception error)
        {
            // NoOp
        }

        public void OnNext(IBrokenRuleNotification value)
        {
            this.Add(value);
        }
    }
}