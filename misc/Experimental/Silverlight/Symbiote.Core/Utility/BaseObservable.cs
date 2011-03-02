/* 
Copyright 2008-2010 Alex Robson

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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Symbiote.Core.Utility
{
    public abstract class BaseObservable<TNotification>
        : IObservable<TNotification>, IDisposable
    {
        protected List<IObserver<TNotification>> observers { get; set; }

        public virtual void Notify(TNotification notification)
        {
            observers.ForEach(x => x.OnNext(notification));
        }

        public virtual void SendCompletion()
        {
            observers.ForEach(x => x.OnCompleted());
        }

        public virtual IDisposable Subscribe(IObserver<TNotification> observer)
        {
            var disposable = this as IDisposable;
            observers.Add(observer);
            return disposable;
        }

        protected BaseObservable()
        {
            this.observers = new List<IObserver<TNotification>>();
        }

        public void Dispose()
        {
            observers.Clear();
            observers = null;
        }
    }
}
