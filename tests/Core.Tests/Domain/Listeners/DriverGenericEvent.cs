using System;
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
}
