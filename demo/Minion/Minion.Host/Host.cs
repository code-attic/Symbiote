using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Messaging;
using Symbiote.Rabbit;

namespace Minion.Host
{
    public class Host : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            "Host has started."
                .ToInfo<IMinion>();

            Bus.AddLocalChannel();
            Bus.AddRabbitChannel(x => x.Direct("Host").AutoDelete());
            Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Host").ExchangeName("Host").StartSubscription().NoAck());

            Bus.AddRabbitChannel(x => x.Direct("Hosted").AutoDelete());
            Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Hosted").ExchangeName("Hosted").NoAck());
        }

        public void Stop()
        {
                
        }

        public Host( IBus bus )
        {
            Bus = bus;
        }
    }
}