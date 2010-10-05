using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Retlang.Channels;
using Retlang.Fibers;

namespace Jackalope.Tests.RetLanging
{
    public class when_blasting
    {
        protected static int receiveCount = 0;
        protected static int messageCount = 1000000;
        protected static int fiberz = 10;
        private Because of = () =>
                                 {
                                     var queues = new List<IFiber>();
                                     var reset = new AutoResetEvent(false);
                                     var channel = new Channel<int>();


                                     var updateLock = new object();

                                     Action<int> onReceive = delegate
                                     {
                                         Thread.Sleep(3);
                                         //lock (updateLock)
                                         //{
                                             receiveCount++;
                                             if (receiveCount == messageCount)
                                             {
                                                 reset.Set();
                                             }
                                         //}
                                     };
                                     using (var fiber = new PoolFiber())
                                     {
                                         fiber.Start();
                                         channel.Subscribe(fiber, onReceive);
                                         for (var i = 0; i < messageCount; i++)
                                         {
                                             channel.Publish(i);
                                         }
                                     }
                                     reset.WaitOne(1000, false);
                                     queues.ForEach(delegate(IFiber q) { q.Dispose(); });
                                 };

        private It should_process_a_million_smessages = () => receiveCount.ShouldEqual(messageCount);
    }
}
