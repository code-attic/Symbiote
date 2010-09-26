using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Hashing;

namespace Symbiote.Jackalope.Impl.Router
{
    public interface IRouteMessages<TMessageId> :
        IMessageHandler<ICorrelate>
    {
        
    }

    public interface IMessageStore<TMessageId>
    {
        void Save<TMessage>(TMessage message);
        TMessage Retrieve<TMessage>(TMessageId messageId);
    }

    public interface IBusEvent
    {
        DateTime Occurred { get; set; }
        string Source { get; set; }
    }

    public abstract class BusEvent
        : IBusEvent
    {
        public DateTime Occurred { get; set; }
        public string Source { get; set; }

        protected BusEvent(string source)
        {
            Occurred = DateTime.Now;
            Source = source;
        }
    }

    public class MessageReceived
        : BusEvent
    {
        public Envelope Envelope { get; set; }

        public MessageReceived(string source, Envelope envelope) : base(source)
        {
            Envelope = envelope;
        }
    }

    public class MessageSent
        : BusEvent
    {
        public object Message { get; set; }
        public string Destination { get; set; }

        public MessageSent(string source, string destination, object message) : base(source)
        {
            Destination = destination;
            Message = message;
        }
    }

    public class Heartbeat
        : BusEvent
    {
        public Heartbeat(string source) : base(source)
        {
        }
    }

    public interface IWatcher
        : IObservable<IBusEvent>
    {
        void Notify(IBusEvent busEvent);
    }
}
