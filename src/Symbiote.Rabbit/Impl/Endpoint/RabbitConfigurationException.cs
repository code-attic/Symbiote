using System;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class RabbitConfigurationException : Exception
    {
        public RabbitConfigurationException(string message) : base(message)
        {   
        }
    }
}