using System;
using RabbitMQ.Client;

namespace Symbiote.Jackalope
{
    public class Envelope : IDisposable
    {
        public bool Empty { get { return Respond == null || Message == null; } }
        public IRespond Respond { get; set; }
        public object Message { get; set; }
        public IBasicProperties MessageDetails
        {
            get
            {
                var details = Message as IHaveMessageDetail;
                if (details == null)
                    return null;
                return details.MessageProperties;
            }
        }
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
            (Respond as IDisposable).Dispose();
        }
    }
}