using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;

namespace Symbiote.Messaging.Impl.Channels
{
    public class LocalChannelFactory<TMessage>
        : IChannelFactory<TMessage>
    {
        public IChannel CreateChannel( IChannelDefinition definition )
        {
            var typedDef = definition as LocalChannelDefinition<TMessage>;
            var channel = Assimilate.GetInstanceOf( typedDef.ChannelType ) as IChannel<TMessage>;
            channel.Name = definition.Name;
            channel.CorrelationMethod = typedDef.CorrelationMethod;
            channel.RoutingMethod = typedDef.RoutingMethod;
            return channel;
        }
    }
}
