using System;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public interface IQueueObserver : 
        IObservable<Envelope>, 
        IDisposable
    {
        void Start(string queueName);
        void Stop();
    }
}