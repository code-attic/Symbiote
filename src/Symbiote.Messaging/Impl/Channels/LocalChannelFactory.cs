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
            var channel = Activator.CreateInstance(typedDef.ChannelType, Dispatcher, typedDef) as IChannel;
            return channel;
        }

        public LocalChannelFactory( IDispatcher dispatcher )
        {
            Dispatcher = dispatcher;
        }
    }

    public class LocalChannelFactory
        : IChannelFactory
    {
        public IDispatcher Dispatcher { get; set; }

        public IChannel CreateChannel(IChannelDefinition definition)
        {
            var channel = new LocalChannel( Dispatcher, definition as LocalChannelDefinition );
            return channel;
        }

        public LocalChannelFactory(IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }
    }
}
