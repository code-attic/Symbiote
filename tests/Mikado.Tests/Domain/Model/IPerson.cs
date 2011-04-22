namespace Mikado.Tests.Domain.Model
{
    public interface IPerson : IHaveAge, IHaveFirstName, IHaveLastName, ITestKey
    {
        
    }

    public interface IAddress : IHaveAddress, IHaveCity, IHaveState, IHaveZipCode
    {
        
    }

    public interface IHaveAddress
    {
        string Address { get; set; }
    }

    public interface IHaveCity
    {
        string City { get; set; }
    }

    public interface IHaveState
    {
        string State { get; set; }
    }

    public interface IHaveZipCode
    {
        string ZipCode { get; set; }
    }
}