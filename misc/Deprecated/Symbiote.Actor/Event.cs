using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Actor
{
    public abstract class ActorEvent
        : IEvent
    {
        public Guid EventId { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public DateTime UtcTimeStamp { get; set; }
    }
}
