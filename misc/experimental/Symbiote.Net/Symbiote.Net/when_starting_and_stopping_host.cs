using Machine.Specifications;

namespace Symbiote.Net
{
    public class when_starting_and_stopping_host
        : with_socket_server
    {
        static bool postStart;
        static bool postStop;

        private Because of = () => 
                                 {
                                     Server.Start( x => { } );
                                     postStart = Server.Running;
                                     Server.Stop();
                                     postStop = Server.Running;
                                 };

        private It should_have_run = () => postStart.ShouldBeTrue();
        private It should_be_stopped = () => postStop.ShouldBeFalse();
    }
}