namespace Actor.Tests.Domain.Model
{
    public class Address
    {
        public int StreetNumber { get; protected set; }
        public string StreetName { get; protected set; }
        public string City { get; protected set; }
        public string State { get; protected set; }
        public string Zip { get; protected set; }

        public void Populate(IAddressMemento addressMemento)
        {
            addressMemento.StreetNumber = StreetNumber;
            addressMemento.StreetName = StreetName;
            addressMemento.City = City;
            addressMemento.State = State;
            addressMemento.Zip = Zip;
        }

        public void PopulateFrom(IAddressMemento addressMemento)
        {
            StreetNumber = addressMemento.StreetNumber;
            StreetName = addressMemento.StreetName;
            City = addressMemento.City;
            State = addressMemento.State;
            Zip = addressMemento.Zip;
        }

        public Address(IAddressMemento addressMemento)
        {
            PopulateFrom( addressMemento );
        }

        public Address( int streetNumber, string streetName, string city, string state, string zip )
        {
            StreetNumber = streetNumber;
            StreetName = streetName;
            City = city;
            State = state;
            Zip = zip;
        }
    }
}