using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Messaging.Tests.Local;
using Symbiote.Messaging;

namespace Messaging.Tests.RequestResponse
{
    public class when_making_request
        : with_bus
    {
        public static Reply Reply { get; set; }

        private Because of = () =>
        {
            bus.AddLocalChannel();
            Reply = bus.Request<Request, Reply>("local", new Request()).WaitFor( 10 );
        };

        private It should_have_response = () => Reply.Text.ShouldEqual( "I have an answer!" );
    }
}
