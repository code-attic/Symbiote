using System;
using RabbitMQ.Client;
using Symbiote.Jackalope.Impl;

namespace Symbiote.Jackalope
{
    public class Envelope : IDisposable
    {
        public bool Empty { get { return MessageDelivery == null || Message == null; } }
        public IMessageDelivery MessageDelivery { get; set; }
        public object Message { get; set; }
        public string CorrelationId
        {
            get
            {
                var correlate = Message as ICorrelate;
                if (correlate == null)
                    return "";
                return correlate.CorrelationId;
            }
        }

        public void Dispose()
        {
            (MessageDelivery as IDisposable).Dispose();
        }
    }
}