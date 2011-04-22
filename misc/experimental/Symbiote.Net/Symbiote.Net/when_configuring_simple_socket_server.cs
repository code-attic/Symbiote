using Machine.Specifications;
using Symbiote.Core;

namespace Symbiote.Net
{
    public class when_configuring_simple_socket_server
        : with_assimilation
    {
        static SocketConfiguration configuration;
        static int expected = 12345;

        private Because of = () => 
                                 { 
                                     configuration = Assimilate.GetInstanceOf<SocketConfiguration>();
                                 };

        private It should_get_correct_instance = () => configuration.ShouldNotBeNull();
        private It should_have_expected_port = () => configuration.Port.ShouldEqual( expected );
    }
}