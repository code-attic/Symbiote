using System;
using System.Threading;
using Demo.Messages;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;

namespace SubscribeDemo
{
    public class HandleMessage //: IMessageHandler<Message>
    {
        private static long total = 0;

        public void Process(Message message, IMessageDelivery messageDelivery)
        {
            //++total;
            //"Received: {0} - {2}. Latency = {1} ms"
            //    .ToInfo<Subscriber>(
            //    message.Body,
            //    DateTime.Now.Subtract(message.Created).TotalMilliseconds,
            //    messageDelivery.Details.MessageId);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            messageDelivery.Acknowledge();
        }
    }
}