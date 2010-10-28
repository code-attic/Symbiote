using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl.Channels
{
    public interface IConfigureChannel<TMessage>
    {
        IConfigureChannel<TMessage> Named( string channelName );
        IConfigureChannel<TMessage> CorrelateBy( string correlationId );
        IConfigureChannel<TMessage> CorrelateBy( Func<TMessage, string> messageProperty );
        IConfigureChannel<TMessage> RouteBy(string routingKey);
        IConfigureChannel<TMessage> RouteBy(Func<TMessage, string> messageProperty);
    }
}
