using System;
using System.Collections.Generic;
using Symbiote.Mikado;

namespace Mikado.Tests.Domain
{
    public class TestSubscriber : IObserver<IBrokenRuleNotification>
    {
        public List<IBrokenRuleNotification> BrokenRules = new List<IBrokenRuleNotification>();

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IBrokenRuleNotification value)
        {
            BrokenRules.Add( value );
        }
    }
}