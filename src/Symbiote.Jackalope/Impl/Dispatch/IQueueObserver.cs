using System;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public delegate void ObserverShutdown(IQueueObserver observer);

    public interface IQueueObserver : 
        IObservable<Envelope>, 
        IDisposable
    {
        event ObserverShutdown OnShutdown;
        void Start(string queueName);
        void Stop();
    }
}