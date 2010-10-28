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
        public IChannel CreateChannel( IChannelDefinition definiton )
        {
            var typedDef = definiton as LocalChannelDefinition<TMessage>;
            var channel = Assimilate.GetInstanceOf( typedDef.ChannelType ) as IChannel<TMessage>;
            channel.Name = definiton.Name;
            channel.CorrelationMethod = typedDef.CorrelationMethod;
            channel.RoutingMethod = typedDef.RoutingMethod;
            return channel;
        }
    }
}
