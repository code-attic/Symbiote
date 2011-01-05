/* 
Copyright 2008-2010 Jim Cowart

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
