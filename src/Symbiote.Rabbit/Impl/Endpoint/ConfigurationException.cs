using System;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {   
        }
    }
}