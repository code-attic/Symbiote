using Machine.Specifications;
using Symbiote.Core;

namespace Symbiote.Net
{
    public class with_socket_server
        : with_assimilation
    {
        protected static ISocketServer Server { get; set; }

        private Establish context = () => 
                                        {
                                            Server = Assimilate.GetInstanceOf<ISocketServer>();
                                        };
    }
}