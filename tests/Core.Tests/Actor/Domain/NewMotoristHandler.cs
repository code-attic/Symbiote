using System;
using Core.Tests.Actor.Domain.Model;
using Symbiote.Core.Actor;
using Symbiote.Messaging;

namespace Core.Tests.Actor.Domain
{
    public class NewMotoristHandler
        : IHandle<NewMotoristMessage>
    {
        public IAgent<Driver> Drivers { get; set; }
        public DriverFactory Factory { get; set; }

        public NewMotoristHandler(IAgent<Driver> drivers, DriverFactory factory)
        {
            Drivers = drivers;
            Factory = factory;
        }

        public Action<IEnvelope> Handle(NewMotoristMessage message)
        {
            var driver = Factory.CreateNewDriver(message.SSN, message.FirstName, message.LastName, message.DateOfBirth);
            Drivers.RegisterActor(driver.SSN, driver);
            return x => x.Acknowledge();
        }
    }
}