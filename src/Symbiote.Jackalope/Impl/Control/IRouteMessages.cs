/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
