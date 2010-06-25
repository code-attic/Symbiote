using System;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public interface ISubscription: IDisposable
    {
        IObservable<Envelope> MessageStream { get; }
        void Start();
        void Stop();
        bool Starting { get; }
        bool Started { get; }
        bool Stopping { get; }
        bool Stopped { get; }
    }
}