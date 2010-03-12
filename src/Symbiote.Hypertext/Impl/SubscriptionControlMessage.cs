using System.Linq;
using System.Text;
using Symbiote.Daemon;

namespace Symbiote.Telepathy
{
    public class SubscriptionControlMessage
    {
        public SubscriptionAction Action { get; set; }
        public string Queue { get; set; }

        public static SubscriptionControlMessage StartSubscription(string queue)
        {
            return new SubscriptionControlMessage()
                       {
                           Action = SubscriptionAction.Start,
                           Queue = queue
                       };
        }

        public static SubscriptionControlMessage StopSubscription(string queue)
        {
            return new SubscriptionControlMessage()
                       {
                           Action = SubscriptionAction.Stop,
                           Queue = queue
                       };
        }

        public static SubscriptionControlMessage StartAllSubscriptions()
        {
            return new SubscriptionControlMessage()
            {
                Action = SubscriptionAction.StartAll
            };
        }

        public static SubscriptionControlMessage StopAllSubscriptions()
        {
            return new SubscriptionControlMessage()
            {
                Action = SubscriptionAction.StopAll
            };
        }

        public SubscriptionControlMessage()
        {
            Action = SubscriptionAction.NoOp;
            Queue = "";
        }
    }
}
