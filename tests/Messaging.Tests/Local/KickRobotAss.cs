using System;
using Symbiote.Messaging;

namespace Messaging.Tests.Local
{
    public class KickRobotAss
    {
        public string CorrelationId { get; set; }
        public string Target { get; set; }
        public DateTime Created { get; set; }

        public KickRobotAss()
        {
        }

        public KickRobotAss(string correlationId, string target)
        {
            CorrelationId = correlationId;
            Target = target;
            Created = DateTime.Now;
        }
    }
}