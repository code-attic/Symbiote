﻿

namespace Core.Tests.Actor.Domain.Events
{
    public class DriverChangedName
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