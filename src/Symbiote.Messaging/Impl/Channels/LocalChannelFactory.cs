using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Dispatch;

namespace Symbiote.Messaging.Impl.Channels
{
    public class LocalChannelFactory<TMessage>
        : IChannelFactory<TMessage>
    {
        public IDispatcher Dispatcher { get; set; }

        public IChannel CreateChannel( IChannelDefinition definition )
        {
            var typedDef = definition as LocalChannelDefinition<TMessage>;
            var channel = Activator.CreateInstance(typedDef.ChannelType, Dispatcher, typedDef) as IChannel<TMessage>;
            return channel;
        }

        public LocalChannelFactory( IDispatcher dispatcher )
        {
            Dispatcher = dispatcher;
        }
    }
}
