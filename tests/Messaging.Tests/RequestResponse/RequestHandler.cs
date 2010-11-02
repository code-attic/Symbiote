using Symbiote.Messaging;

namespace Messaging.Tests.RequestResponse
{
    public class RequestHandler : IHandle<Request>
    {
        public void Handle( IEnvelope<Request> envelope )
        {
            envelope.Reply( new Reply()
            {
                Text = "I have an answer!"
            } );
        }
    }
}