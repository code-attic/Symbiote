using System;
using Symbiote.Core;

namespace Symbiote.Net
{
    public static class SocketHostAssimilation
    {
        public static IAssimilate SocketServer( this IAssimilate assimilate, Action<ISocketConfigurator> configure )
        {
            var configurator = new SocketConfigurator();
            configure( configurator );
            assimilate.Dependencies( x => 
                                         {
                                             x.For<SocketConfiguration>().Use( configurator.Configuration );
                                         } );


            return assimilate;
        }
    }
}