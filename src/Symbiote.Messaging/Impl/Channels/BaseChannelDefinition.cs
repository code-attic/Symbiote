using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl.Channels
{
    public abstract class BaseChannelDefinition<TMessage> :
        IChannelDefinition<TMessage>
    {
        public string Name { get; set; }
        public Type MessageType { get; private set; }
        public Func<TMessage, string> RoutingMethod { get; set; }
        public Func<TMessage, string> CorrelationMethod { get; set; }
        public abstract Type ChannelType { get; }
        public abstract Type FactoryType { get; }

        public IConfigureChannel<TMessage> Named( string channelName )
        {
            Name = channelName;
            return this;
        }

        public IConfigureChannel<TMessage> CorrelateBy( string correlationId )
        {
            CorrelationMethod = x => correlationId;
            return this;
        }

        public IConfigureChannel<TMessage> CorrelateBy( Func<TMessage, string> messageProperty )
        {
            CorrelationMethod = messageProperty;
            return this;
        }

        public IConfigureChannel<TMessage> RouteBy( string routingKey )
        {
            RoutingMethod = x => routingKey;
            return this;
        }

        public IConfigureChannel<TMessage> RouteBy( Func<TMessage, string> messageProperty )
        {
            RoutingMethod = messageProperty;
            return this;
        }

        protected BaseChannelDefinition()
        {
            Name = "default";
            MessageType = typeof(TMessage);
            RoutingMethod = x => "";
            CorrelationMethod = x => "";
        }
    }
}
