using System;
using Symbiote.Messaging;

namespace Messaging.Tests.RequestResponse
{
    public class RequestHandler : IHandle<Request>
    {
        public Action<IEnvelope> Handle( Request message )
        {
            return x => x.Reply( new Reply()
            {
                Text = "I have an answer!"
            } );
        }
    }
}