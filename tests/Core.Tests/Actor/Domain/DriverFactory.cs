using System;
using Core.Tests.Actor.Domain.Model;

namespace Core.Tests.Actor.Domain
{
    public class DriverFactory
    {
        public Driver CreateNewDriver(string ssn, string firstName, string lastName, DateTime dateOfBirth)
        {
            return new Driver(ssn, firstName, lastName, dateOfBirth);
        }
    }
}