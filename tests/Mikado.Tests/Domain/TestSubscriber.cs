using System;
using System.Collections.Generic;
using Symbiote.Mikado;

namespace Mikado.Tests.Domain
{
    public class TestSubscriber : IObserver<IBrokenRuleNotification>
    {
        public List<IBrokenRuleNotification> BrokenRules = new List<IBrokenRuleNotification>();
        public bool ThrowException { get; set; }

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
            if(!ThrowException)
                BrokenRules.Add( value );
            else
                throw new Exception("TestSubscriber was told to throw an exception...");
        }
    }
}