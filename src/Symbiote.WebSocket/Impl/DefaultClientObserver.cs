using System;

namespace Symbiote.WebSocket.Impl
{
    public class DefaultClientObserver : BaseClientObserver
    {
        public DefaultClientObserver(Action<Tuple<string, string>> messageReceived) : base(messageReceived)
        {
        }
    }
}