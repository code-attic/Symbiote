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
