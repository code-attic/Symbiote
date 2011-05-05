using System;
using System.Collections.Generic;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.Domain.Listeners
{
    public class StaticListenerForEverything : EventListenerBase<IEvent>
    {
        public List<IEvent> Events { get; set; }

        public StaticListenerForEverything()
        {
            Events = new List<IEvent>();
        }

        public override void OnEvent(IEvent value)
        {
            Events.Add( value );
        }
    }
}