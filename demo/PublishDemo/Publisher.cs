using System;
using System.Collections.Generic;
using System.Threading;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using System.Linq;

namespace PublishDemo
{
    public class Publisher
    {
        private IBus _bus;
        private Action<string, Message> send;

        public void Start()
        {
            //"Publisher is starting.".ToInfo<Publisher>();
            //long i = 0;
            //var message = new Message("Message");
            //while(true)
            //{
            //    send.BeginInvoke("publisher", message, null, null);
            //}
        }

        public Publisher(IBus bus)
        {

            _bus = bus;
            _bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout));
            _bus.AddEndPoint(x => x.Exchange("secondary", ExchangeType.fanout));
            _bus.DefineRouteFor<Message>(x => x.SendTo("publisher"));
            var observable = Observable.Generate(0, x => x < 10000, x => new Message("Message"), x => x + 1);
            _bus.AutoRouteFromSource(observable);
            send = bus.Send;
        }
    }
}