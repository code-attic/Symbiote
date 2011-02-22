using System;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.Domain.Listeners
{
    public class VehicleGenericEvent : IEvent
    {
        public Guid EventId { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public DateTime UtcTimeStamp { get; set; }
    }
}