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
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Router
{
    public abstract class BaseRouter : 
        IMessageHandler<ICorrelate>,
        IMessageHandler<SubscriberOnline>
    {
        protected IBus Bus { get; set; }
        protected ConcurrentDictionary<string, RoutingGroup> RoutingGroups { get; set; }
        
        public void Process(ICorrelate message, IMessageDelivery messageDelivery)
        {
            var group = GetRoutingGroup(messageDelivery.Queue);
            var exchange = group.Exchanges.GetNode(message.CorrelationId);
            Bus.Send(exchange, message);
            group.RegisterSentMessage(exchange, messageDelivery);
        }

        protected virtual RoutingGroup GetRoutingGroup(string sourceQueue)
        {
            return RoutingGroups[sourceQueue];
        }

        public void Process(SubscriberOnline message, IMessageDelivery messageDelivery)
        {
            try
            {
                RoutingGroup routing = null;
                var clientExchange = message.ClientExchange;
                var sourceQueue = message.SourceQueue;

                if(!RoutingGroups.TryGetValue(clientExchange, out routing))
                {
                    routing = new RoutingGroup(sourceQueue);
                    Bus.Subscribe(sourceQueue);
                }
                routing.Exchanges.AddNode(clientExchange, sourceQueue);
                messageDelivery.Acknowledge();
            }
            catch (Exception)
            {
                messageDelivery.Reject();
            }
        }

        protected BaseRouter(IBus bus)
        {
            Bus = bus;
            RoutingGroups = new ConcurrentDictionary<string, RoutingGroup>();
        }
    }

    public class RoutingGroup
    {
        public string SourceQueue { get; set; }
        public Distributor<string> Exchanges { get; set; }
        protected ConcurrentDictionary<string, ConcurrentDictionary<string, IMessageDelivery>> ClientMessages { get; set; }

        public void RegisterSentMessage(string clientName, IMessageDelivery messageDelivery)
        {
            ConcurrentDictionary<string, IMessageDelivery> messages = null;
            if(!ClientMessages.TryGetValue(clientName, out messages))
            {
                messages = new ConcurrentDictionary<string, IMessageDelivery>();
                ClientMessages[clientName] = messages;
            }
            messages[messageDelivery.Details.MessageId] = messageDelivery;
        }

        public void AcknowledgeSentMessage(string clientName, string messageId)
        {
            ConcurrentDictionary<string, IMessageDelivery> messages = null;
            if (ClientMessages.TryGetValue(clientName, out messages))
            {
                IMessageDelivery delivery = null;
                if(messages.TryGetValue(messageId, out delivery))
                {
                    delivery.Acknowledge();
                }
            }
        }

        public void ReleaseClientMessages(string clientName)
        {
            ConcurrentDictionary<string, IMessageDelivery> messages = null;
            if (ClientMessages.TryRemove(clientName, out messages))
            {
                messages.ForEach(x => x.Value.Reject());
            }
        }

        public RoutingGroup(string sourceQueue)
        {
            SourceQueue = sourceQueue;
            Exchanges = new Distributor<string>(1200);
            ClientMessages = new ConcurrentDictionary<string, ConcurrentDictionary<string, IMessageDelivery>>();
        }
    }

    public class SubscriberOnline
    {
        public IAmqpEndpointConfiguration Configuration { get; set; }
        public string SourceQueue { get; set; }
        public string ClientExchange { get; set; }

        public SubscriberOnline()
        {
        }

        public SubscriberOnline(IAmqpEndpointConfiguration configuration, string sourceQueue, string exchangeAlias)
        {
            Configuration = configuration;
            SourceQueue = sourceQueue;
            ClientExchange = exchangeAlias;
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
