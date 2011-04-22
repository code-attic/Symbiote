using Symbiote.Core;

namespace Mikado.Tests.Domain.Model
{
    public class AddressMemento : IMemento<PersonAddress>
    {
        public void Capture( PersonAddress instance )
        {
            Address = instance.Address;
            City = instance.City;
            State = instance.State;
            ZipCode = instance.ZipCode;
        }

        protected string Address { get; set; }
        protected string City { get; set; }
        protected string State { get; set; }
        protected string ZipCode { get; set; }

        public void Reset( PersonAddress instance )
        {
            instance.Address = Address;
            instance.City = City;
            instance.State = State;
            instance.ZipCode = ZipCode;
        }

        public PersonAddress Retrieve()
        {
            return new PersonAddress()
                       {
                           Address = Address,
                           City = City,
                           State = State,
                           ZipCode = ZipCode
                       };
        }
    }
}