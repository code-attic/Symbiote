﻿using System;

namespace Symbiote.Core.Work
{
    public abstract class EventBase : IEvent
    {
        public Guid EventId { get; set; }
        public string ActorType { get; set; }
        public string ActorId { get; set; }
        public DateTime UtcTimeStamp { get; set; }
    }
}
