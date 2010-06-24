using System.Threading;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;

namespace PublishDemo
{
    public class Publisher
    {
        private IBus _bus;

        public void Start()
        {
            "Publisher is starting.".ToInfo<Publisher>();
            long i = 0;
            while(true)
            {
                _bus.Send("publisher", new Message("Message {0}".AsFormat(++i)));
                //Thread.Sleep(3000);
            }
        }

        public Publisher(IBus bus)
        {
            _bus = bus;
            _bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout).Durable().PersistentDelivery());
        }
    }
}