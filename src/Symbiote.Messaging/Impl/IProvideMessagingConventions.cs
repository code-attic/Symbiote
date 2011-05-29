using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl
{
    public interface IProvideMessagingConventions
    {
        void CreateCompetingQueues();
        void CreateConsumerExchange();
        void CreateConsumerQueue();
        void CreateExchangeToExchangeBindings();
        void CreateTypeBasedExchanges();
        void CreateWireTaps();
        void StartSubscriptions();
    }
}
