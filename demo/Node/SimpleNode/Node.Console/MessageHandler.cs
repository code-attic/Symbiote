using System;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Mesh;

namespace Node.Console
{
    public class MessageHandler
        : IHandle<Message>
    {
        public INodeIdentityProvider IdentityProvider { get; set; }

        public Action<IEnvelope> Handle( Message message )
        {
            "{0} got a message: {1}"
                .ToDebug<NodeService>(IdentityProvider.Identity, message.Text);
            return x => x.Acknowledge();
        }

        public MessageHandler( INodeIdentityProvider identityProvider )
        {
            IdentityProvider = identityProvider;
        }
    }
}