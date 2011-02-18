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

        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnNext(DriverGenericEvent value)
        {
            Events.Add( value );
        }
    }
}