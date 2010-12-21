namespace Core.Tests.Domain.Model
{
    public interface IAddressMemento
    {
        int StreetNumber { get; set; }
        string StreetName { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
    }
}