using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Envelope;

namespace Symbiote.JsonRpc.Channel
{
    public class JsonRpcChannelDefinition<TMessage>
        : BaseChannelDefinition<TMessage>
    {
        public override Type ChannelType
        {
            get { return typeof(); }
        }
        public override Type FactoryType
        {
            get {  }
        }
    }

    public class JsonRpcChannel<TMessage>
        : IChannel<TMessage>
    {
        public JsonRpcChannelDefinition<TMessage> Definition { get; set; }

        public string Name
        {
            get { return Definition.Name; }
        }
        public void ExpectReply<TReply>( TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply )
        {
            var envelope = new Envelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
            };

            modifyEnvelope(envelope);
            dispatcher.ExpectResponse<TReply>(envelope.MessageId.ToString(), onReply);

            
        }

        public IEnvelope<TMessage> Send( TMessage message )
        {
            var envelope = new Envelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
            };

            
            return envelope;
        }

        public IEnvelope<TMessage> Send( TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope )
        {
            var envelope = new Envelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
            };

            modifyEnvelope(envelope);

            
            return envelope;
        }
    }
}
