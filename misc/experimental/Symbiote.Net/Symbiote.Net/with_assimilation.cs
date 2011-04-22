using Machine.Specifications;
using Symbiote.Core;

namespace Symbiote.Net
{
    public class with_assimilation
    {
        private Establish context = () => 
                                        {
                                            SocketHostAssimilation.SocketServer( Assimilate
                                                                   .Initialize(), x => x.ListenOn( 12345 ) );
                                        };
        

    }
}