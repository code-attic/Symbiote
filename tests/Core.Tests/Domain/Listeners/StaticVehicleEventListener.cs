using System;
using System.Collections.Generic;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.Domain.Listeners
{
    public class StaticVehicleEventListener : EventListenerBase<VehicleGenericEvent>
    {
        public List<VehicleGenericEvent> Events { get; set; }

        public StaticVehicleEventListener() : base ( false )
        {
            Events = new List<VehicleGenericEvent>();
        }

        public override void OnEvent(VehicleGenericEvent value)
        {
            Events.Add( value );
        }
    }
}