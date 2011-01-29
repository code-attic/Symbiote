using Actor.Tests.Domain.Model;

namespace Actor.Tests.Domain.Memoization
{
    public class AddressMemento : IAddressMemento
    {
        public int StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}