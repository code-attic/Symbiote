﻿using System;
using Symbiote.Core;

namespace Core.Tests.Domain.Model
{
    public class VehicleKeyAccessor : IKeyAccessor<Vehicle>
    {
        public string GetId( Vehicle actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( Vehicle actor, TKey id )
        {
            actor.Id = Guid.Parse( id.ToString() );
        }
    }

    public class Vehicle
    {
        public string VIN { get; protected set; }
        public string Make { get; protected set; }
        public string Model { get; protected set; }
        public int Year { get; protected set; }
        public Guid Id { get; set; }

        public void Populate(IVehicleMemento vehicleMemento)
        {
            vehicleMemento.Vin = VIN;
            vehicleMemento.Make = Make;
            vehicleMemento.Model = Model;
            vehicleMemento.Year = Year;
        }

        public void PopulateFrom(IVehicleMemento vehicleMemento)
        {
            VIN = vehicleMemento.Vin;
            Make = vehicleMemento.Make;
            Model = vehicleMemento.Model;
            Year = vehicleMemento.Year;
        }

        public Vehicle(IVehicleMemento vehicleMemento)
        {
            PopulateFrom( vehicleMemento );
        }

        public Vehicle( string vin, string make, string model, int year )
        {
            VIN = vin;
            Make = make;
            Model = model;
            Year = year;
            Id = Guid.NewGuid();
        }
    }
}