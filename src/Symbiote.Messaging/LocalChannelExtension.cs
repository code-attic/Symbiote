using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging.Impl.Channels;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Messaging
{
    public static class LocalChannelExtension
    {
        public static IBus AddLocalEndpoint(this IBus bus, string channelName)
        {
            IChannelManager manager = ServiceLocator.Current.GetInstance<IChannelManager>();
            manager.AddDefinition(new NameOnlyChannelDefinition(channelName));
            return bus;
        }
    }
}
