using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Messaging.Impl.Mesh;
using Symbiote.Messaging.Impl.Saga;

namespace Symbiote.Messaging.Config
{
    public class MessagingScan : IDefineScanningInstructions
    {
        public Action<IScanInstruction> Scan()
        {
            return scan =>
                       {
                           scan.ConnectImplementationsToTypesClosing(
                               typeof( IHandle<> ) );
                           scan.ConnectImplementationsToTypesClosing(
                               typeof( IHandle<,> ) );
                           scan.AddAllTypesOf<INodeHealthBroadcaster>();
                           scan.AddAllTypesOf<INodeChannelManager>();
                           scan.AddAllTypesOf<ISaga>();
                           scan.ConnectImplementationsToTypesClosing(typeof(ISaga<>));
                       };
        }
    }
}