using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Rabbit;
using Symbiote.Daemon;
using Symbiote.Log4Net;

namespace RequestResponse
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Messaging()
                .Rabbit( x => x.AddBroker( r => r.Defaults() ) )
                .AddConsoleLogger<Service>( x => x.Info().MessageLayout( m => m.Message().Newline() ) )
                .Daemon( x => x.Name( "Test" ).Arguments( args ) )
                .RunDaemon();
        }
    }

    public class Service : IDaemon
    {
        protected IBus Bus { get; set; }

        public void Start()
        {
            "Starting"
                .ToInfo<Service>();

            for (int i = 0; i < 10; i++)
            {
                Bus.Request<Request, Response>( new Request(), HandleResponse );
                //Bus.Publish( "request", new Request());
            }
        }

        public void Stop()
        {
            "Stopping"
                .ToInfo<Service>();
        }

        public void HandleResponse(Response response)
        {
            "Response {0} received."
                .ToInfo<Service>(response.Count);
        }

        public Service( IBus bus )
        {
            Bus = bus;

            bus.AddRabbitChannel( x => x.AutoDelete().Direct( "request" ) );
            bus.AddRabbitQueue(x => x.AutoDelete().QueueName("request").ExchangeName("request").StartSubscription().NoAck());
        }
    }

    public class Request
    {
        
    }

    public class Response
    {
        public int Count { get; set; }
    }

    public class RequestHandler : IHandle<Request>
    {
        public static int Count { get; set; }

        public void Handle( IEnvelope<Request> envelope )
        {
            "Got a request..."
                .ToInfo<Service>();
            envelope.Reply( new Response()
            {
                Count = ++Count
            } );
        }
    }
}
