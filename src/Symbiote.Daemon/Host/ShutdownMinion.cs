using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Subscriptions;

namespace Symbiote.Daemon.Host
{
    [Serializable]
    [DataContract]
    public class ShutdownMinion
    {
        [DataMember( Order = 1 )]
        public DateTime Issued { get; set; }
        [DataMember( Order = 2 )]
        public string Reason { get; set; }
    }

    public class ShutdownHandler :
        IHandle<ShutdownMinion>
    {
        public ISubscriptionManager SubscriptionManager { get; set; }

        public Action<IEnvelope> Handle( ShutdownMinion message )
        {
            SubscriptionManager.StopAllSubscriptions();
            return x => x.Acknowledge();
        }

        public ShutdownHandler( ISubscriptionManager subscriptionManager )
        {
            SubscriptionManager = subscriptionManager;
        }
    }
}
