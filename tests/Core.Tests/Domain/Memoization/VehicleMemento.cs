using System;
using Core.Tests.Domain.Model;
using Symbiote.Core;

namespace Core.Tests.Domain.Memoization
{
    public class VehicleMemento : IMemento<Vehicle>
    {
        public string Vin { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }

        public void Capture( Vehicle instance )
        {
            Vin = instance.VIN;
            Make = instance.Make;
            Model = instance.Model;
            Year = instance.Year;
        }

        public void Reset( Vehicle instance )
        {
            Vin = instance.VIN;
            Make = instance.Make;
            Model = instance.Model;
            Year = instance.Year;
        }

        public Vehicle Retrieve()
        {
            return new Vehicle( Vin, Make, Model, Year );
        }
    }
}