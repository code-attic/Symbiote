using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeChannelManager
    {
        void InitializeChannels();
        void AddNewOutgoingChannel( string channelName );
    }
}
