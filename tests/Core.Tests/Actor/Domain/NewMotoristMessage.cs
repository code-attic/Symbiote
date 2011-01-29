using System;

namespace Actor.Tests.Domain
{
    public class NewMotoristMessage
    {
        public string SSN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        public NewMotoristMessage() {}
        public NewMotoristMessage( string ssn, string firstName, string lastName, DateTime dateOfBirth )
        {
            SSN = ssn;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
    }
}