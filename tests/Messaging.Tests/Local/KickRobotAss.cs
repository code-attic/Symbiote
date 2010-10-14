using Symbiote.Messaging;

namespace Messaging.Tests.Local
{
    public class KickRobotAss
        : ICorrelate
    {
        public string CorrelationId { get; set; }
        public string Target { get; set; }

        public KickRobotAss()
        {
        }

        public KickRobotAss(string correlationId, string target)
        {
            CorrelationId = correlationId;
            Target = target;
        }
    }
}