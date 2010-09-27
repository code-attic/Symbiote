using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Hashing;
using Symbiote.Jackalope.Config;

namespace Symbiote.Jackalope.Impl.Router
{
    public abstract class BaseRouter : 
        IMessageHandler<ICorrelate>,
        IMessageHandler<SubscriberOnline>
    {
        protected ConcurrentDictionary<string, RoutingGroup> 

        public void Process(ICorrelate message, IMessageDelivery messageDelivery)
        {
            
        }

        public void Process(SubscriberOnline message, IMessageDelivery messageDelivery)
        {
            
        }
    }

    public class RoutingGroup
    {
        public string Alias { get; set; }
        public Distributor<string> Exchanges { get; set; }


    }

    public class SubscriberOnline
    {
        public IAmqpEndpointConfiguration Configuration { get; set; }
        public string ExchangeAlias { get; set; }

        public SubscriberOnline()
        {
        }

        public SubscriberOnline(IAmqpEndpointConfiguration configuration, string exchangeAlias)
        {
            Configuration = configuration;
            ExchangeAlias = exchangeAlias;
        }
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
