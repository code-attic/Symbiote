using Messaging.Tests.Channels.Manager;
using Symbiote.Messaging;

namespace Messaging.Tests.Actor.Agent
{
    public class DummyHandler :
        IHandle<DummyActor, DummyMessage>
    {
        public void Handle( DummyActor actor, IEnvelope<DummyMessage> envelope )
        {
            
        }
    }
}