using System.Threading;
using Machine.Specifications;
using Symbiote.Rabbit;

namespace Rabbit.Tests
{
    public class when_sending_messages
        : with_rabbit_configuration
    {
        private Because of = () =>
            {
                Bus.AddRabbitExchange( x => x.Direct( "test" ).CorrelateBy<Message>( m => m.CorrelationId ).AutoDelete() );
                Bus.AddRabbitQueue(x => x.ExchangeName("test").QueueName("test").AutoDelete().NoAck().StartSubscription());
                                        
                Bus.Publish("test", new Message() {Id = 1, CorrelationId = "1"}, x => x.RoutingKey = "test");
                Bus.Publish("test", new Message() { Id = 2, CorrelationId = "1" });
                Bus.Publish("test", new Message() { Id = 3, CorrelationId = "1" });

                Thread.Sleep(45);
            };
        
        private It actor_should_have_received_three_messages = () => Actor.MessageIds.ShouldContain(1, 2, 3);
    }
}