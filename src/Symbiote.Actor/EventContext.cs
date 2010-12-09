using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Actor.Impl.Eventing;
using Symbiote.Core;

namespace Symbiote.Actor
{
    public class EventContext
    {
        private static IEventContextProvider _provider;
        protected static IEventContextProvider ContextProvider
        {
            get
            {
                _provider = _provider ?? Assimilate.GetInstanceOf<IEventContextProvider>();
                return _provider;
            }
        }

        public static IEventContext CreateFor<TActor>(TActor instance)
            where TActor : class
        {
            return ContextProvider.GetContext(instance);
        }
    }
}
