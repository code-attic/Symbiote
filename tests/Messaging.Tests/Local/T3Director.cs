using System;
using Symbiote.Messaging;

namespace Messaging.Tests.Local
{
    public class T3Director
        : IHandle<Actor, KickRobotAss>
    {
        public void Handle(Actor samWorthington, IEnvelope<KickRobotAss> envelope)
        {
            samWorthington.KickTheCrapOutOf(envelope.Message.Target);
            samWorthington.Lag = DateTime.Now.Subtract( envelope.Message.Created );
        }

        public T3Director()
        {
        }
    }
}
