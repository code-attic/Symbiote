using System;
using System.Collections.Generic;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.Domain.Listeners
{
    public class StaticDriverEventListener : EventListenerBase<DriverGenericEvent>
    {
        public List<DriverGenericEvent> Events { get; set; }

        public StaticDriverEventListener()
        {
            Events = new List<DriverGenericEvent>();
        }
        
        public override void OnEvent(DriverGenericEvent value)
        {
            Events.Add( value );
        }
    }
}