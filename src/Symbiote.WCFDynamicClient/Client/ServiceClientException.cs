using System;
using System.Runtime.Serialization;

namespace Symbiote.Wcf.Client
{
    public class ServiceClientException : Exception
    {
        public ServiceClientException()
        {
        }

        public ServiceClientException(string message)
            : base(message)
        {
        }

        public ServiceClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ServiceClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}