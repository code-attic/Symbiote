using System;

namespace Symbiote.Jackalope.Impl
{
    public interface IQueueObserver : 
        IObservable<Envelope>, 
        IDisposable
    {
        void Start(string queueName);
        void Stop();
    }
}