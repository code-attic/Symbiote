using System;
using PublishDemo;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;

namespace SubscribeDemo
{
    public class HandleMessage : IMessageHandler<Message>
    {
        private static long total = 0;

        public void Process(Message message, IMessageDelivery messageDelivery)
        {
            ++total;
            //"Received: {0}. {1} total."
            //    .ToInfo<Subscriber>(message.Body, ++total);
            
            
            messageDelivery.Acknowledge();
        }
    }
}