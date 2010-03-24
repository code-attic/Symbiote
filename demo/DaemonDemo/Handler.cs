using System;
using System.Threading;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;
using Symbiote.Jackalope;

namespace DaemonDemo
{
    public class Handler : IMessageHandler<IMessage>
    {
        private ILogger<Demo> _log;
        private Demo _demoService;
        public static int total = 0;

        public void Process(IMessage message, IResponse response)
        {
            //var rnd = new Random(DateTime.Now.Millisecond).Next(100);
            //if (rnd >= 99)
            //    throw new Exception("Because I can...");

            _demoService.Configure();
            if(message.Rejection)
            {
                "Message Rejected :("
                    .ToInfo<Demo>();
            }
            else if (message.Reject)
            {
                message.Rejection = true;
                response.Reject();
            }
            else
            {
                "Received: \"{0}\" On [{1}]"
                .ToInfo<Demo>(
                     message.Body,
                     message.Created);
                response.Acknowledge();
            }
            total++;
        }

        public Handler(ILogger<Demo> log, Demo demo)
        {
            _log = log;
            _demoService = demo;
        }
    }

    public class HandlerOne : IMessageHandler<MessageOne>
    {
        public void Process(MessageOne message, IResponse response)
        {
            "HandlerOne got a message too!"
                .ToInfo<Demo>();
        }
    }
}