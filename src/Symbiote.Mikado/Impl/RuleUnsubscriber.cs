using System;
using System.Collections.Generic;

namespace Symbiote.Mikado.Impl
{
    public class RuleUnsubscriber : IDisposable
    {
        private List<IObserver<IBrokenRuleNotification>> _observers;
        private IObserver<IBrokenRuleNotification> _observer;

        public RuleUnsubscriber(List<IObserver<IBrokenRuleNotification>> observers, IObserver<IBrokenRuleNotification> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public virtual void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
