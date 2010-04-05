using System;

namespace Symbiote.WebSocket
{
    public class DefaultClientObserver : BaseClientObserver
    {
        public DefaultClientObserver(Action<Tuple<string, string>> messageReceived) : base(messageReceived)
        {
        }
    }
}