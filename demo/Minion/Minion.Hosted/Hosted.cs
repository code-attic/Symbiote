using Minion.Messages;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Messaging;
using Symbiote.Rabbit;

namespace Minion.Hosted
{
    public class Hosted : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            "Hosted Daemon has been summoned!"
                .ToInfo<IMinion>();
            Bus.AddLocalChannel();
            Bus.AddRabbitChannel(x => x.Direct("Host").AutoDelete().Durable().PersistentDelivery());

            Bus.AddRabbitChannel(x => x.Direct("Hosted").AutoDelete());
            Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Hosted").ExchangeName("Hosted").StartSubscription().NoAck());

            Bus.Publish( "Host", new MinionUp() { Text = "You rang?"} );
        }

        public void Stop()
        {
            "Alas, I am dead fellah!"
                .ToInfo<IMinion>();
        }

        public Hosted(IBus bus)
        {
            Bus = bus;
        }
    }
}