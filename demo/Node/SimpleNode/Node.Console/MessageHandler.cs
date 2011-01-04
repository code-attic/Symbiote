using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Mesh;

namespace Node.Console
{
    public class MessageHandler
        : IHandle<Message>
    {
        public INodeIdentityProvider IdentityProvider { get; set; }

        public void Handle( IEnvelope<Message> envelope )
        {
            "{0} got a message: {1}"
                .ToDebug<NodeService>(IdentityProvider.Identity, envelope.Message.Text);
        }

        public MessageHandler( INodeIdentityProvider identityProvider )
        {
            IdentityProvider = identityProvider;
        }
    }
}