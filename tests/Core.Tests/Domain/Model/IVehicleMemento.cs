namespace Core.Tests.Domain.Model
{
    public interface IVehicleMemento
    {
        string Vin { get; set; }
        string Make { get; set; }
        string Model { get; set; }
        int Year { get; set; }
    }
}