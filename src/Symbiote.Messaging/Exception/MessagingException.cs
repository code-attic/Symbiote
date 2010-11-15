using System;

namespace Symbiote.Messaging
{
    public class MessagingException : Exception
    {
        public MessagingException(string message) : base(message)
        {
        }
    }
}