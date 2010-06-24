using System;

namespace Symbiote.Jackalope
{
    public interface IQueueStreamCollection
    {
        IObservable<Envelope> this[string queueName] { get; }
    }
}