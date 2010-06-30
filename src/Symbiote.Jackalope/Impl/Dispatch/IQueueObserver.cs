using System;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public interface IQueueObserver : 
        IObservable<Envelope>, 
        IDisposable
    {
        long SleepFor { get; set; }
        void Start(string queueName);
        void Stop();
    }
}