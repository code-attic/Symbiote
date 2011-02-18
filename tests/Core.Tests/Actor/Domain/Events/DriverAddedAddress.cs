
namespace Core.Tests.Actor.Domain.Events
{
    public class DriverAddedAddress
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
}
