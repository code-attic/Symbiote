using System;
using Core.Tests.Actor.Domain.Model;
using Symbiote.Core;

namespace Core.Tests.Actor.Domain.Memoization
{
    public class AddressMemento : IAddressMemento, IMemento<Address>
    {
        public int StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public void Capture( Address instance )
        {
            StreetName = instance.StreetName;
            StreetNumber = instance.StreetNumber;
            City = instance.City;
            State = instance.State;
            Zip = instance.Zip;
        }

        public void Reset( Address instance )
        {
            StreetName = instance.StreetName;
            StreetNumber = instance.StreetNumber;
            City = instance.City;
            State = instance.State;
            Zip = instance.Zip;
        }

        public Address Retrieve()
        {
            return new Address( StreetNumber, StreetName, City, State, Zip );
        }
    }
}