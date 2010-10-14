using System;
using System.Diagnostics;
using System.Threading;
using Demo.Messages;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;

namespace SubscribeDemo
{
    public class HandleMessage : IMessageHandler<Message>
    {
        private static double latencyTotal = 0;
        private static double count = 0;
        private static Stopwatch watch = new Stopwatch();


        public void Process(Message message, IMessageDelivery messageDelivery)
        {
            count++;
            latencyTotal += DateTime.Now.Subtract(message.Created).TotalMilliseconds;

            var avgLatency = latencyTotal / (count == 0 ? 1 : count);
            var msgsPerSecond = count / watch.Elapsed.TotalSeconds;
            if(count%1000 == 0)
            {
                "{0} msgs total. {1} msgs/sec. {2} ms latency / msg."
                    .ToInfo<Subscriber>(count, msgsPerSecond, avgLatency);
            }

            messageDelivery.Acknowledge();
        }

        public HandleMessage()
        {
            if(!watch.IsRunning)
                watch.Start();
        }
    }

    //var latencyTotal1 = 0d;
    //        var latencyTotal2 = 0d;
    //        var count1 = 0d;
    //        var count2 = 0d;
    //        var watch = new Stopwatch();
    //        Bus
    //            .QueueStreams["subscriber"]
    //            .Do(x =>
    //                    {
    //                        if(count1 == 0)
    //                            watch.Start();
    //                        x.MessageDelivery.Acknowledge();
    //                        count1++;
    //                        latencyTotal1 += DateTime.Now.Subtract((x.Message as Message).Created).TotalMilliseconds;
    //                    })
    //            .BufferWithTime(TimeSpan.FromSeconds(1))
    //            .Subscribe(x =>
    //            {
    //                var avgLatency = latencyTotal1 / (count1 == 0 ? 1 : count1);
    //                var msgsPerSecond = count1 / watch.Elapsed.TotalSeconds;
    //                "Q1: {0} msgs total. {1} msgs/sec. {2} ms latency / msg."
    //                    .ToInfo<Subscriber>(count1, msgsPerSecond, avgLatency);
    //                Thread.Sleep(TimeSpan.FromMilliseconds(93));
    //            });
}