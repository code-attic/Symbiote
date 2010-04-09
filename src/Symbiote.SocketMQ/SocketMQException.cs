using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.SocketMQ
{
    public class SocketMQException : Exception
    {
        public SocketMQException(string message) : base(message)
        {
        }

        public SocketMQException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
