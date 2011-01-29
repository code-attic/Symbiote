using Actor.Tests.Domain.Model;
using Symbiote.Core.Actor;
using Symbiote.Messaging;

namespace Actor.Tests.Domain
{
    public class NewMotoristHandler
        : IHandle<NewMotoristMessage>
    {
        public IAgent<Driver> Drivers { get; set; }
        public DriverFactory Factory { get; set; }

        public void Handle( IEnvelope<NewMotoristMessage> envelope )
        {
            var message = envelope.Message;
            var driver = Factory.CreateNewDriver( message.SSN, message.FirstName, message.LastName, message.DateOfBirth );
            Drivers.RegisterActor( driver.SSN, driver );
        }

        public NewMotoristHandler( IAgent<Driver> drivers, DriverFactory factory )
        {
            Drivers = drivers;
            Factory = factory;
        }
    }
}