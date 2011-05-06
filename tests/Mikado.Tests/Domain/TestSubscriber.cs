using System;
using System.Collections.Generic;
using Symbiote.Mikado;

namespace Mikado.Tests.Domain
{
    public class TestSubscriber : IListenToBrokenRules
    {
        public List<IBrokenRuleNotification> BrokenRules = new List<IBrokenRuleNotification>();
        public bool ThrowException { get; set; }

        public void OnBrokenRule(IBrokenRuleNotification value)
        {
            if(!ThrowException)
                BrokenRules.Add( value );
            else
                throw new Exception("TestSubscriber was told to throw an exception...");
        }
    }
}