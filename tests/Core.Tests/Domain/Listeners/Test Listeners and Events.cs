using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Impl.UnitOfWork;

namespace Core.Tests.Domain.Listeners
{
    public class DriverGenericEvent : IEvent
    {
        public Guid EventId { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public DateTime UtcTimeStamp { get; set; }
    }

    public class VehicleGenericEvent : IEvent
    {
        public Guid EventId { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public DateTime UtcTimeStamp { get; set; }
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

        public StaticVehicleEventListener()
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
            Events.Add(value);
        }
    }
}
