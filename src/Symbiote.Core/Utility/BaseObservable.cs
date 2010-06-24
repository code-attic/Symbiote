using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Utility
{
    public abstract class BaseObservable<TNotification>
        : IObservable<TNotification>, IDisposable
    {
        protected ConcurrentBag<IObserver<TNotification>> observers { get; set; }

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
            this.observers = new ConcurrentBag<IObserver<TNotification>>();
        }

        public void Dispose()
        {
            while (observers.Count > 0)
            {
                IObserver<TNotification> o;
                observers.TryTake(out o);
            }
        }
    }
}
