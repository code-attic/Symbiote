using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Rabbit;

namespace Rabbit.Tests
{
    //public class when_sending_400000_messages_to_60_actors
    //    : with_rabbit_configuration
    //{
    //    protected static List<Actor> cast { get; set; }
    //    protected static Stopwatch receiveWatch { get; set; }
    //    protected static Stopwatch sendWatch { get; set; }
    //    protected static int MessagesToSend = 400000;
    //    protected static int actorCount = 60;
    //    protected static IDispatcher dispatcher;

    //    private Because of = () =>
    //        {
    //            Actor.Created = 0;

    //            Bus.AddRabbitChannel(x => x.Direct("test").CorrelateBy<Message>(m => m.CorrelationId).AutoDelete());
    //            Bus.AddRabbitQueue(x => x.ExchangeName("test").QueueName("test").AutoDelete().NoAck());
            
    //            var names = Enumerable.Range(0, actorCount).Select(x => "Extra " + x).ToList();
    //            var message = Enumerable.Range(0, actorCount)
    //                .Select(x => new Message() { CorrelationId = names[x] })
    //                .ToList();

            
    //            sendWatch = Stopwatch.StartNew();
    //            for (int i = 0; i < MessagesToSend; i++)
    //            {
    //                message[i % actorCount].Id = i;
    //                Bus.Publish("test", message[i % actorCount]);
    //                //if(i % 1000 == 0)
    //                //    Bus.CommitChannelOf<Message>();
    //            }
    //            sendWatch.Stop();

    //            Bus.StartSubscription("test");
    //            receiveWatch = Stopwatch.StartNew();
    //            Thread.Sleep(TimeSpan.FromSeconds(1));
    //            receiveWatch.Stop(); 

    //            dispatcher = Assimilate.GetInstanceOf<IDispatcher>();
    //        };
        
    //    private It should_receive_in_1_second = () =>
    //                                            receiveWatch.ElapsedMilliseconds.ShouldBeLessThan(10);

    //    private It should_send_in_1_second = () =>
    //                                         sendWatch.ElapsedMilliseconds.ShouldBeLessThan(10);

    //    private It should_only_have_created_the_actor_60_times = () =>
    //                                                             Actor.Created.ShouldEqual(actorCount);

    //    private It should_have_sent_all_messages_to_actor = () =>
    //                                                        Actor.MessageIds.Count.ShouldEqual(MessagesToSend);
        
    //    private It should_have_all_teh_dispatchers = () => dispatcher.Count.ShouldEqual(MessagesToSend);
    //}
}