using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Demo.Messages;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Jackalope;
using System.Linq;
using Symbiote.Jackalope.Impl.Router;
using Symbiote.Log4Net;
using Symbiote.StructureMap;

namespace PublishDemo
{
    public class Publisher : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            var sends = 0m;
            var observable = Observable.Generate(0, x => x < 500000, x => x + 1, x => x);
            var watch = new Stopwatch();
            var timer = new Timer(e =>
                                      {
                                          try
                                          {
                                              "{0} messages in {1} seconds for a {2} milisecond per send average"
                                                  .ToInfo<Publisher>(
                                                      sends,
                                                      watch.Elapsed.TotalSeconds,
                                                      watch.ElapsedMilliseconds/sends);
                                          }
                                          catch (Exception exception)
                                          {
                                          }
                                      },
                                    null, 
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(1));
            
            observable
                .ToEnumerable()
                //.AsParallel()
                //.ForAll(m =>
                .ForEach(m =>
                    {
                        if(sends == 0)
                            watch.Start();
                        sends += 2;
                        Bus.Send("publisher", new Message("Hello"));
                        //Bus.Send("secondary", new Message("Hello"));
                    });
            watch.Stop();
        }

        public void Stop()
        {
            
        }
        
        public void Initialize()
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Jackalope(x => x.AddServer(s => s.AMQP091().Address("localhost")))
                .AddConsoleLogger<IBus>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddConsoleLogger<Publisher>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .Dependencies(x => x.Scan(y =>
                {
                    y.TheCallingAssembly();
                    y.AddSingleImplementations();
                }));
        }

        public Publisher()
        {
            Bus = ServiceLocator.Current.GetInstance<IBus>();
            Bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout));
            //Bus.AddEndPoint(x => x.QueueName("subscriber").LoadBalanced());
            Bus.AddEndPoint(x => x.QueueName("subscriber"));
            Bus.BindQueue("subscriber", "publisher");
            //Bus.AddEndPoint(
            //    x => x.Exchange("control", ExchangeType.fanout).QueueName("routing").RoutingKeys("subscriber").Broker("control"));
            //Bus.DefineRouteFor<SubscriberOnline>(x => x.SendTo("control").WithRoutingKey(s => s.SourceQueue));
        }
    }
}