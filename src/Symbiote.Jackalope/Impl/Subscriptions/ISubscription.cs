using System;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public interface ISubscription: IDisposable
    {
        IObservable<Envelope> MessageStream { get; }
        string QueueName { get; set; }
        void Start();
        void Stop();
        bool Starting { get; }
        bool Started { get; }
        bool Stopping { get; }
        bool Stopped { get; }
    }
}