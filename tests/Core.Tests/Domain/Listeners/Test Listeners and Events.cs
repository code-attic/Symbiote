using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.Domain.Listeners
{
    public class DriverGenericEvent : IEvent
    {
        public Guid EventId { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public DateTime UtcTimeStamp { get; set; }
    }

    public class DriverGenericEventChild : DriverGenericEvent
    {
        // test sub type to verify that the IEventListenerManager is taking inheritance into account
    }

    public class VehicleGenericEvent : IEvent
    {
        public Guid EventId { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public DateTime UtcTimeStamp { get; set; }
    }

    public class VehicleGenericEventChild : VehicleGenericEvent
    {
        // test sub type.  the StaticVehicleEventListener class should NOT be listening for this one....
    }

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

    public class StaticVehicleEventListener : EventListenerBase<VehicleGenericEvent>
    {
        public List<VehicleGenericEvent> Events { get; set; }

        public StaticVehicleEventListener() : base ( false )
        {
            Events = new List<VehicleGenericEvent>();
        }

        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnNext(VehicleGenericEvent value)
        {
            Events.Add( value );
        }
    }

    public class StaticListenerForEverything : EventListenerBase<IEvent>
    {
        public List<IEvent> Events { get; set; }

        public StaticListenerForEverything()
        {
            Events = new List<IEvent>();
        }

        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnNext(IEvent value)
        {
            Events.Add( value );
        }
    }
}
