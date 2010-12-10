using Symbiote.Actor;

namespace Actor.Tests.Domain.Events
{
    public class DriverChangedName
        : ActorEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DriverChangedName() {}

        public DriverChangedName( string firstName, string lastName )
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}