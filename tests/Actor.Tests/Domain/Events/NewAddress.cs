using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actor.Tests.Domain.Model;
using Symbiote.Actor;

namespace Actor.Tests.Domain.Events
{
    public class DriverAddedAddress
        : ActorEvent
    {
        public int StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public DriverAddedAddress() {}

        public DriverAddedAddress( int streetNumber, string streetName, string city, string state, string zip )
        {
            StreetNumber = streetNumber;
            StreetName = streetName;
            City = city;
            State = state;
            Zip = zip;
        }
    }

    public class DriverChangedName
        : ActorEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
