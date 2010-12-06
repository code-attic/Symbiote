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
        public static string Reply {get;set;}
        public static void OnReply(Reply reply)
        {
            Reply = reply.Text;
        }
        private Because of = () =>
        {
            bus.AddLocalChannel();
            bus.Request<Request, Reply>( new Request(), OnReply );
            Thread.Sleep( 10 );
        };

        private It should_have_response = () => Reply.ShouldEqual( "I have an answer!" );
    }
}
